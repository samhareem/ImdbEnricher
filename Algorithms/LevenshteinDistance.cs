using System;

namespace IMDBEnricher.Algorithms
{
    public class LevenshteinDistance : IStringDistanceAlgorithm
    {
        /// <inheritdoc/>
        public int CalculateDistance(string first, string second)
        {
            //Trim strings
            first = first.Trim();
            second = second.Trim();
            
            //If either of the strings is empty, return the length of the other string
            if (first.Length == 0 || second.Length == 0)
            {
                return Math.Max(first.Length, second.Length);
            }
            
            //Initialize the 2-dimensional matrix used to calculate the Levenshtein distance between the two strings
            //This creates a 2 dimensional matrix with a top left cell containing 0 and a running number on the first
            //row and column 
            var matrix = new int[first.Length + 1, second.Length + 1];
            for (var column = 0; column <= first.Length; matrix[column, 0] = column++) { }
            for (var row = 0; row <= second.Length; matrix[0, row] = row++) { }

            //Traverse the matrix row by row, filling in values according to the algorithm
            for (var row = 1; row <= second.Length; row++)
            {
                for (var column = 1; column <= first.Length; column++)
                {
                    var currentCost = first[column - 1] == second[row - 1] ? 0 : 1;
                    
                    //Calculate minimum cost for current cell
                    var minimumCost = Math.Min(
                        Math.Min(matrix[column - 1, row] + 1, matrix[column, row - 1] + 1),
                        matrix[column - 1, row - 1] + currentCost);
                    
                    //Else set current cell to minimumCost and continue traversal
                    matrix[column, row] = minimumCost;
                }
            }

            //Once the whole matrix has been traversed and filled in, the lower right cell will contain the distance
            //between the strings
            return matrix[first.Length, second.Length];
        }
    }
}