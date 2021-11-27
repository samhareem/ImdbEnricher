using System;
using System.Linq;
using IMDBEnricher.Models;

namespace IMDBEnricher.Algorithms
{
    public class Comparer : IComparer
    {
        private IStringDistanceAlgorithm _stringComparer;
        private IYearDistanceAlgorithm _yearComparer;

        public Comparer(IStringDistanceAlgorithm stringDistanceAlgorithm, IYearDistanceAlgorithm yearDistanceAlgorithm)
        {
            _stringComparer = stringDistanceAlgorithm;
            _yearComparer = yearDistanceAlgorithm;
        }
        
        public int Compare(SearchTitle searchTitle, Title imdbTitle)
        {
            //Initial score
            var score = 0;
            
            //If a title is provided, we compare it with both the primary IMDB title and the original title and choose the best fit
            if (!string.IsNullOrWhiteSpace(searchTitle.Title))
            {
                score += Math.Min(_stringComparer.CalculateDistance(searchTitle.Title, imdbTitle.OriginalName),
                        _stringComparer.CalculateDistance(searchTitle.Title, imdbTitle.PrimaryName));
            }
            
            //If a director is provided and the serach title has one, we compare it with each director for the title and choose the best fit
            if (!string.IsNullOrWhiteSpace(searchTitle.Director))
            {
                if (imdbTitle.Directors.Any())
                {
                    var minScore = Int32.MaxValue;
                    foreach (var director in imdbTitle.Directors)
                    {
                        minScore = Math.Min(minScore,
                            _stringComparer.CalculateDistance(searchTitle.Director, director.PrimaryName));
                    }

                    score += minScore;
                }
                else
                {
                    //If no director is provided on IMDB, we assume the worst so as not to hinder titles with a director
                    score += searchTitle.Director.Length;
                }
            }
            
            //If a year is provided and the IMDB title has one, compare them
            if (searchTitle.Year.HasValue)
            {
                if (imdbTitle.StartYear.HasValue)
                {
                    score += _yearComparer.CompareYears(searchTitle.Year.Value, imdbTitle.StartYear.Value);
                }
                else
                {
                    //TODO Make configurable?
                    //If no startyear is provided on IMDB, we assume relatively large distance of 10 years
                    score += 10;
                }
            }

            return score;
        }
    }
}