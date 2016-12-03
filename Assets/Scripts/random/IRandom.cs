namespace random
{
    /// <summary>
    /// For use in place of direct calls to System.Random or UnityEngine.Random, so that random numbers can be swapped out for non-random ones for testing, etc.
    /// Methods have more verbose names than do those for accessing the singleton instance, since those calls should be concise.
    /// </summary>
    public interface IRandom
    {
        int RangeInt(int min, int max);
        float RangeFloat(float min, float max);
        double RangeDouble(double min, double max);
        void SetSeedTo(int seed);
    }
}
