using System.Linq;
using Hocon;

namespace ObjectModel;

public static class HoconExtensions {
    extension(HoconValue value)
    {
        public object GetNumber()
        {
            if (value.First() is HoconLong)
            {
                return long.Parse(value.Raw);
            }
            else if (value.First() is HoconDouble)
            {
                return double.Parse(value.Raw);
            }

            throw new System.InvalidOperationException("Field is not a number.");
        }

        public bool GetBoolean()
        {
            return bool.Parse(value.Raw);
        }
    }
}