using Data.Core.Entities;

namespace Data.Core.Interfaces
{
    public interface IDataContextProvider
    {
        Co2Reading CurrentData { get; }
    }
}