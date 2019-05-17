using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GuestDirectBookingContracts.publishing
{
    public class ContractReader : SchemaRepository
    {
        public ContractReader(string schemaRepositoryPath, string microServiceName)
            : base(schemaRepositoryPath, microServiceName)
        {}
        
        public async Task<string> ReadExample(string messageName, string jsonMessageExampleFileName)
        {
            return await ReadFile(_examplesDirectoryPath + "/" + messageName + "/" + jsonMessageExampleFileName);
        }
        
        public static async Task<string> ReadLocalExample(string baseDir,  string messageName, string jsonMessageExampleFileName)
        {
            return await ReadFile(baseDir + "/" + messageName + "/" + jsonMessageExampleFileName);
        }

        public async Task<IEnumerable<string>> ReadExamples(string messageName)
        {
            return await ReadFilesinDir(new DirectoryInfo(_examplesDirectoryPath + "/"+ messageName));
        }

        public async Task<string> ReadContract(string messageName, string joiContractFileName)
        {
            return await ReadFile(_contractDirectoryPath + "/" + messageName + "/" + joiContractFileName);
        }
        
        public async Task<string> ReadLocalContract(string baseDir,  string messageName, string joiContractFileName)
        {
            return await ReadFile(baseDir + "/" + messageName + "/" + joiContractFileName);
        }
 
        public async Task<IEnumerable<string>> ReadContracts(string messageName)
        {
            return await ReadFilesinDir(new DirectoryInfo(_contractDirectoryPath + "/" + messageName));
        }


        private static async Task<string> ReadFile(string filePath)
        {
            using (var source = new StreamReader(filePath, Encoding.UTF8))
            {
                return await source.ReadToEndAsync();
            }
        }
        
       private static async Task<IEnumerable<string>> ReadFilesinDir(DirectoryInfo dirInfo)
       {
           var examples = new List<string>();
           foreach (var fileInfo in dirInfo.EnumerateFiles("*.json", SearchOption.TopDirectoryOnly))
           {
               using (var source = fileInfo.OpenText())
               {
                   examples.Add(await source.ReadToEndAsync());
               }
           }

           return examples;
       }

 
     }
}