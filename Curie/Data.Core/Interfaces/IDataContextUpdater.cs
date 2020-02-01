using Data.Core.Entities;

namespace Data.Core.Interfaces
{
    public interface IDataContextUpdater
    {
        void UpdateData(Co2Reading reading);
    }
}