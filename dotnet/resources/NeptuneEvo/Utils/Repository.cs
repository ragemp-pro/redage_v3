using Newtonsoft.Json;

namespace NeptuneEvo.Utils
{
    public static class Repository
    {
        public static T Clone<T>(this T obj)
        {
            var clonedJson = JsonConvert.SerializeObject (obj);

            return JsonConvert.DeserializeObject<T> (clonedJson);
        }
    }
}