using System;

namespace NoiseAmpControlApp
{
    public static class NoiseFilter
    {
        private static UInt16 _ch1Value;
        private static UInt16 _ch2Value;
        private static UInt16 _ch3Value;
        private static UInt16 _ch4Value;
        private static UInt16 cyc1 = 0;
        private static UInt16 cyc2 = 0;
        private static UInt16 cyc3 = 0;
        private static UInt16 cyc4 = 0;
        public static byte Vol1;

        public static void StartCalculation(byte[] sourceBytes)
        {
            _ch1Value = ConvertHextoInt(sourceBytes, 1);
            _ch2Value = ConvertHextoInt(sourceBytes, 5);
            _ch3Value = ConvertHextoInt(sourceBytes, 9);
            _ch4Value = ConvertHextoInt(sourceBytes, 13);

            Form1.Form.UpdateSerialConsole($"Ch1_value:{_ch1Value,0:D} Ch2_value:{_ch2Value,0:D} Ch3_value:{_ch3Value,0:D} Ch4_value:{_ch4Value,0:D}");

            VolumeController();
        }

        private static UInt16 ConvertHextoInt(byte[] sourceBytes, int toIndex)
        {
            int charValue = 0;
            byte[] charArray = new byte[4];

            for (byte i = 0; i < Constants.MaxChr; i++)
            {
                charArray[i] = sourceBytes[i + toIndex];

                if ((charArray[i] >= '0') && (charArray[i] <= '9'))
                {
                    charValue += ((charArray[i] - 0x30) * Convert.ToInt16(Math.Pow(16, 3 - i)));
                }
                else if ((charArray[i] >= 'A') && (charArray[i] <= 'F'))
                {
                    charValue += ((charArray[i] - 0x37) * Convert.ToInt16(Math.Pow(16, 3 - i)));
                }
            }

            return (UInt16)charValue;
        }

        private static void VolumeController()
        {
            byte vol2;
            byte vol3;
            byte vol4;
            UInt32 collector1 = 0;
            UInt32 collector2 = 0;
            UInt32 collector3 = 0;
            UInt32 collector4 = 0;

            if (cyc1 == Constants.Noise1Cycle)
            {
                collector1 /= cyc1;
                cyc1 = 0;
                collector1 /= 16;
                Vol1 = (byte)(64 - collector1);
            }
            else
            {
                collector1 += (UInt16)_ch1Value;
                cyc1++;
            }
            if (cyc2 == Constants.Noise2Cycle)
            {
                collector2 /= cyc2;
                cyc2 = 0;
                collector2 /= 16;
                vol2 = (byte)(64 - collector2);
            }
            else
            {
                collector2 += (UInt16)_ch2Value;
                cyc2++;
            }
            if (cyc3 == Constants.Noise3Cycle)
            {
                collector3 /= cyc3;
                cyc3 = 0;
                collector3 /= 16;
                vol3 = (byte)(64 - collector3);
            }
            else
            {
                collector3 += (UInt16)_ch3Value;
                cyc3++;
            }
            if (cyc4 == Constants.Noise4Cycle)
            {
                collector4 /= cyc4;
                cyc4 = 0;
                collector4 /= 16;
                vol4 = (byte)(64 - collector4);
            }
            else
            {
                collector4 += (UInt16)_ch4Value;
                cyc4++;
            }
        }

    }
}

