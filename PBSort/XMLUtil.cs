using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PBSort
{
    public class XMLUtil
    {
        public static string Serialize<T>(T srcObject, Type t, Type[] extratypes)
        {
            return SerializeJK(srcObject, t, extratypes);
        }

        public static string Serialize<T>(T srcObject)
        {
            return SerializeJK(srcObject, null, null);
        }

        public static string SerializeJK<T>(T srcObject, Type t, Type[] extratypes)
        {
            string srcObjectXml = string.Empty;

            MemoryStream stream = null;
            StreamReader reader = null;
            XmlSerializer x = null;
            try
            {

                if ((t == null) || (extratypes == null))
                    x = new XmlSerializer(srcObject.GetType());
                else
                    x = new XmlSerializer(t, extratypes);

                stream = new MemoryStream();
                XmlTextWriter xtw = new XmlTextWriter(stream, Encoding.UTF8);
                xtw.Formatting = Formatting.Indented;
                x.Serialize(xtw, srcObject);
                reader = new StreamReader(stream);
                stream.Seek(0, SeekOrigin.Begin);
                srcObjectXml = reader.ReadToEnd();
            }
            finally
            {

                if (null != stream)
                {
                    stream.Close();
                    stream.Dispose();
                }

                if (null != reader)
                {
                    reader.Close();
                    reader.Dispose();
                }

            }


            return srcObjectXml;
        }

        public static T Deserialize<T>(string xmlString)
        {
            StringReader sr = null; ;
            XmlTextReader xtr = null;
            T instance;

            try
            {
                sr = new StringReader(xmlString);
                xtr = new XmlTextReader(sr);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                instance = (T)xmlSerializer.Deserialize(xtr);

            }
            finally
            {
                if (xtr != null)
                    xtr.Close();

                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return instance;
        }
    }
}
