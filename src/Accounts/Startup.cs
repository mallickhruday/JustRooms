using System;
using Accounts.Adapters.Data;
using Accounts.Ports.Handlers;
using Accounts.Ports.Policies;
using Accounts.Ports.Repositories;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;
using Paramore.Brighter.DynamoDb.Extensions;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Darker.AspNetCore;
using Polly;
using Polly.Registry;

namespace Accounts
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var useLocalAwsServices = Configuration.GetValue<bool>("AWS:UseLocalServices");
            
            if (useLocalAwsServices)
            {
                services.AddSingleton<IAmazonDynamoDB>(sp => CreateClient());
            }
            else
            {
                services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
                services.AddAWSService<IAmazonDynamoDB>();
            }
            
            services.AddScoped<DynamoDbTableBuilder>();
            services.AddScoped<IUnitOfWork, DynamoDbUnitOfWork>();
            
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetry(new[] { TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150) });
            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreaker(1, TimeSpan.FromMilliseconds(500));
            var retryPolicyAsync = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150) });
            var circuitBreakerPolicyAsync = Policy.Handle<Exception>().CircuitBreakerAsync(1, TimeSpan.FromMilliseconds(500));
            var policyRegistry = new PolicyRegistry()
            {
                { CommandProcessor.RETRYPOLICY, retryPolicy },
                { CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy },
                { CommandProcessor.RETRYPOLICYASYNC, retryPolicyAsync },
                { CommandProcessor.CIRCUITBREAKERASYNC, circuitBreakerPolicyAsync },
                {Catalog.DynamoDbAccess, retryPolicyAsync}
            }; 
            
            //TODO: Make the DynamoDb policy more realistic
            
            services.AddBrighter(options =>
                {
                    options.PolicyRegistry = policyRegistry;
                    options.CommandProcessorLifetime = ServiceLifetime.Scoped;
                })
                .AsyncHandlersFromAssemblies(typeof(AddNewAccountHandlerAsync).Assembly);

            services.AddDarker()
                .AddHandlersFromAssemblies(typeof(GetAccountByIdHandlerAsync).Assembly);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
        private IAmazonDynamoDB CreateClient()
        {
            var accessKey = Configuration.GetValue<string>("AWS_ACCESS_KEY_ID");
            var accessSecret = Configuration.GetValue<string>("AWS_SECRET_ACCESS_KEY");
            var credentials = new BasicAWSCredentials(accessKey, accessSecret);
            var serviceUrl = Configuration.GetValue<string>("DynamoDb:LocalServiceUrl");
            var clientConfig = new AmazonDynamoDBConfig { ServiceURL = serviceUrl };
            return new AmazonDynamoDBClient(credentials, clientConfig);
        }

    }
}