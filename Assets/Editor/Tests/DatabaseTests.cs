namespace database.test
{
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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
            database.Initialize(); // This sets the singleton instance.
        }

        [Test]
        public void GetWords()
        {
            List<string> linkedWords = Database.Instance.GetWordsLinkedTo("Sonic").ToList();
            Assert.IsTrue(linkedWords.Contains("video game"), "Check for video game");
            Assert.IsTrue(linkedWords.Contains("hedgehog"), "Check for hedgehog");
            Assert.IsTrue(linkedWords.Contains("blue"), "Check for blue");
            Assert.IsTrue(linkedWords.Contains("chili dog"), "Check for chili dog");
            // There are more, but whatever.
            Assert.IsTrue(linkedWords.Contains("cool"), "Check for cool descriptor");
            Assert.IsTrue(linkedWords.Contains("funny"), "Check for funny descriptor");
        }
    }
}