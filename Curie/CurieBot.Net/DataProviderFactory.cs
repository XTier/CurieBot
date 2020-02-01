using CurieBot.Net.Enums;
using CurieBot.Net.Interfaces;
using Data.Core.Exceptions;
using Data.Core.Interfaces;
using Data.Csv;
using Data.Csv.Settings;
using Data.Hid.Core.Settings;

namespace CurieBot.Net
{
    public class DataProviderFactory : IDataProviderFactory
    {
        private readonly CsvManageSettings _csvManageSettings;
        private readonly HidManageSettings _hidManageSettings;

        public DataProviderFactory(CsvManageSettings csvManageSettings, HidManageSettings hidManageSettings)
        {
            _csvManageSettings = csvManageSettings;
            _hidManageSettings = hidManageSettings;
        }

        public IDataProvider CreateDataProvider(DataProviderType type)
        {
            switch (type)
            {
                case DataProviderType.LogFile:
                    return new TimedCsvDataProvider(_csvManageSettings);
                case DataProviderType.Device:
                    return new HidDataProvider(_hidManageSettings);
                default:
                    throw new CustomException($"A data provider is not defined for type '{type}'");
            }
        }
    }
}