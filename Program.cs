using System.IO;
using IMDBEnricher.Algorithms;
using IMDBEnricher.Models;
using IMDBEnricher.Readers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IMDBEnricher
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var enricher = serviceProvider.GetRequiredService<ImdbEnricher>(); 
            //enricher.UpdateImdbTitles();
            enricher.EnrichImdbInformation("C:/Dump/ImdbEnricherInput.csv", "C:/Dump/ImdbEnricherOutput.csv");
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