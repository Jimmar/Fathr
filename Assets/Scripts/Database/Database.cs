namespace database
{
    using System;
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

        private string wordFilesResourcesPath;
        private string imageFilesResourcesPath;
        private string imageFilesActualPath { get { return Path.Combine(Application.dataPath, "Resources/" + imageFilesResourcesPath_c); } }

        private Dictionary<string, Word> wordDatabase; // Keys are the words.
        private List<Image> imageDatabase;
        private List<string> fakeWordDatabase; // Usable for all words.

        #region Member classes
        [XmlRoot("BaseWord")]
        public class Word : IWantXMLDeserializationCallback
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
            public double understanding { get; set; }
            [XmlElement("UnderstoodByDadType")]
            public double understoodByDadType { get; private set; }
            /// <summary>Tracks understanding as it's changed for the current image, and then this is assigned to understanding.</summary>
            public double understandingCurrent { get; set; }

            public Word() { } // Needed for deserialization.
            public Word(string word) { // Try to not use.
                this.word = word;
                this.linkedWords = new List<LinkedWord>();
                this.linkedDescriptors = new List<LinkedWord>();
            }

            public void OnDeserialization(object sender) { // Nothing is deserialized in before construction, so initialize here.
                this.understandingCurrent = this.understanding;
            }
        }

        [XmlRoot("Image")]
        public class Image
        {
            [XmlAttribute("fileName")]
            public string fileName { get; private set; }
            [XmlIgnore]
            public string resourcePath { get; set; } // Set manually.
            [XmlArray("LinkedWords")]
            [XmlArrayItem("Link")]
            public List<LinkedWord> linkedWords { get; private set; }
            [XmlArray("Descriptors")]
            [XmlArrayItem("Link")]
            public List<LinkedWord> linkedDescriptors { get; private set; }
            [XmlIgnore]
            public bool hasBeenUsedAlready = false;
        }
        // NOTE: Splitting this up into using Word, LinkedWord, and string was a bad idea.
        [XmlRoot("Link")]
        public class LinkedWord
        {
            [XmlAttribute("word")]
            public string word { get; private set; }
            [XmlAttribute("weight")]
            public double weight { get; private set; }
            [XmlIgnore]
            public bool isDescriptorAssociated { get; set; }

            public LinkedWord() { } // For deserialization.

            public LinkedWord(string word, double weight) {
                this.word = word;
                this.weight = weight;
                this.isDescriptorAssociated = false;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Use Initialize instead.
        /// </summary>
        private Database() { } 

        /// <summary>
        /// Call during regular game loading to prepare the words. This will also set the singleton instance.
        /// </summary>
        /// <param name="wordDirPath"> Directory of the word files, relative to Resources. Set to null to use the default..</param>
        /// <param name="imageDirPath">Directory of the image files, relative to Resources. Set to null to use the default..</param>
        public static void Initialize(string wordDirPath = null, string imageDirPath = null)
        {
            instance = new Database();
            instance.wordDatabase = new Dictionary<string, Word>();
            instance.imageDatabase = new List<Image>();
            instance.fakeWordDatabase = new List<string>();
            instance.wordFilesResourcesPath = wordDirPath ?? wordFilesResourcesPath_c;
            instance.imageFilesResourcesPath = imageDirPath ?? imageFilesResourcesPath_c;

            instance.LoadXmlWords(Path.Combine(Application.dataPath, "Resources/" + instance.wordFilesResourcesPath));
            instance.LoadXmlImages(Path.Combine(Application.dataPath, "Resources/" + instance.imageFilesResourcesPath));
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
                    if (xmlFile.Name.Contains("_FakeWords")) {
                        this.fakeWordDatabase.AddRange(wordsFromFile[i].linkedWords.Select(word => word.word));
                        this.fakeWordDatabase.AddRange(wordsFromFile[i].linkedDescriptors.Select(word => word.word));
                    } else {
                        this.wordDatabase.Add(wordsFromFile[i].word, wordsFromFile[i]);
                    }
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
                IList<Image> imagesFromFile = database.XmlLoader.LoadSubXml<Image>(xmlFile.FullName, "Image");
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
        public IEnumerable<string> GetWordsLinkedTo(string targetWord, int numFakeWords)
        {
            if (!this.wordDatabase.ContainsKey(targetWord)) {
                throw new System.ArgumentException(string.Format("No word database entry for '{0}'!", targetWord));
            }
            this.fakeWordDatabase.Shuffle();
            return this.wordDatabase[targetWord].linkedWords.Select(word => word.word).Concat(
                this.wordDatabase[targetWord].linkedDescriptors.Select(word => word.word)).Concat(
                this.fakeWordDatabase.GetRange(0, numFakeWords));
        }

        /// <summary>
        /// Gets the info about a specified word.
        /// </summary>
        public Word GetWord(string targetWord)
        {
            if (!this.wordDatabase.ContainsKey(targetWord)) {
                throw new System.ArgumentException(string.Format("No word database entry for '{0}'!", targetWord));
            }
            return this.wordDatabase[targetWord];
        }
        public bool TryGetWord(string targetWord, out Word word)
        {
            try {
                word = this.GetWord(targetWord);
                return true;
            } catch {
                word = null;
                return false;
            }
        }
        public Word GetOrCreateWord(string targetWord)
        {
            try {
                return this.GetWord(targetWord);
            } catch {
                Word word = new Word(targetWord);
                word.understanding = 1;
                word.understandingCurrent = 1;
                this.wordDatabase.Add(targetWord, word);
                return word;
            }
        }

        /// <summary>
        /// Load in an unused image file. Will timeout if no unused images are found.
        /// </summary>
        public Image GetUnusedImage()
        {
            int timeout = 50;
            while (timeout > 0)
            {
                Image image = this.imageDatabase[random.Random.Range(0, this.imageDatabase.Count)];
                if (!image.hasBeenUsedAlready)
                {
                    image.hasBeenUsedAlready = true;
                    image.resourcePath = Path.Combine(this.imageFilesResourcesPath, image.fileName);
                    return image;
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
