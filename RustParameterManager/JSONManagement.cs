using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLightParameterManager
{
    public class JSONManagement
    {
        public static Dictionary<string, object> dictionaryOfValues(JToken jobect)
        {
            return extractValues(jobect).ToDictionary(pair=>pair.Item1, pair=>pair.Item2);
        }

        public static List<(string, object)> extractValues(JToken jobect)
        {
            List<(string, object)> values = new List<(string, object)>();
            onJValuesOfJSON(jobect, jvalue => { values.Add((jvalue.Path, jvalue.Value)); });
            return values;
        }

        public static List<(string, string)> structureFromValues(List<(string, object)> newValues)
        {
            return newValues.Select((path_and_value) =>
            {
                string path = path_and_value.Item1;
                object value = path_and_value.Item2;
                return (path, value == null ? "null" : value.GetType().ToString());
            }).ToList();
        }

        public static List<JValue> getJValuesOf(JToken json)
        {
            List<JValue> values = new List<JValue>();
            onJValuesOfJSON(json, jvalue => values.Add(jvalue));
            return values;
        }

        public static void onJValuesOfJSON(JToken jtoken, Action<JValue> action)
        {
            bool isRoot = jtoken.Path.Equals("");
            if (jtoken is JValue jvalue)
            {
                action.Invoke(jvalue);
            }
            jtoken.Children().ToList().ForEach(child => { onJValuesOfJSON(child, action); });
        }

    }
}
