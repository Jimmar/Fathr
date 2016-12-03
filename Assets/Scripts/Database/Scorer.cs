using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Database
{
    /// <summary>
    /// Adjusts dad knowledge based on the player's input.
    /// </summary>
    class Scorer
    {
        /* NOTES ON WHAT TO DO:
         * - Images have linked words and weights. When dad understands those words at at least the weights' levels, it's understood.
         * - If the player uses words to describe a target word, as long as no words are below some minimum dad knowledge, combine their weights to the target word to determine the score.
         * --- E.g. Sonic - "It's a VIDEO GAME that's FAST." Dad understands VIDEO GAME at 0.6 and FAST at 1.0. Those words are both 1.0-weighted to Sonic, so Dad now understands Sonic at 0.6.
         * - If dad doesn't understand a used word sufficiently well, he will prompt for explanation. If he then understands it, the understanding cascades to previous statements.
         * - If dad understands all words associated with an image but no descriptors, he will prompt for that.
         * --- If a descriptor is given that the image contains, that weight will factor into the score.
         * --- E.g. Dad knows Sonic is VIDEO GAME and FAST, but he asks "But why do people like this?" referring to Sonic slash porn. The player responds, "Because it's sexy." The image has SEXY weighted at 0.5, so dad is content with this answer.
         * --- This might cause dad to learn new descriptors. E.g. now Sonic is SEXY at 0.5.
         * --- At the end of the game, this will determine dad's general statement about Tumblr, based on the most used descriptor. "Well, Tumblr sure is SEXY. I think I get it now."
         */
    }
}
