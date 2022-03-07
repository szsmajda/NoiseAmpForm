namespace NoiseAmpControlApp
{
    public enum SendTypes
    {
        SpeakOut,
        NoiseMeasure,
        KeepAlive,
    }

    enum buff_stats : byte
    {
        OL1 = 1,
        OL2 = 2,
        OL3 = 3,
        UL1 = 4,
        UL2 = 5,
        UL3 = 6,
        OK = 0
    };
}
