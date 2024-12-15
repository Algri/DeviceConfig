namespace IoT.RPiController.Services.Helpers
{
    public static class IoTHelper
    {
        public static byte UpdateByte(byte value, byte position, bool bitstate)
        {
            if (bitstate)
            {
                //left-shift 1, then bitwise OR
                return (byte)(value | (1 << position));
            }
            //left-shift 1, take compliment, then bitwise AND
            return (byte)(value & ~(1 << position));
        }

        public static int UpdateInt(int value, byte position, bool bitstate)
        {
            if (bitstate)
            {
                //left-shift 1, then bitwise OR
                return value | (1 << position);
            }
            //left-shift 1, take compliment, then bitwise AND
            return value & ~(1 << position);
        }

        public static bool CheckBit(byte value, byte position)
        {
            // internal method for reading the value of a single bit within a byte
            return (value & (1 << position)) != 0;
        }
    }
}
