namespace random
{
using System;
using System.Collections.Generic;

    /// <summary>
    /// An implementation of IRandom that does not return random numbers. For testing and potentially for replays.
    /// FUTURE[1/20/16]: For replays, this currently does nothing to ensure that values are requested in the same order, but that's an external problem? I.e., determinism is currently not guaranteed with multiple clients.
    ///                  I could have another implementation that maintains a dictionary of objects to random values for replaying.
    /// </summary>
    public class NotRandom : IRandom
    {
        private Queue<double> rangeResults;

        /// <summary>
        /// Instead of generating random numbers, will return the one or more valuesToReturn in order. When only one value remains, returns that value indefinitely.
        /// valuesToReturn should be doubles, but when ints are requested they will be rounded.
        /// </summary>
        /// <param name="valuesToReturn">Must specify at least one.</param>
        public NotRandom(params double[] valuesToReturn) {
            if (valuesToReturn.Length < 1) {
                throw new ArgumentException("NotRandom requires at least one valueToReturn during construction.");
            }
            this.rangeResults = new Queue<double>(valuesToReturn);
        }

        /// <summary>
        /// Does not bounds check, instead assuming that the valuesToReturn during construction were correct.
        /// </summary>
        public int RangeInt(int min, int max) {
            double r = this.rangeResults.Count == 1 ? this.rangeResults.Peek() : this.rangeResults.Dequeue(); // Will throw if count == 0, which should be caught during construction.
            return (int)(Math.Round(r)); // Note that Round() will round towards even numbers given xxx.5.
        }
        /// <summary>
        /// Does not bounds check, instead assuming that the valuesToReturn during construction were correct.
        /// </summary>
        public float RangeFloat(float min, float max) {
            return (float)(this.rangeResults.Count == 1 ? this.rangeResults.Peek() : this.rangeResults.Dequeue());
        }
        /// <summary>
        /// Does not bounds check, instead assuming that the valuesToReturn during construction were correct.
        /// </summary>
        public double RangeDouble(double min, double max) {
            return this.rangeResults.Count == 1 ? this.rangeResults.Peek() : this.rangeResults.Dequeue();
        }

        /// <summary>
        /// No-op, since no random numbers are generated. Doesn't throw so that any substitutions of NotRandoms for Randoms will
        /// not have unhandled exceptions under normal behavior.
        /// </summary>
        public void SetSeedTo(int seed) { }
    }
}
