namespace random
{
using UnityEngine;
    
    /// <summary>
    /// Generates random ints and floats.
    /// </summary>
    public class Random : random.IRandom
    {
        private System.Random rand;

        public Random() { 
            this.rand = new System.Random();
        }
        public Random(int seed) {
            this.SetSeedTo(seed);    
        }

        /// <summary>
        /// Returns a random integer on [min, max).
        /// </summary>
        public int RangeInt(int min, int max) {
            return this.rand.Next(min, max);
        }
        /// <summary>
        /// Returns a random float on [min, max).
        /// </summary>
        public float RangeFloat(float min, float max) {
            double d = this.rand.NextDouble();
            return (float)(d * (max - min) + min); // Can introduce error, but since doubles are more precise than floats it shouldn't matter.
        }
        /// <summary>
        /// Returns a random double on [min, max).
        /// </summary>
        public double RangeDouble(double min, double max) {
            double d = this.rand.NextDouble();
            return d * (max - min) + min;
        }

        /// <summary>
        /// Sets the seed for future random numbers. This can also be called during construction.
        /// </summary>
        public void SetSeedTo(int seed) {
            this.rand = new System.Random(seed);
        }


        #region Singleton management and static access
        /// <summary>
        /// Use the Random static members for any access to random numbers.
        /// I'm letting this be public so that I can change it as needed for tests, replays, etc.; because it should be easy to not use it inappropriately.
        /// </summary>
        public static random.IRandom Instance { get; set; }

        /// <summary>
        /// By default, the random generator will be initialized to give random numbers with the default seed, since this is the most likely use case.
        /// </summary>
        static Random() {
            Instance = new Random();
        }

        /// <summary>
        /// Returns a random integer on [min, max).
        /// </summary>
        public static int Range(int min, int max) {
            return Instance.RangeInt(min, max);
        }
        /// <summary>
        /// Returns a random float on [min, max).
        /// </summary>
        public static float Range(float min, float max) {
            return Instance.RangeFloat(min, max); // FUTURE[3/3/16]: I really don't need to care, but I could add in float.epsilon here? I don't think I need to since it's downcasting from a double...
        }
        /// <summary>
        /// Returns a random double on [min, max).
        /// </summary>
        public static double Range(double min, double max) {
            return Instance.RangeDouble(min, max);
        }
        /// <summary>
        /// Randomly returns true or false with equal weighting.
        /// </summary>
        public static bool Bool() {
            return Instance.RangeInt(0, 2) == 0;
        }
        /// <summary>
        /// Randomly returns -1 or 1 with equal weighting.
        /// </summary>
        public static int Sign() {
            return Bool() ? 1 : -1;
        }
        /// <summary>
        /// Randomly returns based on the the expected time interval. Use to do something randomly and approximately (or at least on average) every N seconds.
        /// Respects time dilation/pausing.
        /// </summary>
        /// <param name="expectedTiming">How often, on average, to return true.</param>
        public static bool RangeTimed(float expectedTiming) {
            return RangeTimed(expectedTiming, Time.deltaTime);
        }
        /// <summary>
        /// Randomly returns based on the the expected time interval. Use to do something randomly and approximately (or at least on average) every N seconds.
        /// Respects time dilation/pausing.
        /// </summary>
        /// <param name="expectedTiming">How often, on average, to return true.</param>
        /// <param name="deltaTime">If deltaTime is known, pass it in.</param> // NOTE[4/15/16]: deltaTime is not constant, so it can't be a default argument.
        public static bool RangeTimed(float expectedTiming, float deltaTime) {
            return Instance.RangeFloat(0.0f, expectedTiming) < deltaTime;
        }

        /// <summary>
        /// Randomly selects a Vector3 within the specified bounds. Assumes that upperBound's components are not lower than those of lowerBound.
        /// </summary>
        public static Vector3 RangeVector(Vector3 lowerBound, Vector3 upperBound)
        {
            return new Vector3(Instance.RangeFloat(lowerBound.x, upperBound.x), Instance.RangeFloat(lowerBound.y, upperBound.y), Instance.RangeFloat(lowerBound.z, upperBound.z));
        }

        /// <summary>
        /// Sets the seed for future random numbers.
        /// </summary>
        public static void SetSeed(int seed) {
            Instance.SetSeedTo(seed);
        }

        #endregion
    }

}