using System;
using DirectBooking.adapters.data;
using DirectBooking.ports.handlers;
using DirectBooking.ports.mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Darker.AspNetCore;
using Polly;
using Polly.Registry;

namespace DirectBooking
{
    /// <summary>
    /// Configure the WebHost
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Get the configuration data for the host
        /// </summary>
        public IConfiguration Configuration { get; }
        
        /// <summary>
        /// Sets the configuration for the bost
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container. 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetry(new[] { TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150) });
            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreaker(1, TimeSpan.FromMilliseconds(500));
            var retryPolicyAsync = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150) });
            var circuitBreakerPolicyAsync = Policy.Handle<Exception>().CircuitBreakerAsync(1, TimeSpan.FromMilliseconds(500));
            var policyRegistry = new PolicyRegistry()
            {
                { CommandProcessor.RETRYPOLICY, retryPolicy },
                { CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy },
                { CommandProcessor.RETRYPOLICYASYNC, retryPolicyAsync },
                { CommandProcessor.CIRCUITBREAKERASYNC, circuitBreakerPolicyAsync }
            }; 
            
            var messageStore = new InMemoryOutbox();
            var gatewayConnection = new RmqMessagingGatewayConnection
            {
                AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672")),
                Exchange = new Exchange("hotel.booking.exchange"),
            };
            var producer = new RmqMessageProducer(gatewayConnection);

            services.AddBrighter(options =>
                {
                    options.PolicyRegistry = policyRegistry;
                    options.BrighterMessaging = new BrighterMessaging(messageStore, producer);
                    options.CommandProcessorLifetime = ServiceLifetime.Scoped;
                })
                .AsyncHandlersFromAssemblies(typeof(BookGuestRoomOnAccountHandlerAsync).Assembly)
                .MapperRegistryFromAssemblies(typeof(GuestRoomBookingMadeMapper).Assembly);

            services.AddDarker(options => options.QueryProcessorLifetime = ServiceLifetime.Scoped)
                .AddHandlersFromAssemblies(typeof(GetBookingByIdHandlerAsync).Assembly);

            services.AddOpenApiDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Accounts API";
                    document.Info.Description = "Hotel customers who have accounts with us";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Ian Cooper",
                        Email = string.Empty,
                        Url = "https://twitter.com/icooper"
                    };
                };
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddDbContext<BookingContext>(options =>
                options.UseMySql(Configuration["Database:Bookings"]));

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
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
            app.UseOpenApi(); 
            app.UseSwaggerUi3();
            app.UseMvc();
        }
   }
}