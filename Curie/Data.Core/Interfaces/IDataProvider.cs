using System;
using Data.Core.Entities;

namespace Data.Core.Interfaces
{
    public interface IDataProvider
    {
        void Init();
        void Stop();
        event Action<Co2Reading> NewReadout;
    }
}