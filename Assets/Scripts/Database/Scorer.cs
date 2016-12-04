namespace database
{
using System;
using System.Collections.Generic;
using System.Linq;
using Word = Database.Word;

    /// <summary>
    /// Adjusts dad knowledge based on the player's input.
    /// </summary>
    public class Scorer
    {
        private delegate void PreviousEvaluation(); // Should bake in the lists.
        private List<PreviousEvaluation> previousEvaluations = new List<PreviousEvaluation>();
        private Stack<Word> wordstoUpdate = new Stack<Word>(); // Stack so that we can retrieve not understood words in order.

        public void Initialize()
        {
            instance = this;
        }

        public void ResetForNewImage()
        {
            while (this.wordstoUpdate.Count > 0) {
                Word word = this.wordstoUpdate.Pop();
                word.understanding = System.Math.Max(word.understanding, word.understandingCurrent);
            }
            this.previousEvaluations.Clear();
            this.wordstoUpdate.Clear();
        }

        /// <summary>
        /// Updates the understood words based on the input phrase. Check the Word objects in the database for their UnderstandingBase.
        /// </summary>
        /// <param name="inputWords">The words used by the player, divided by the Words and not literal words. (E.g. "video games" is one Word.)</param>
        /// <param name="currentNotUnderstoodWords">All words active on the current image that dad doesn't understand. These are the words to be updated.</param>
        /// <param name="imageWords">The Word objects associated with the current image.</param>
        public void EvaluatePlayerPhrase(List<string> inputWords, List<string> currentNotUnderstoodWords, List<Database.LinkedWord> imageWords)
        {
            // Compare each word against the input words, updating the current understanding while maintaining the base so that values aren't prematurely updated.
            foreach (string currentNotUnderstoodStr in currentNotUnderstoodWords)
            {
                Word currentWord;
                if (!Database.Instance.TryGetWord(currentNotUnderstoodStr, out currentWord)) { // Dad already understands this, probably.
                    continue;
                }
                // TODO: Handle "connector" words here.
                currentWord.understandingCurrent = Math.Max(currentWord.understandingCurrent,
                    this.EvaluateForWord(inputWords, currentWord));
                
                if (!this.wordstoUpdate.Contains(currentWord)) {
                    this.wordstoUpdate.Push(currentWord);
                }
            }
            // After updating all not understood words, reevaluate previous words and update their current understanding if needed.
            foreach (PreviousEvaluation eval in this.previousEvaluations) {
                eval();
            }
            // Add the word evaluation call to the list of previous calls so we can re-call it as understandings get updated.
            // This will add duplicates, but that's okay. Also there's some redundant work. Oops.
            foreach (string currentNotUnderstoodStr in currentNotUnderstoodWords)
            {
                Word currentWord;
                if (!Database.Instance.TryGetWord(currentNotUnderstoodStr, out currentWord)) { // Dad already understands this, probably.
                    continue;
                }
                this.previousEvaluations.Add(() => { this.EvaluateForWord(inputWords, currentWord); });
            }
            // Update the not understood words list to remove words that are now understood.
            for (int i = 0; i < currentNotUnderstoodWords.Count; i++)
            {
                Database.LinkedWord correspondingImageWord = imageWords.Where(word => word.word.Equals(currentNotUnderstoodWords[i])).FirstOrDefault();
                if (correspondingImageWord != null)
                {
                    Word currentWord;
                    Database.Instance.TryGetWord(currentNotUnderstoodWords[i], out currentWord);
                    if (currentWord == null ||
                        currentWord.understandingCurrent > correspondingImageWord.weight)
                    {
                        currentNotUnderstoodWords.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Evaluate an individual word. Called from EvaluatePlayerPhrase.
        /// </summary>
        /// <returns>The updated understanding value. Assign this to understandingCurrent.</returns>
        private double EvaluateForWord(List<string> inputWords, Word currentNotUnderstoodWord)
        {
            int wordCount = 0; // Some inputWords might not be included, so track separately.
            double sum = 0;
            foreach (string inputStr in inputWords) // Redundant, but just looping through them all each time for now.
            {
                Word inputWord;
                Database.LinkedWord linkedWord = currentNotUnderstoodWord.linkedWords.Where(word => word.word.Equals(inputStr, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                Database.Instance.TryGetWord(inputStr, out inputWord);
                // If the linkedWord doesn't have a database entry, assume that dad understands it.
                if (linkedWord != null)
                { // Don't use current so that understanding of just this word doesn't increase until the image is done. Otherwise, redoing the same function call will keep adding to understanding.
                    sum += linkedWord.weight * (inputWord != null ? inputWord.understanding : 1);
                    wordCount++; // Increase whether or not the input word, which is in the database, is related to the not understood word.
                }
            }
            return Math.Min(1, (wordCount > 0 ? sum / wordCount : 0) + currentNotUnderstoodWord.understanding);
        }

        #region Singleton management
        private static Scorer instance;
        public static Scorer Instance
        {
            get { return instance; }
        }
        #endregion
    }
}


/* NOTES ON WHAT TO DO:
 * - Images have linked words and weights. When dad understands those words at at least the weights' levels, it's understood.
 * - If the player uses words to describe a target word, as long as no words are below some minimum dad knowledge, combine their weights to the target word to determine the score.
 * --- E.g. Sonic - "It's a VIDEO GAME that's FAST." Dad understands VIDEO GAME at 0.6 and FAST at 1.0. Those words are both 1.0-weighted to Sonic, so Dad now understands Sonic at 0.6.
 * - If dad doesn't understand a used word sufficiently well, he will prompt for explanation. If he then understands it, the understanding cascades to previous statements.
 * - If dad understands all words associated with an image but no descriptors, he will prompt for that.
 * --- If a descriptor is given that the image contains, that weight will factor into the score.
 * --- E.g. Dad knows Sonic is VIDEO GAME and FAST, but he asks "But why do people like this?" referring to Sonic slash porn. 
 *     The player responds, "Because it's sexy." The image has SEXY weighted at 0.5, so dad is content with this answer.
 * --- This might cause dad to learn new descriptors. E.g. now Sonic is SEXY at 0.5.
 * --- At the end of the game, this will determine dad's general statement about Tumblr, based on the most used descriptor. "Well, Tumblr sure is SEXY. I think I get it now."
 */
