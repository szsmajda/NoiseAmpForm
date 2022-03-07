using System;

namespace NoiseAmpControlApp
{
    public static class NoiseFilter
    {
        private static UInt32 collector1 = 0;
        private static UInt32 collector2 = 0;
        private static UInt32 collector3 = 0;
        private static UInt32 collector4 = 0;
        private static UInt16 _ch1Value;
        private static UInt16 _ch2Value;
        private static UInt16 _ch3Value;
        private static UInt16 _ch4Value;
        private static UInt16 cyc1 = 0;
        private static UInt16 cyc2 = 0;
        private static UInt16 cyc3 = 0;
        private static UInt16 cyc4 = 0;
        private static UInt16 _noise1Avg;
        private static UInt16 _noise2Avg;
        private static UInt16 _noise3Avg;
        private static UInt16 _noise4Avg;
        public static byte Ch1Volume;
        public static byte Ch2Volume;
        public static byte Ch3Volume;
        public static byte Ch4Volume;

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
            if (cyc1 == Constants.Noise1Cycle)
            {
                collector1 /= cyc1;
                _noise1Avg = (UInt16)collector1;
                cyc1 = 0;
                collector1 = 0;
                if(_noise1Avg < Constants.MinimumNoiseValue)
                {
                    Ch1Volume = Constants.MinimumVolumeValue;
                }
                else if(_noise1Avg > Constants.MaximumNoiseValue)
                {
                    Ch1Volume = Constants.MaximumVolumeValue;
                }
                else
                {
                    Ch1Volume = Convert.ToByte(Constants.MinimumVolumeValue - (_noise1Avg / 20));
                }
                //collector1 /= 20;
                //if (collector1 < 10) { collector1 = 10; }
                //Ch1Volume = (byte)(63 - collector1);
                
            }
            else
            {
                collector1 += (UInt16)_ch1Value;
                cyc1++;
            }
            if (cyc2 == Constants.Noise2Cycle)
            {
                collector2 /= cyc2;
                _noise2Avg = (UInt16)collector2;
                cyc2 = 0;
                collector2 = 0;
                if (_noise2Avg < Constants.MinimumNoiseValue)
                {
                    Ch2Volume = Constants.MinimumVolumeValue;
                }
                else if (_noise2Avg > Constants.MaximumNoiseValue)
                {
                    Ch2Volume = Constants.MaximumVolumeValue;
                }
                else
                {
                    Ch2Volume = Convert.ToByte(Constants.MinimumVolumeValue - (_noise2Avg / 20));
                }
            }
            else
            {
                collector2 += (UInt16)_ch2Value;
                cyc2++;
            }
            if (cyc3 == Constants.Noise3Cycle)
            {
                collector3 /= cyc3;
                _noise3Avg = (UInt16)collector3;
                cyc3 = 0;
                collector3 = 0;
                if (_noise3Avg < Constants.MinimumNoiseValue)
                {
                    Ch3Volume = Constants.MinimumVolumeValue;
                }
                else if (_noise3Avg > Constants.MaximumNoiseValue)
                {
                    Ch3Volume = Constants.MaximumVolumeValue;
                }
                else
                {
                    Ch3Volume = Convert.ToByte(Constants.MinimumVolumeValue - (_noise3Avg / 20));
                }
            }
            else
            {
                collector3 += (UInt16)_ch3Value;
                cyc3++;
            }
            if (cyc4 == Constants.Noise4Cycle)
            {
                collector4 /= cyc4;
                _noise4Avg = (UInt16)collector4;
                cyc4 = 0;
                collector4 = 0;
                if (_noise4Avg < Constants.MinimumNoiseValue)
                {
                    Ch4Volume = Constants.MinimumVolumeValue;
                }
                else if (_noise4Avg > Constants.MaximumNoiseValue)
                {
                    Ch4Volume = Constants.MaximumVolumeValue;
                }
                else
                {
                    Ch4Volume = Convert.ToByte(Constants.MinimumVolumeValue - (_noise4Avg / 20));
                }
            }
            else
            {
                collector4 += (UInt16)_ch4Value;
                cyc4++;
            }
        }

    }
}

