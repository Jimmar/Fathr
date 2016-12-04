﻿namespace database
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
            // So many ToList calls! This isn't a looker, this here code.
            if        (Game.Instance.currentImage.linkedWords.Select(word => word.word).ToList().ScrambledEquals(Game.Instance.outstandingNotUnderstoodWords.ToList())) {
                // You have not gotten dad to understand any of the words.
                return "Okay, go on...";
            } else {
                Database.Word nextNotUnderstoodWord = Game.Instance.myScorer.WordsToUpdate.Where(word => word.understandingCurrent < Game.Instance.currentImage.linkedWords.Where(linkedWord => linkedWord.Equals(word)).First().weight).Last();
                return string.Format("But what is {0}?", nextNotUnderstoodWord.word);
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
            Game.Instance.outstandingNotUnderstoodWords.AddRange(Game.Instance.currentImage.linkedWords.Select(word => word.word));
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
