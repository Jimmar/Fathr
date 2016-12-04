namespace database
{
using System.Collections.Generic;
using System.Linq;

    /// <summary>
    /// The main point of contact between the front gameplay logic and the database/evaluation.
    /// After Initializing this, pass in the player's input to SubmitPlayerInput, and use GetOutputForCurrentState to
    /// retrieve the text that dad should say.
    /// </summary>
    public class DadResponder
    {
        /// <summary>
        /// Use Initialize instead.
        /// </summary>
        private DadResponder() { }

        /// <summary>
        /// Prepares the DadResponder.
        /// </summary>
        public static void Initialize() {
            instance = new DadResponder();
        }

        /// <summary>
        /// Handle the player input phrase. This will update dad's understanding of any words involved.
        /// Afterwards, use GetOutputForCurrentState to retrieve the next string.
        /// </summary>
        /// <param name="sentenceType">The sentence template used for this submission. Use SentenceType strings.</param>
        /// <param name="inputWords">The words that the player filled in. Separated by strings, not literal words. (E.g. "video games" is one string).</param>
        public void SubmitPlayerInput(string sentenceType, List<string> inputWords)
        {
            Game.Instance.myScorer.EvaluatePlayerPhrase(sentenceType, inputWords, Game.Instance.outstandingNotUnderstoodWords, Game.Instance.currentImage.linkedWords);

        }

        /// <summary>
        /// Get the thing that dad should say now,
        /// </summary>
        /// <returns>Output string.</returns>
        public string GetOutputForCurrentState()
        {
            if (Game.Instance.currentImage == null)
            {
                Game.Instance.currentImage = Database.Instance.GetUnusedImage();
                Game.Instance.outstandingNotUnderstoodWords.Clear();
                Game.Instance.outstandingNotUnderstoodWords.AddRange(Game.Instance.currentImage.linkedWords.Select(word => word.word));
                return "What is this, child?";
            }

            return "ERROR ERROR ERROR I AM A ROBOT";
        }

        #region Singleton management
        private static DadResponder instance;
        public static DadResponder Instance
        {
            get { return instance; }
        }
        #endregion

    }
}
