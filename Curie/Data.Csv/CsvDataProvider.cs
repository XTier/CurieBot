using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Data.Core.Entities;
using Data.Csv.Mappings;
using Serilog;

namespace Data.Csv
{
    public abstract class CsvDataProvider
    {
        protected readonly DataFilePathAssembler FilePathAssembler;

        protected CsvDataProvider(string rootFolder)
        {
            FilePathAssembler = new DataFilePathAssembler(rootFolder);
        }

        protected Co2Reading RetrieveValue()
        {
            var path = FilePathAssembler.GetPath();
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