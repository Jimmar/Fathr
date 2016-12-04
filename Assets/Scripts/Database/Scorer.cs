namespace database
{
using System;
using System.Collections.Generic;
using System.Linq;
using LinkedWord = Database.LinkedWord;
using Word = Database.Word;

    /// <summary>
    /// Adjusts dad knowledge based on the player's input.
    /// </summary>
    public class Scorer
    {
        private List<Word> wordstoUpdate = new List<Word>(); // Track which words need to have their understanding values updated.
        // Used to track not understood words that are used to describe other words. Pseudo-stack; when considering new words, checked in reverse order via this property.
        // LinkedWords since they'll need to be assigned weights based on the related word.
        private List<LinkedWord> additionalUnknownWords = new List<LinkedWord>();
        // Right now, purely for output.
        public string AdditionalUnknownWord { get { return this.additionalUnknownWords.Count > 0 ? this.additionalUnknownWords.Last().word : null; } }
        
        public void ResetForNewImage()
        {
            foreach (Word word in this.wordstoUpdate) {
                word.understanding = System.Math.Max(word.understanding, word.understandingCurrent);
            }
            this.wordstoUpdate.Clear();
            this.additionalUnknownWords.Clear();
        }

        [Flags]
        public enum ScoreState
        {

        }

        /// <summary>
        /// Updates the understood words based on the input phrase. Check the Word objects in the database for their UnderstandingBase.
        /// </summary>
        /// <param name="sentenceType">The type of sentence that the player selected. Use SentenceType members.</param>
        /// <param name="inputWords">The words used by the player, divided by the Words and not literal words. (E.g. "video games" is one Word.)</param>
        /// <param name="currentNotUnderstoodWords">All words active on the current image that dad doesn't understand. These are the words to be updated.</param>
        /// <param name="imageWords">The Word objects associated with the current image.</param>
        public void EvaluatePlayerPhrase(string sentenceType, List<string> inputWords, List<Word> currentNotUnderstoodWords, List<Database.LinkedWord> imageWords)
        {
            // workarounds since currentNotUnderstoodWords is outstandingNotUnderstoodWords.
            int originalCount = currentNotUnderstoodWords.Count; // Technically going to modify the collection when adding to outstandingNotUnderstoodWords.
            List<Word> originalNotUnderstoodWords = currentNotUnderstoodWords.GetRange(0, originalCount);
            // Compare each word against the input words, updating the current understanding while maintaining the base so that values aren't prematurely updated.
            for (int w = 0; w < originalCount; w++)
            {
                Word currentWord = originalNotUnderstoodWords[w];
                currentWord.understandingCurrent = Math.Max(currentWord.understandingCurrent,
                    this.EvaluateForWord(inputWords, currentWord));
                
                if (!this.wordstoUpdate.Contains(currentWord)) {
                    this.wordstoUpdate.Add(currentWord);
                }

                // Lots of redundancies here, but...
                // If a word was used that itself isn't fully understood, remember to follow up on it.
                foreach (string inputWord in inputWords)
                {
                    Word inputWordObj;
                    Database.Instance.TryGetWord(inputWord, out inputWordObj);
                    if (inputWordObj != null && inputWordObj.understandingCurrent < 1 &&
                        ( this.additionalUnknownWords.Count == 0 || !this.additionalUnknownWords.Select(word => word.word).Contains(currentWord.word) ) &&
                        currentWord.linkedWords.Where(word => word.word.Equals(inputWord)).Count() > 0) // Just get the weight from the earliest element that uses the unknown word, assuming they're in descending order in the XML file.
                    {
                        // Add the new word to the image, adding a weight based on its relationship to the word that begat it, unless dad already knows it.
                        double requiredWeight = 
                            currentWord.linkedWords.Where(word => word.word.Equals(inputWord)).First().weight * 
                            imageWords.Where(word => word.word.Equals(currentWord.word)).First().weight;
                        if (inputWordObj.understandingCurrent < requiredWeight)
                        {
                            this.additionalUnknownWords.Add(new LinkedWord(inputWord, requiredWeight));
                            Game.Instance.outstandingNotUnderstoodWords.Add(inputWordObj);
                        }
                    }
                }
            }
            // After updating all not understood words, reevaluate previous words and update their current understanding if needed.
            List<string> inputWithAdditional = inputWords.Union(this.additionalUnknownWords.Select(word => word.word)).ToList(); // Union prevents duplicates.
            for (int i = 0; i < originalNotUnderstoodWords.Count; i++) {
                Word currentWord = originalNotUnderstoodWords[i];
                currentWord.understandingCurrent = Math.Max(currentWord.understandingCurrent, this.EvaluateForWord(inputWithAdditional, currentWord));
            }
            // Update the global confusion/understanding based on the sentence type and the difference from the target understandings to the current ones.
            for (int i = 0; i < originalNotUnderstoodWords.Count; i++)
            {
                Word currentWord = originalNotUnderstoodWords[i];
                // Just gonna make recursive words not contribute here, since they'll add to confusion later.
                if (imageWords.Where(word => word.word.Equals(currentWord.word)).Count() > 0) {
                    double delta = currentWord.understandingCurrent - imageWords.Where(word => word.word.Equals(currentWord.word)).First().weight;
                    this.DetermineConfunderstansionChangeForWord(delta, currentWord.understandingCurrent, sentenceType);
                }
            }
            // Update the not understood words list to remove words that are now understood.
            for (int i = 0; i < currentNotUnderstoodWords.Count; i++)
            {
                Database.LinkedWord correspondingImageWord = imageWords.Where(word => word.word.Equals(currentNotUnderstoodWords[i].word)).FirstOrDefault();
                correspondingImageWord = correspondingImageWord ?? this.additionalUnknownWords.Where(word => word.word.Equals(currentNotUnderstoodWords[i].word)).FirstOrDefault();
                if (correspondingImageWord != null)
                {
                    Word currentWord = currentNotUnderstoodWords[i];
                    if (currentWord == null ||
                        currentWord.understandingCurrent >= correspondingImageWord.weight - float.Epsilon)
                    {
                        currentNotUnderstoodWords.RemoveAt(i); // This stays as current so we remove from the original set.
                        i--;
                        // Ugh.
                        for (int j = 0; j < this.additionalUnknownWords.Count; j++) {
                            if (this.additionalUnknownWords[j].word.Equals(currentWord.word)) {
                                this.additionalUnknownWords.RemoveAt(j);
                                j--;
                            }
                        }
                    }
                }
            }
        }

        private void DetermineConfunderstansionChangeForWord(double deltaCurrentToGoalUnderstanding, double currentUnderstanding, string sentenceType)
        {
            // Just putting this in for tests because game jam!
            if (Game.Instance == null) {
                return;
            }

            // Note that multi-word responses naturally weigh a little more towards understanding since dad's per-word understanding will (can) increase faster.
            double multiplier =
                sentenceType.Equals(SentenceType.RememberBlank) ? 0.3 :
                sentenceType.Equals(SentenceType.ItsBlank) || sentenceType.Equals(SentenceType.ItsJustBlank) || sentenceType.Equals(SentenceType.ItsSimilarToBlank) ? 0.6 :
                sentenceType.Equals(SentenceType.IfYouMixTheseThree) || sentenceType.Equals(SentenceType.ItsThreeBlanks) ? 1.2 :
                1.0;
            if (deltaCurrentToGoalUnderstanding >= 0.0) { // This will trigger once when the word is understood, since it's then removed from the notUnderstoodList.
                Game.Instance.Understanding += (float)(multiplier * 2.0); // 2 is arbitrary for now, high enough to work towards game end.
                Game.Instance.Confusion -= (float)(multiplier * 2.0);
            } else if (deltaCurrentToGoalUnderstanding >= -0.2) {
                Game.Instance.Confusion += (float)(multiplier * 0.5);
            } else if (currentUnderstanding < 0.2) {
                Game.Instance.Understanding -= (float)(multiplier * 1.0);
                Game.Instance.Confusion += (float)(multiplier * 3.0);
            } else {
                Game.Instance.Understanding -= (float)(multiplier * 1.0);
                Game.Instance.Confusion += (float)(multiplier * 1.0);
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
                    // However, do allow this to update if in the additional words list.
                    double understandingBase = (inputWord != null ? inputWord.understanding : 1);
                    double understanding = this.additionalUnknownWords.Count > 0 && this.additionalUnknownWords.Select(word => word.word).Contains(linkedWord.word) ? inputWord.understandingCurrent : understandingBase; // Seems dicey, but if LinkedWord were a struct that might help here.
                    sum += linkedWord.weight * (inputWord != null ? understanding : 1);
                    wordCount++; // Increase whether or not the input word, which is in the database, is related to the not understood word.
                }
            }
            return Math.Min(
                1, 
                (wordCount > 0 ? sum / wordCount : 0) + currentNotUnderstoodWord.understanding);
        }

        /// <summary>
        /// Without updating the score or understandings, simply determine if the current image has been adequately explained.
        /// </summary>
        /// <returns></returns>
        public bool CheckForImageComplete()
        {
            // TODO: Check for adjectives.
            return Game.Instance.outstandingNotUnderstoodWords == null || Game.Instance.outstandingNotUnderstoodWords.Count == 0; // Shouldn't be null here, but...
        }

        /// <summary>
        /// To be called when changing images, this will impose a penalty if the player didn't adequately explain the image.
        /// Do this before actually changing the images so the scorer can query the game state.
        /// </summary>
        public void CheckForImageEndBonusOrPenalty()
        {
            if (Game.Instance.outstandingNotUnderstoodWords.Count == 0) {
                Game.Instance.Understanding += 3.0f;
            } else {
                for (int i = 0; i < Game.Instance.outstandingNotUnderstoodWords.Count; i++) {
                    Game.Instance.Confusion += 1.0f - (float)Game.Instance.outstandingNotUnderstoodWords[i].understandingCurrent;
                }
            }
        }
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
