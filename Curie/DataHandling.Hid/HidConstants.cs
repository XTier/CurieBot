namespace DataHandling.Hid
{
    internal class HidConstants
    {
        internal const byte Temperature = 0x42;
        internal const byte Co2 = 0x50;
        internal const byte ReadLoopCount = 9;
        internal static readonly byte[] Report = { 0x00, 0xa1, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };    // feature
        internal static readonly byte[] Key = { 0xa1, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };             // aka 'magic table'
        internal static readonly byte[] Cstate = { 0x48, 0x74, 0x65, 0x6D, 0x70, 0x39, 0x39, 0x65 };          // "Htemp99e"
        internal static readonly byte[] Shuffle = { 2, 4, 0, 7, 1, 6, 5, 3 };
    }
}