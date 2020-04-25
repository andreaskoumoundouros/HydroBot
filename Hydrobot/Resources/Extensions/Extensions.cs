using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Hydrobot.Resources.Extensions {
    public static class Extensions {
        public static async Task<byte[]> ReadAllBytes(this BinaryReader reader) {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream()) {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0) {
                    await ms.WriteAsync(buffer, 0, count);
                }
                return ms.ToArray();
            }
        }

        public static string ExtractKeyValue(JArray json, string key) {
            foreach (Newtonsoft.Json.Linq.JObject content in json.Children<JObject>()) {
                foreach (JProperty prop in content.Properties()) {
                    if (prop.Name == key) {
                        return (string)prop.Value;
                    }
                }
            }
            return "";
        }
    }
}
