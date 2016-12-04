namespace database
{
using System;
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
            instance.ActuallyChangeImage(); // Get first image.
        }

        /// <summary>
        /// Handle the player input phrase. This will update dad's understanding of any words involved and return the next thing for dad to say.
        /// </summary>
        /// <param name="sentenceType">The sentence template used for this submission. Use SentenceType strings.</param>
        /// <param name="inputWords">The words that the player filled in. Separated by strings, not literal words. (E.g. "video games" is one string).</param>
        /// <param name="nextDadPhrase">Out param. The next thing for dad to say will be assigned here regardless of return value.</param>
        public bool SubmitPlayerInput(string sentenceType, List<string> inputWords, out string nextDadPhrase)
        {
            Game.Instance.myScorer.EvaluatePlayerPhrase(sentenceType, inputWords, Game.Instance.outstandingNotUnderstoodWords, Game.Instance.currentImage.linkedWords);
            if (Game.Instance.myScorer.CheckForImageComplete() || sentenceType.Equals(SentenceType.NeverMindKeepGoing, StringComparison.OrdinalIgnoreCase)) {
                nextDadPhrase = this.AdvanceImage();
                return true;
            } else {
                nextDadPhrase = this.GetOutputForCurrentState();
            }
            return false;
        }

        /// <summary>
        /// Get the thing that dad should say now. Not called when the image is changing.
        /// </summary>
        /// <returns>Output string.</returns>
        public string GetOutputForCurrentState()
        {
            // Dad is hung up on an unknown word you used to describe something.
            if (Game.Instance.myScorer.AdditionalUnknownWord != null) {
                return string.Format("{0} what's {1}?", random.Random.Bool() ? "But" : "Wait,", Game.Instance.myScorer.AdditionalUnknownWord);
            }
            // Dad might understand in part, though he's not confused about a specific thing.
            else if (Game.Instance.currentImage.linkedWords.Count > Game.Instance.outstandingNotUnderstoodWords.Count &&
                Game.Instance.outstandingNotUnderstoodWords.Count > 0)
            {
                return random.Random.Bool() ? "I still don't understand the picture..." : "I'm starting to get this picture?...";
            }
            return "ERROR ERROR ERROR I AM A ROBOT";
        }

        /// <summary>
        /// Advances the image as far as the database things are concerned. The UI might hang onto the existing image for longer
        /// until it's ready to display the new one.
        /// <returns>The thing for dad to say regarding the previous image.</returns>
        /// </summary>
        private string AdvanceImage()
        {
            string dadOutput =
                Game.Instance.outstandingNotUnderstoodWords.Count > 1 ? "Well, if you say so. I don't get that one, though..." :
                Game.Instance.outstandingNotUnderstoodWords.Count > 0 ? "Okay, that's fine, I guess." :
                                                                        "Okay, I think I get it now.";
            // Check this here in case the image is advanced before dad understands.
            Game.Instance.myScorer.CheckForImageEndBonusOrPenalty();

            this.ActuallyChangeImage();

            Game.Instance.myScorer.ResetForNewImage();
            return dadOutput;
        }
        private void ActuallyChangeImage()
        {
            Game.Instance.currentImage = Database.Instance.GetUnusedImage();
            Game.Instance.outstandingNotUnderstoodWords.Clear();
            Game.Instance.outstandingNotUnderstoodWords.AddRange(Game.Instance.currentImage.linkedWords.Select(word => Database.Instance.GetOrCreateWord(word.word))); // GetOrCreate is so that words that dad is assumed to understand will just get added automatically.
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
