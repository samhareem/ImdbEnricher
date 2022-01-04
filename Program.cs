using System.IO;
using CommandLine;
using IMDBEnricher.Algorithms;
using IMDBEnricher.Readers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IMDBEnricher
{
    //Command verb for updating IMDB data
    [Verb("update", HelpText = "Update local IMDB data to correspond with given datasets.")]
    class UpdateOptions {
        [Option('d', "dataset-directory", Required = false, Default = null, HelpText = "Directory containing IMDB datasets if overriding config.")]
        public string? DatasetDirectory { get; set; }
        [Option('o', "output-file", Required = false, Default = null, HelpText = "Output-file for IMDB data if overriding config.")]
        public string? OutputFile { get; set; }
    }
    
    //Command verb for enriching IMDB data
    [Verb("enrich", HelpText = "Try and enrich titles in input file with IMDB data.")]
    class EnrichOptions {
        [Option('i', "input-file", Required = true, HelpText = "Input file path.")]
        public string InputFile { get; set; }
        [Option('o', "output-file", Required = true, HelpText = "Output file path.")]
        public string OutputFile { get; set; }
        [Option('d', "data-file", Required = false, HelpText = "Data file path if overriding configuration.")]
        public string? DataFile { get; set; }
        [Option("max-candidate-score", Required = false, Default = null, HelpText = "Maximum score for a title to be considered a candidate if overriding config.")]
        public int? MaxCandidateScore { get; set; }
        [Option("max-choice-candidate-score", Required = false, Default = null, HelpText = "Maximum score for a title to be considered a choice candidate if overriding config.")]
        public int? MaxChoiceCandidateScore { get; set; }
    }
    
    
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var enricher = serviceProvider.GetRequiredService<ImdbEnricher>();

            Parser.Default.ParseArguments<UpdateOptions, EnrichOptions>(args)
                .WithParsed<UpdateOptions>(options => enricher.UpdateImdbTitles(options.DatasetDirectory, options.OutputFile))
                .WithParsed<EnrichOptions>(options => enricher.EnrichImdbInformation(options.InputFile, options.OutputFile,
                    options.DataFile, options.MaxCandidateScore, options.MaxChoiceCandidateScore));
        }
        
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())                    
                .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information)                    
                .AddTransient<ImdbDataSetUpdater>();
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false)
                .Build());
            services.AddSingleton<IImdbDataSetUpdater, ImdbDataSetUpdater>();
            services.AddSingleton<IImdbTitleReader, ImdbTitleReader>();
            services.AddSingleton<IInputReader, InputReader>();
            
            services.AddSingleton<IStringDistanceAlgorithm, LevenshteinDistance>();
            services.AddSingleton<IYearDistanceAlgorithm, SimpleYearDistance>();
            services.AddSingleton<IComparer, Comparer>();

            services.AddSingleton<ImdbEnricher>();
        }
    }
}