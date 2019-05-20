using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibGit2Sharp;
using MessageSchemaRepository;

namespace GuestDirectBookingContracts.publishing
{
    public enum FileType
    {
        Example,
        Contract
    }
    
    public class ContractPublisher : SchemaRepository
    {
        protected const int BUFFER_SIZE = 1024;

        public ContractPublisher(string schemaRepositoryPath, string microServiceName)
            :  base(schemaRepositoryPath, microServiceName)
        {}

        /// <summary>
        ///     Publish example JSON for a message to repo. Path will be
        ///     /schemaRepositoryPath/contractDirectorypath/messageName/examples
        ///     The workingFileContent will be written to disk . It will be added to the repo if not already
        ///     in existence. If changed it will be committed to the repo (though not pushed).
        ///     We do not automatically push because we want the user to push when happy with the changes.
        /// </summary>
        /// <param name="messageName">The name of the message we are publishing an example for</param>
        /// <param name="jsonMessageExample">The example to publish</param>
        /// <param name="jsonMessageExampleFileName">
        ///     The name of the file to publish the example to. Defaults to messageName.XXX
        ///     where XXX is an count of number of examples in the folder
        /// </param>
        public async Task PublishExample(
            string messageName, 
            string jsonMessageExample,
            string jsonMessageExampleFileName = null,
            Contributor author = null,
            Contributor commiter = null)
        {
            var examplesDirectoryPath = _examplesDirectoryPath + "/" + messageName;
            AssertDirectory(examplesDirectoryPath);
            await WriteExampleIfNew(examplesDirectoryPath, messageName, jsonMessageExample, jsonMessageExampleFileName, author, commiter);
        }
        
        /// <summary>
        /// Publish example Joi JavaScript file to repo. Path will be
        /// schemaRepositoryPath/exampleDirectoryPath/messageName/joiSchemaFileName
        /// The javascript will be written to disk
        /// </summary>
        /// <param name="messageName"></param>
        /// <param name="joiSchema"></param>
        /// <param name="joiSchemaFileName"></param>
        /// <param name="author"></param>
        /// <param name="committer"></param>
        /// <returns></returns>
        public async Task PublishContract(
            string messageName, 
            string joiSchema,
            string joiSchemaFileName,
            Contributor author = null,
            Contributor committer = null)
        {
            var contractsDirectoryPath = _contractDirectoryPath  + "/" + messageName;
            AssertDirectory(contractsDirectoryPath);
            await WriteContractIfNew(contractsDirectoryPath, messageName, joiSchema, joiSchemaFileName, author, committer);

        }

        private void AssertFile(string fileName)
        {
            void SafeFileCreate(string s)
            {
                FileStream fs = null;
                try
                {
                    fs = File.Create(s);
                }
                finally
                {
                    fs?.Close();
                }
            }

            if (!File.Exists(fileName)) SafeFileCreate(fileName);
        }
        
        private void CommitFile(string relativePathToRepo, Contributor author, Contributor committer)
        {
            if (author == null) author = new Contributor("author", "author@anonymous.com");
            if (committer == null) committer = new Contributor("committer", "committer@anonymous.com");
            
            using (var repo = new Repository(_schemaRepositoryPath))
            {
                repo.Index.Add(relativePathToRepo);
                repo.Commit(
                    "",
                    new Signature(author.Name, author.Email, new DateTimeOffset()),
                    new Signature(committer.Name, committer.Email, new DateTimeOffset()),
                    new CommitOptions());
            }
        }

        private async Task<bool> FilesAreEqual(string workingFileContent, string sourcePath,
            CancellationToken ct = default(CancellationToken))
        {
            using (var change = new StreamReader(new MemoryStream(new UTF8Encoding().GetBytes(workingFileContent))))
            using (var source = new StreamReader(sourcePath, Encoding.UTF8))
            {
                var changeLength = change.BaseStream.Length;
                var sourceLength = source.BaseStream.Length;

                if (changeLength != sourceLength) return false;

                var pos = 0;
                bool same;
                do
                {
                    var remainingBytes = changeLength - pos;
                    var bytesToRead = (int) Math.Min(remainingBytes, BUFFER_SIZE);

                    var changeBytes = new char[bytesToRead];
                    await change.ReadAsync(changeBytes, pos, bytesToRead);
                    var sourceBytes = new char[bytesToRead];
                    await source.ReadAsync(sourceBytes, 0, bytesToRead);

                    same = changeBytes.SequenceEqual(sourceBytes);
                    pos += bytesToRead;
                } while (same && pos < changeLength);

                return same;
            }
        }
        
        private async Task WriteContractIfNew(string contractsDirectoryPath,
            string messageName,
            string joiSchema,
            string joiSchemaFileName,
            Contributor author,
            Contributor committer)
        {
            await WriteFileIfNew(FileType.Contract, contractsDirectoryPath, messageName, joiSchema, joiSchemaFileName, author, committer);
        }

        private async Task WriteExampleIfNew(string examplesDirectoryPath,
            string messageName,
            string jsonMessageExample,
            string jsonMessageExampleFileName,
            Contributor author,
            Contributor committer)
        {
            if (string.IsNullOrEmpty(jsonMessageExampleFileName))
            {
                var dirInfo = new DirectoryInfo(examplesDirectoryPath);

                //Note because we don't take any kind of lock on the directory this is not safe against simultaneous updates
                //You may also get merge collisions
                var fileCount = dirInfo.EnumerateFileSystemInfos().Count();
                jsonMessageExampleFileName = jsonMessageExample + $"v.{fileCount + 1}";
            }

            await WriteFileIfNew(FileType.Example, examplesDirectoryPath, messageName, jsonMessageExample, jsonMessageExampleFileName, author, committer);
        }

        private async Task WriteFileIfNew(FileType fileType,
            string directoryPath,
            string messageName,
            string content,
            string fileName,
            Contributor author,
            Contributor committer)
        {
            var filePath = directoryPath + "/" + fileName;

            AssertFile(filePath);

            if (await FilesAreEqual(content, filePath)) return;

            await WriteFile(content, filePath);
            var basePath = fileType == FileType.Contract ? _contractsBaseWorkDir : _examplesBaseWorkDir;
            CommitFile(basePath + "/" + messageName + "/" + fileName, author, committer);
        }


        private async Task WriteFile(string jsonMessageExample, string fileName)
        {
            using (var change = new StreamReader(new MemoryStream(new UTF8Encoding().GetBytes(jsonMessageExample))))
            using (var fileStream = new StreamWriter(File.OpenWrite(fileName), System.Text.Encoding.UTF8))
            {
                var changeLength = change.BaseStream.Length;

                var remainingBytes = changeLength;
                do
                {
                    var bytesToWrite = (int) Math.Min(remainingBytes, BUFFER_SIZE);
                    
                    var changeBytes = new char[bytesToWrite];
                    await change.ReadAsync(changeBytes, 0, bytesToWrite);
                    await fileStream.WriteAsync(changeBytes, 0, bytesToWrite);
                    remainingBytes -= bytesToWrite;

                } while (remainingBytes > 0);
            }
        }
    }
}