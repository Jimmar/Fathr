namespace database.test
{
using NUnit.Framework;
using System.Collections.Generic;

    /// <summary>
    /// Tests for the Scorer.
    /// </summary>
    [TestFixture]
    public class ScorerTests
    {
        private Scorer scorer;

        [TestFixtureSetUp]
        public void Initialize()
        {
            Database.Initialize("Xml/TestWords", "Xml/TestImages"); // This sets the singleton instance.
            this.scorer = new Scorer();
        }

        [Test]
        public void FullScenario()
        {
            // GOAL: The image has tags "Sonic" and "Overwatch," neither of which dad understands.
            Database.Image image = Database.Instance.GetUnusedImage(); // Assumes there's only one.
            this.scorer.ResetForNewImage();
            List<string> unknownWords = new List<string>(new string[] { "Sonic", "Overwatch", "romance" });

            List<string> first  = new List<string>(new string[] { "It's", "Sonic", "with", "Overwatch", "." });
            List<string> second = new List<string>(new string[] { "It's like", "Mario", "but", "blue", "." });
            List<string> third  = new List<string>(new string[] { "It's a", "video game", "with", "guns", "and", "teamwork", "." });

            // Initial state.
            Assert.AreEqual(0.0, Database.Instance.GetWord("Sonic").understanding,     "Sonic initial understanding");
            Assert.AreEqual(0.5, Database.Instance.GetWord("Mario").understanding,     "Mario initial understanding");
            Assert.AreEqual(0.0, Database.Instance.GetWord("Overwatch").understanding, "Overwatch initial understanding");
            Assert.AreEqual(1.0, Database.Instance.GetWord("sex").understanding,       "Sex initial understanding");
            Assert.AreEqual(0.0, Database.Instance.GetWord("Sonic").understandingCurrent,     "Sonic initial understanding current");
            Assert.AreEqual(0.5, Database.Instance.GetWord("Mario").understandingCurrent,     "Mario initial understanding current");
            Assert.AreEqual(0.0, Database.Instance.GetWord("Overwatch").understandingCurrent, "Overwatch initial understanding current");
            Assert.AreEqual(1.0, Database.Instance.GetWord("sex").understandingCurrent,       "Sex initial understanding current");

            // First string: Dad doesn't know those things.
            this.scorer.EvaluatePlayerPhrase(SentenceType.ItsBlankWithBlank, first, unknownWords, image.linkedWords);
            Assert.AreEqual(0.0, Database.Instance.GetWord("Sonic").understandingCurrent,     "Sonic understanding current after first");
            Assert.AreEqual(0.5, Database.Instance.GetWord("Mario").understandingCurrent,     "Mario understanding current after first");
            Assert.AreEqual(0.0, Database.Instance.GetWord("Overwatch").understandingCurrent, "Overwatch understanding current after first");
            Assert.AreEqual(1.0, Database.Instance.GetWord("sex").understandingCurrent,       "Sex understanding current after first");
            Assert.AreEqual(2, unknownWords.Count,                                            "Dad still doesn't understand anything new");

            // Second string: Oh, dad kind of knows Mario (0.5) and definitely knows red (1.0), so now he knows Sonic well enough.
            this.scorer.EvaluatePlayerPhrase(SentenceType.ItsLikeBlankButBlank, second, unknownWords, image.linkedWords);
            Assert.AreEqual(0.7, Database.Instance.GetWord("Sonic").understandingCurrent,     "Sonic understanding current after second"); // Sonic-Mario is 0.8, Sonic-blue is 1.0, so factor in the understandings: 0.8*0.5+1.0*1.0 / 2
            Assert.AreEqual(0.5, Database.Instance.GetWord("Mario").understandingCurrent,     "Mario understanding current after second");
            Assert.AreEqual(0.0, Database.Instance.GetWord("Overwatch").understandingCurrent, "Overwatch understanding current after second");
            Assert.AreEqual(1.0, Database.Instance.GetWord("sex").understandingCurrent,       "Sex understanding current after second");
            Assert.AreEqual(1, unknownWords.Count,                                            "Dad understands Sonic well enough now");
            Assert.AreEqual("Overwatch", unknownWords[0],                                     "Dad still doesn't know Overwatch, though");

            // Third string: Dad kind of knows video games (0.5) and definitely knows guns and teamwork (both 1.0), so now he knows Overwatch well enough.
            this.scorer.EvaluatePlayerPhrase(SentenceType.ItsThreeBlanks, third, unknownWords, image.linkedWords);
            Assert.AreEqual(0.7, Database.Instance.GetWord("Sonic").understandingCurrent,       "Sonic understanding current after third");
            Assert.AreEqual(0.5, Database.Instance.GetWord("Mario").understandingCurrent,       "Mario understanding current after third");
            Assert.AreEqual(2.5/3, Database.Instance.GetWord("Overwatch").understandingCurrent, "Overwatch understanding current after third"); // 0.5*1+1*1+1*1 / 3
            Assert.AreEqual(1.0, Database.Instance.GetWord("sex").understandingCurrent,         "Sex understanding current after third");
            Assert.AreEqual(0, unknownWords.Count,                                              "Dad understands Overwatch well enough now");

            // Now the adjective!

        }
    }
}
