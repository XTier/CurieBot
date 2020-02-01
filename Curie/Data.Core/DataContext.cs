using Data.Core.Entities;
using Data.Core.Interfaces;

namespace Data.Core
{
    public class DataContext : IDataContextProvider, IDataContextUpdater
    {
        public void UpdateData(Co2Reading reading)
        {
            if (reading == null || !reading.IsSuccess)
                return;

            CurrentData = reading;
        }

        public Co2Reading CurrentData { get; private set; }
    }
}