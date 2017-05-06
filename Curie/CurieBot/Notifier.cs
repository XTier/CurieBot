using System;
using System.Threading;
using System.Threading.Tasks;
using CurieBot.Settings.Interfaces;
using DataHandling.Core.Entities;
using DataHandling.Core.Interfaces;
using Serilog;

namespace CurieBot
{
    // TODO refactor
    public class Notifier : IDataProvider
    {
        private readonly TimeSpan _updateTime;
        private readonly TimeSpan _delayOnErrorTime;
        private readonly IDataContextProvider _dataContext;
        private readonly ManualResetEvent _event = new ManualResetEvent(false);

        public Notifier(IDataContextProvider dataContext, ISlackSettings settings)
        {
            _dataContext = dataContext;
            _updateTime = settings.NotifyPeriod;
            _delayOnErrorTime = settings.DelayOnErrorTime;
        }

        private void Work()
        {
            while (_event.WaitOne())
            {
                Co2Reading lastReading;
                bool readSuccess;
                do
                {
                    lastReading = RetrieveValue();
                    readSuccess = lastReading?.IsSuccess ?? false;
                    if (!readSuccess)
                    {
                        Log.Warning("CO2 Readings are not present or outdated.");
                        Task.Delay(_delayOnErrorTime).Wait();
                    }
                } while (!readSuccess);

                NewReadout?.Invoke(lastReading);
                Task.Delay(_updateTime).Wait();
            }
        }

        private Co2Reading RetrieveValue()
        {
            return _dataContext.CurrentData;
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