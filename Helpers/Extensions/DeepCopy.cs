using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Extensions
{
    public static class ObjectExtension
    {
        // Deep clone
        public static void DeepCopy<T>(ref T object2Copy, ref T objectCopy)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(T));

                serializer.Serialize(stream, object2Copy);
                stream.Position = 0;
                objectCopy = (T)serializer.Deserialize(stream);
            }
        }
    }
}
