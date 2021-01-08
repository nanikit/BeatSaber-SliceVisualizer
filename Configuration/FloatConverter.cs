using IPA.Config.Data;

namespace SliceVisualizer.Configuration
{
    public class FloatConverter
    {
        public static float ValueToFloat(Value val)
        {
            return val switch
            {
                FloatingPoint point => (float) point.Value,
                Integer integer => integer.Value,
                _ => throw new System.ArgumentException("List element was not a number"),
            };
        }
    }
}
