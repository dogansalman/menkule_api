
using System.Collections.Generic;
using System.Web.Script.Serialization;


namespace rest_api.Libary.JsonHelper
{
    public static class JsonHelper
    {
        public static string DictionaryToJson(Dictionary<string, object> dictionaryData)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(dictionaryData);
        }
    }
}