using System;

namespace IMDBEnricher.Algorithms
{
    public class SimpleYearDistance : IYearDistanceAlgorithm
    {
        /// <inheritdoc/>
        public int CompareYears(int first, int second)
        {
            return Math.Abs(first - second);
        }
    }
}