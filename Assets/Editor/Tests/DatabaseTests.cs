namespace database.test
{
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Image = Database.Image;

    /// <summary>
    /// Unit tests...in a GAME JAM GAME??? Seems like the easiest way to make sure this works without having
    /// to actually set up the game first.
    /// </summary>
    [TestFixture]
    public class DatabaseTests
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            Database database = new Database();
            database.Initialize("Xml/TestWords", "Xml/TestImagesFake"); // This sets the singleton instance.
        }

        [Test]
        public void GetWords()
        {
            random.Random.Instance = new random.Random(5); // Seed arbitrary to choose the expected fake words.

            List<string> linkedWords = Database.Instance.GetWordsLinkedTo("Sonic", 3).ToList();
            Assert.IsTrue(linkedWords.Contains("video game"), "Check for video game");
            Assert.IsTrue(linkedWords.Contains("hedgehog"), "Check for hedgehog");
            Assert.IsTrue(linkedWords.Contains("blue"), "Check for blue");
            Assert.IsTrue(linkedWords.Contains("chili dog"), "Check for chili dog");
            // There are more, but whatever.
            Assert.IsTrue(linkedWords.Contains("cool"), "Check for cool descriptor");
            Assert.IsTrue(linkedWords.Contains("funny"), "Check for funny descriptor");
            // Check for FakeWords, too.
            Assert.IsTrue(linkedWords.Contains("sports"), "Check for fake sports");
            Assert.IsTrue(linkedWords.Contains("dog"), "Check for fake dog");
            Assert.IsTrue(linkedWords.Contains("beer"), "Check for fake beer");

            random.Random.Instance = new random.Random();
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(System.InvalidOperationException), ExpectedMessage = "Could not find any unused images!")]
        public void GetImages()
        {
            random.Random.Instance = new random.NotRandom(0,  0, 1,  0, 2,  0, 1, 2);

            Image image1 = Database.Instance.GetUnusedImage();
            Assert.AreEqual("1", image1.fileName, "Get the first test image.");
            Image image2 = Database.Instance.GetUnusedImage();
            Assert.AreEqual("2", image2.fileName, "Get the second image, avoiding the first image.");
            Image image3 = Database.Instance.GetUnusedImage();
            Assert.AreEqual("3", image3.fileName, "Get the third image, avoiding those that were selected already.");

            Image imageNotFound = Database.Instance.GetUnusedImage();

            random.Random.Instance = new random.Random();
        }
    }
}