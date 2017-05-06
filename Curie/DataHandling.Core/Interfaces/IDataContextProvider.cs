using DataHandling.Core.Entities;

namespace DataHandling.Core.Interfaces
{
    public interface IDataContextProvider
    {
        Co2Reading CurrentData { get; }
    }
}