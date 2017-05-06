using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using DataHandling.Core.Entities;
using DataHandling.Csv.Mappings;
using Serilog;

namespace DataHandling.Csv
{
    public abstract class CsvDataProvider
    {
        protected readonly DataFilePathAssembler _filePathAssembler;

        protected CsvDataProvider(string rootFolder)
        {
            _filePathAssembler = new DataFilePathAssembler(rootFolder);
        }

        protected Co2Reading RetrieveValue()
        {
            var path = _filePathAssembler.GetPath();
            var readings = GetRecords(path);
            return readings?.Last();
        }

        private static List<Co2Reading> GetRecords(string path)
        {
            try
            {
                using (var textReader = File.OpenText(path))
                {
                    var csv = new CsvReader(textReader);
                    csv.Configuration.RegisterClassMap<Co2ReadingMap>();
                    return csv.GetRecords<Co2Reading>().ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An exception occurred while reading the data from file '{path}'. {ex}");
                return new List<Co2Reading>{ Co2Reading.CreateError("Unexpected error") };
            }
        }
    }
}