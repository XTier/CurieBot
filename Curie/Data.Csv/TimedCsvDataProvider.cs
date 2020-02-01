using System;
using System.Threading;
using System.Threading.Tasks;
using Data.Core.Entities;
using Data.Core.Interfaces;
using Data.Csv.Settings;
using Serilog;

namespace Data.Csv
{
    public class TimedCsvDataProvider : CsvDataProvider, IDataProvider
    {
        private readonly TimeSpan _updateTime;
        private readonly TimeSpan _dataBreakTime;
        private readonly ManualResetEvent _event = new ManualResetEvent(false);

        public TimedCsvDataProvider(string rootFolder, TimeSpan updateTime, TimeSpan dataBreakTime) : base(rootFolder)
        {
            _updateTime = updateTime;
            _dataBreakTime = dataBreakTime;
        }

        public TimedCsvDataProvider(CsvManageSettings settings) : base(settings.ZgRootFolder)
        {
            _updateTime = settings.UpdatePeriod;
            _dataBreakTime = settings.DataBreakTime;
        }

        private void Work()
        {
            while (_event.WaitOne())
            {
                var lastReading = RetrieveValue();
                if (lastReading.IsSuccess && DateTime.Now - lastReading.Time > _dataBreakTime)
                {
                    Log.Warning("CO2 Readings are outdated.");
                    lastReading = Co2Reading.CreateError("No data");
                }

                NewReadout?.Invoke(lastReading);
                Task.Delay(_updateTime).Wait();
            }
        }

        public void Init()
        {
            // TODO BUG: multiple inits cause parallel working processes
            _event.Set();
            Task.Run(() => Work());
        }

        public void Stop()
        {
            _event.Reset();
        }

        public event Action<Co2Reading> NewReadout;
    }
}