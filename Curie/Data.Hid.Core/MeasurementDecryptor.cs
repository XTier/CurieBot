using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Hid.Core
{
    public class MeasurementDecryptor
    {
        private const byte Temperature = 0x42;
        private const byte Co2 = 0x50;
        private const byte ReadLoopCount = 9;
        private readonly byte[] _report = { 0x00, 0xa1, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };     // feature
        private readonly byte[] _key = { 0xa1, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };              // aka 'magic table'
        private readonly byte[] _cState = { 0x48, 0x74, 0x65, 0x6D, 0x70, 0x39, 0x39, 0x65 };           // "Htemp99e"
        private readonly byte[] _shuffle = { 2, 4, 0, 7, 1, 6, 5, 3 };

        public Measurement RetrieveMeasurement(byte[] deviceData)
        {
            if (deviceData is null)
                throw new ArgumentNullException(nameof(deviceData));

            if (deviceData.Length != 9)
                return Measurement.Failed();

            var data = deviceData.Skip(1).ToArray();

            var decrypted = Decrypt(data);
            var parsed = Parse(decrypted);

            return parsed;
        }

        private byte[] Decrypt(byte[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            var phase1 = new byte[8];
            for (byte i = 0; i < 8; i++)
            {
                byte j = _shuffle[i];
                phase1[j] = data[i];
            }

            var phase2 = new byte[8];
            for (byte i = 0; i < 8; i++)
            {
                phase2[i] = (byte)(phase1[i] ^ _key[i]);
            }

            var phase3 = new byte[8];
            for (byte i = 0; i < 8; i++)
            {
                phase3[i] = (byte)(((phase2[i] >> 3) | (phase2[(i - 1 + 8) % 8] << 5)) & 0xff);
            }

            var ctmp = new byte[8];
            for (byte i = 0; i < 8; i++)
            {
                ctmp[i] = (byte)(((_cState[i] >> 4) | (_cState[i] << 4)) & 0xff);
            }

            var result = new byte[8];
            for (byte i = 0; i < 8; i++)
            {
                result[i] = (byte)((0x100 + phase3[i] - ctmp[i]) & 0xff);
            }

            return result;
        }

        private static Measurement Parse(IReadOnlyList<byte> decrypted)
        {
            var isChecksumOk = decrypted[4] == 0x0d && (byte)decrypted.Take(3).Sum(_ => _) == decrypted[3];

            if (isChecksumOk)
            {
                byte op = decrypted[0];
                int value = decrypted[1] << 8 | decrypted[2];
                switch (op)
                {
                    case Co2:
                        return Measurement.WithCo2(value);
                    case Temperature:
                        return Measurement.WithTemperature(value / 16.0 - 273.15);
                    default:
                        return Measurement.Failed();
                }
            }

            return Measurement.Failed();
        }
    }
}