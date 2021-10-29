namespace IMDBEnricher.Algorithms
{
    public interface IStringDistanceAlgorithm
    {
        /// <summary>
        /// Returns the distance between the given strings calculated using the algorithm implementation.
        /// </summary>
        /// <param name="first">First string to compare.</param>
        /// <param name="second">Second string to compare.</param>
        /// <returns>Integer signifying the distance between the two strings.</returns>
        int CalculateDistance(string first, string second);
    }
}