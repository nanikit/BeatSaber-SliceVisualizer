using System.Runtime.CompilerServices;
using IPA.Config.Data;
using IPA.Config.Stores;
using System.Linq;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SliceVisualizer.Configuration
{
    public class ColorConverter : ValueConverter<Color>
    {
        public ColorConverter() { }
        public override Color FromValue(Value value, object parent)
        {
            if (value is List list)
            {
                var array = list.Select(FloatConverter.ValueToFloat).ToArray();
                return new Color(array[0], array[1], array[2], array[3]);
            }
            else if (value is Text text)
            {
                var color = new Color(1f, 1f, 1f, 1f);
                ColorUtility.TryParseHtmlString(text.Value, out color);
                return color;
            }
            else
            {
                throw new System.ArgumentException("Color deserializer expectes either string or array");
            }
        }

        public override Value ToValue(Color obj, object parent)
        {
            Plugin.Log.Info(string.Format("trying to serialize color: {0}", obj));
            var array = new float[] { obj.r, obj.g, obj.b, obj.a };
            return Value.From(array.Select(x => Value.Float((decimal)x)));
        }
    }
}