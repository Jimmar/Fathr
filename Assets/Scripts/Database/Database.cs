namespace database
{
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

    /// <summary>
    /// The main word and image database. Handles loading in words and constructing the links between them.
    /// Use this whenever you need to get a list of words related to a word or image.
    /// </summary>
    public class Database
    {
        private const string wordFilesResourcesPath_c = "Xml/Words";
        private const string imageFilesResourcesPath_c = "Xml/Images";
        private static readonly string imageFilesActualPath_c = Path.Combine(Application.dataPath, "Resources/" + imageFilesResourcesPath_c);

        private Dictionary<string, Word> wordDatabase;   // Keys are the words.
        private List<Image> imageDatabase;

        #region Member classes
        [XmlRoot("BaseWord")]
        public class Word
        {
            [XmlAttribute("word")]
            public string word { get; private set; }
            [XmlArray("LinkedWords")]
            [XmlArrayItem("Link")]
            public List<LinkedWord> linkedWords { get; private set; } // Doesn't prevent modification of list contents.
            [XmlArray("Descriptors")]
            [XmlArrayItem("Link")]
            public List<LinkedWord> linkedDescriptors { get; private set; }
            [XmlElement("UnderstandingBase")]
            public double understandingBase { get; private set; } // TODO: Default should be 1.
            [XmlElement("UnderstoodByDadType")]
            public short understoodByDadType { get; private set; }

            public Word() { } // Needed for serialization.
        }

        [XmlRoot("Image")]
        public class Image
        {
            [XmlAttribute("fileName")]
            public string fileName { get; private set; }
            [XmlArray("LinkedWords")]
            [XmlArrayItem("Link")]
            public List<LinkedWord> linkedWords { get; private set; }
            [XmlArray("Descriptors")]
            [XmlArrayItem("Link")]
            public List<LinkedWord> linkedDescriptors { get; private set; }
            [XmlIgnore]
            public bool hasBeenUsedAlready = false;
        }

        [XmlRoot("Link")]
        public class LinkedWord
        {
            [XmlAttribute("word")]
            public string word { get; private set; }
            [XmlAttribute("weight")]
            public double weight { get; private set; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Call during regular game loading to prepare the words. This will also set the singleton instance.
        /// </summary>
        public void Initialize()
        {
            instance = this;
            this.wordDatabase = new Dictionary<string, Word>();
            this.LoadXmlWords(Path.Combine(Application.dataPath, "Resources/" + wordFilesResourcesPath_c));
            this.LoadXmlImages(Path.Combine(Application.dataPath, "Resources/" + imageFilesResourcesPath_c));
        }

        /// <summary>
        /// Loads in all word XML files.
        /// </summary>
        /// <param name="containingDirectoryPath">Absolute path, using Application.dataPath.</param>
        private void LoadXmlWords(string containingDirectoryPath)
        {
            DirectoryInfo containingDirectoryInfo = new DirectoryInfo(containingDirectoryPath);
            FileInfo[] xmlFiles = containingDirectoryInfo.GetFiles("*.xml");
            foreach (FileInfo xmlFile in xmlFiles)
            {
                IList<Word> wordsFromFile = database.XmlLoader.LoadSubXml<Word>(xmlFile.FullName, "BaseWord");
                for (int i = 0; i < wordsFromFile.Count; i++) {
                    this.wordDatabase.Add(wordsFromFile[i].word, wordsFromFile[i]);
                }
            }
        }

        /// <summary>
        /// Loads in all image XML files. Mostly a copy-paste of LoadXmlWords.
        /// </summary>
        /// <param name="containingDirectoryPath">Absolute path, using Application.dataPath.</param
        private void LoadXmlImages(string containingDirectoryPath)
        {
            DirectoryInfo containingDirectoryInfo = new DirectoryInfo(containingDirectoryPath);
            FileInfo[] xmlFiles = containingDirectoryInfo.GetFiles("*.xml");
            foreach (FileInfo xmlFile in xmlFiles)
            {
                IList<Image> imagesFromFile = database.XmlLoader.LoadSubXml<Image>(xmlFile.FullName, "BaseImage");
                for (int i = 0; i < imagesFromFile.Count; i++) {
                    this.imageDatabase.Add(imagesFromFile[i]);
                }
            }
        }
        #endregion

        #region Word retrieval
        /// <summary>
        /// Retrieves, unweighted, the words (including descriptors) that are linked to the specified targetWord. Use for setting the word bank, etc.
        /// </summary>
        public IEnumerable<string> GetWordsLinkedTo(string targetWord)
        {
            if (!this.wordDatabase.ContainsKey(targetWord)) {
                throw new System.ArgumentException(string.Format("No word database entry for '{0}'!", targetWord));
            }
            return this.wordDatabase[targetWord].linkedWords.Select(word => word.word).Concat(
                this.wordDatabase[targetWord].linkedDescriptors.Select(word => word.word));
        }

        public Texture2D GetUnusedImage()
        {
            int timeout = 50;
            while (timeout > 0)
            {
                Image image = this.imageDatabase[Random.Range(0, this.imageDatabase.Count)];
                if (!image.hasBeenUsedAlready)
                {
                    image.hasBeenUsedAlready = true;
                    return Resources.Load<Texture2D>(Path.Combine(imageFilesActualPath_c, image.fileName));
                }
                timeout--;
            }
            throw new System.InvalidOperationException("Could not find any unused images!");
        }
        #endregion

        #region Singleton management
        private static Database instance;
        public static Database Instance
        {
            get { return instance; }
        }
        #endregion
    }
}
