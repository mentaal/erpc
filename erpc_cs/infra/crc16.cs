using System;

namespace erpc {

    public class CRC16 {

        public CRC16() { }
        public static UInt16 calc(byte[] data, int size) {
            UInt32 crc = 0x1d0f;
            UInt32 j;
            if (size > data.Length)
            {
                return 0;
            }
            for (j = 0; j < size; ++j)
            {
                UInt32 i;
                UInt32 b = data[j];
                crc ^= b << 8;
                for (i = 0; i < 8; ++i)
                {
                    UInt32 temp = crc << 1;
                    if ((crc & 0x8000) != 0)
                    {
                        temp ^= 0x1021;
                    }
                    crc = temp;
                }
            }

            return (UInt16)crc;
        }
    }
}
