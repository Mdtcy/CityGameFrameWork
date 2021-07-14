using System;

namespace CityBuilderCore
{
    [Serializable]
    public class StructureLevelMask
    {
        public int Value;

        public bool Check(int value) => Check(Value,value);

        public static bool Check(int a, int b) => a == 0 || b == 0 || (a & b) != 0;
    }
}
