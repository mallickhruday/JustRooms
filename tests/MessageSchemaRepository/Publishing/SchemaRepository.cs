using System.IO;
using LibGit2Sharp;

namespace GuestDirectBookingContracts.publishing
{
    public class SchemaRepository
    {
        protected string _contractDirectoryPath;
        protected string _examplesDirectoryPath;
        protected string _microServiceName;
        protected string _schemaRepositoryPath;
        protected string _contractsBaseWorkDir;
        protected string _examplesBaseWorkDir;

        public SchemaRepository(string schemaRepositoryPath, string microServiceName)
        {
            _schemaRepositoryPath = schemaRepositoryPath;
            _microServiceName = microServiceName;
            _contractsBaseWorkDir = _microServiceName + "/" + "contracts";
            _contractDirectoryPath = _schemaRepositoryPath + "/" + _contractsBaseWorkDir;
            _examplesBaseWorkDir = _microServiceName + "/" + "examples";
            _examplesDirectoryPath = _schemaRepositoryPath + "/" + _examplesBaseWorkDir; 


            AssertRepository();
        }

        protected void AssertDirectory(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists) directoryInfo.Create();
        }

        protected void AssertRepository()
        {
            if (!Repository.IsValid(_schemaRepositoryPath)) Repository.Init(_schemaRepositoryPath);

            //create the directory structure for our if it does not exist
            AssertDirectory(_schemaRepositoryPath + "/" + _microServiceName);

            //create a subdirectory for contracts and for examples, if they do not already exist
            AssertDirectory(_contractDirectoryPath);
            AssertDirectory(_examplesDirectoryPath);
        }
    }
}