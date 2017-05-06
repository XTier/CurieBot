using System;
using System.IO;

namespace DataHandling.Csv
{
    public class DataFilePathAssembler
    {
        private readonly string _rootFolder;
        private static DateTime Now => DateTime.Now;

        public DataFilePathAssembler(string rootFolder)
        {
            _rootFolder = rootFolder;
        }

        public string GetFolderPath()
        {
            var now = Now;
            return Path.Combine(_rootFolder, now.Year.ToString(), $"{Now.Month:d2}");
        }

        public string GetFileName()
        {
            return string.Concat($"{Now.Day:d2}", ".csv");
        }

        public string GetPath()
        {
            return Path.Combine(GetFolderPath(), GetFileName());
        }
    }
}