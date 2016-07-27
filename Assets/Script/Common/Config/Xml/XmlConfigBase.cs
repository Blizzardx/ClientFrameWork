﻿using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Common.Config
{
    public abstract class XmlConfigBase
    {
        public static string Serialize<T>(T config) where T : XmlConfigBase, new()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (Stream outputStream = new MemoryStream(64))
            {
                XmlTextWriter a = new XmlTextWriter(outputStream, Encoding.UTF8);
                a.Formatting = Formatting.Indented;
                serializer.Serialize(a, config);
                byte[] bytes = new byte[outputStream.Length];
                outputStream.Position = 0;
                outputStream.Read(bytes, 0, bytes.Length);

                Encoding encoding = Encoding.UTF8;
                string res = encoding.GetString(bytes);
                return res;
            }
        }
        public static T DeSerialize<T>(string xmlData) where T : XmlConfigBase, new()
        {
            Encoding encoding = Encoding.UTF8;
            byte[] data = encoding.GetBytes(xmlData);
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            T sysConfig = null;
            using (MemoryStream stream = new MemoryStream(data))
            {
                sysConfig = serializer.Deserialize(stream) as T;
            }
            return sysConfig;
        }
    }
}