using System;
using Data.Core.Entities;
using Data.Core.Interfaces;
using Data.Hid.Core.Settings;

namespace CurieBot.Net
{
    // TODO AV: fake temporary class, remove
    public class HidDataProvider : IDataProvider
    {
        private readonly HidManageSettings _hidManageSettings;

        public HidDataProvider(HidManageSettings hidManageSettings)
        {
            _hidManageSettings = hidManageSettings;
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public event Action<Co2Reading> NewReadout;
    }
}