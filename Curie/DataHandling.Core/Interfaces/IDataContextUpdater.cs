using DataHandling.Core.Entities;

namespace DataHandling.Core.Interfaces
{
    public interface IDataContextUpdater
    {
        void UpdateData(Co2Reading reading);
    }
}