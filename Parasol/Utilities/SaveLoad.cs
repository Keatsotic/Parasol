using System.IO;
using System.Xml.Serialization;

namespace Parasol
{
	public static class SaveLoad
	{
		public enum SerializeModes { Binary, XML };

		public static class XmlSerialization
		{

			public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
			{
				TextWriter writer = null;
				try
				{
					var serializer = new XmlSerializer(typeof(T));
					writer = new StreamWriter(filePath, append);
					serializer.Serialize(writer, objectToWrite);
				}
				finally
				{
					if (writer != null)
						writer.Close();
				}
			}

			public static T ReadFromXmlFile<T>(string filePath) where T : new()
			{
				TextReader reader = null;
				try
				{
					var serializer = new XmlSerializer(typeof(T));
					reader = new StreamReader(filePath);
					return (T)serializer.Deserialize(reader);
				}
				finally
				{
					if (reader != null)
						reader.Close();
				}
			}
		}
	}
}