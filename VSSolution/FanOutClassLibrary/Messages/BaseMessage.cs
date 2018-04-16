using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FanOutClassLibrary.Messages
{
    public abstract class BaseMessage
    {
        public string Type { get; private set; }

        public BaseMessage()
        {
            string typeName = GetType().Name;
            typeName = typeName.Substring(0, typeName.Length - "Message".Length);
            Type = typeName;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static BaseMessage FromJson(string json)
        {
            JObject parsed = JObject.Parse(json);

            if (parsed.TryGetValue("Type", out JToken typeValue))
            {
                string typeName = typeValue.Value<string>() + "Message";

                var matchedType = typeof(BaseMessage).Assembly.GetTypes().FirstOrDefault(i => i.FullName == "FanOutClassLibrary.Messages." + typeName);
                if (matchedType != null)
                {
                    return (BaseMessage)JsonConvert.DeserializeObject(json, matchedType);
                }
            }

            return null;
        }
    }
}
