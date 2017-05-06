using System;
using DataHandling.Core.Entities;

namespace DataHandling.Core.Interfaces
{
    public interface IDataProvider
    {
        void Init();
        void Stop();
        event Action<Co2Reading> NewReadout;
    }
}