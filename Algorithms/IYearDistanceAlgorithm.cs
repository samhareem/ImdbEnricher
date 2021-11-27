namespace IMDBEnricher.Algorithms
{
    public interface IYearDistanceAlgorithm
    {
        /// <summary>
        /// Returns the distance between the given years calculated using the algorithm implementation.
        /// </summary>
        /// <param name="first">First year to compare.</param>
        /// <param name="second">Second year to compare.</param>
        /// <returns>Integer signifying the distance between the two years.</returns>
        int CompareYears(int first, int second);
    }
}