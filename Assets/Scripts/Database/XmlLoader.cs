namespace database
{
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;

    /// <summary>
    /// Abstracts XML loading and saving. Use to deserialize structs that have been serialized into XML.
    /// Used instead of TextAsset since I need actual file I/O, not just reading in data (although this is also used in places where parsing TextAssets into XML would suffice).
    /// </summary>
    public static class XmlLoader
    {
        /// <summary>
        /// Outputs a serializable object to the specified file.
        /// </summary>
        /// <typeparam name="T">A struct that can be serialized into XML.</typeparam>
        /// <param name="xmlPath">Full file output path.</param>
        /// <param name="serializableObject">Object to output.</param>
        public static void SaveXml<T>(string xmlPath, T serializableObject)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fileStream = new FileStream(xmlPath, FileMode.Create)) {
                serializer.Serialize(fileStream, serializableObject);
            }
        }

        /// <summary>
        /// Load a serializable object from XML.
        /// </summary>
        /// <typeparam name="T">A struct that can be deserialized from XML.</typeparam>
        /// <param name="xmlPath">Full XML file path.</param>
        /// <returns>The loaded object, or null if not present.</returns>
        public static T LoadXml<T>(string xmlPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fileStream = new FileStream(xmlPath, FileMode.Open)) {
                T deserialized = (T) serializer.Deserialize(fileStream);
                return deserialized;
            }
        }

        /// <summary>
        /// Given an XML file, loads a subset of the entire XML based on some target node name and deserializes it.
        /// Loaded in order of increasing depth, and then by order in the file.
        /// </summary>
        /// <typeparam name="T">A struct that can be deserialized from XML.</typeparam>
        /// <param name="xmlPath">Full path to the XML file.</param>
        /// <param name="elementNameToLimitTo">Name of the node(s) to load. Note that this should be unambiguous with all nodes and attributes.</param>
        /// <returns>List of any deserialized XML elements.</returns>
        public static IList<T> LoadSubXml<T>(string xmlPath, string elementNameToLimitTo)
        {
            XElement root = XElement.Load(xmlPath);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            List<XElement> limitedElements = new List<XElement>();
            List<T> loadedElements = new List<T>();
            Queue<XElement> elementsToCheck = new Queue<XElement>();
            elementsToCheck.Enqueue(root);

            while (elementsToCheck.Count > 0)
            {
                XElement currentElement = elementsToCheck.Dequeue();
                if (string.Compare(currentElement.Name.LocalName, elementNameToLimitTo) == 0) {
                    limitedElements.Add(currentElement);
                    continue;
                }
                foreach(XElement element in currentElement.Elements()) {
                    elementsToCheck.Enqueue(element);
                }
            }

            foreach (XElement element in limitedElements)
            {
                T deserialized = (T) serializer.Deserialize(element.CreateReader());
                loadedElements.Add(deserialized);
            }
            return loadedElements;
        }
    }
}