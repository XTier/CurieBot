using System;
using System.Threading;
using System.Threading.Tasks;
using Data.Core.Entities;
using Data.Core.Interfaces;

namespace Data.Core.Providers
{
    public class FakeDataProvider : IDataProvider
    {
        private readonly TimeSpan _period;
        private readonly Random _rng;
        private readonly ManualResetEvent _event = new ManualResetEvent(false);

        public FakeDataProvider(TimeSpan period)
        {
            _period = period;
            _rng = new Random();
        }

        public void Init()
        {
            // TODO BUG: multiple inits cause parallel working processes
            _event.Set();
            Task.Run(() => Work());
        }

        private void Work()
        {
            while (_event.WaitOne())
            {
                NewReadout?.Invoke(CreateValue());
                Task.Delay(_period).Wait();
            }
        }

        private Co2Reading CreateValue()
        {
            return Co2Reading.Create(DateTime.Now,
                                     _rng.Next(400, 2000),
                                     _rng.Next(15, 30) + _rng.NextDouble());
        }

        public void Stop()
        {
            _event.Reset();
        }

        public event Action<Co2Reading> NewReadout;
    }
}