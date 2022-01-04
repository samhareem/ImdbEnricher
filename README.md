# ImdbEnricher

A simple command line tool for attempting to get IMDB data for uncertain movie titles based on title name, release year and director. Uses fuzzy logic to compare the given information to that of IMDB and outputs a list of candidates or a single candidate if a close match is found.

The purpose of this tool is to allow the matching of inaccurate movie information, such as a badly remembered title, to an IMDB entry. It can also be used to fetch additional information, such as genre, for a list of titles. The motivation for the tool came from the [movie database](https://hiff.fi/historia/) for the Helsinki International Film Festival. While the listing is a great tool for anyone interested in the history of the festival, there are some issues with the data quality. The way titles are presented is inconsistent, sometimes the cells contain irrelevant information or typos and there are some general mistakes in the information. The list also doesn't include the genre of the title. Using a precursor of this tool, I managed to match most of the titles in the list to their IMDB counterpart, which allowed me to quickly fix any mistakes in the title information and add new information to the titles.

## Example

For example, let's say I want to find more information for a movie that came out in 2000 called Adélie. Searching IMDB for this movie returns no results. I must be misremembering something! Without knowing who the director is, it would be difficult to find the information just by browsing IMDB. Instead, I can try searching for the movie using this tool. Since the tool uses fuzzy logic when matching titles, it will show the 2001 movie Amélie by Jean-Pierre Jeunet as a very close match to the information provided, allowing me to quickly check that it is indeed the movie I was looking for.

## Quickstart

First download the necessary [IMDB Datasets](https://www.imdb.com/interfaces/). The following datasets are required by the program:

* name.basics.tsv
* title.crew.tsv
* title.ratings.tsv
* title.akas.tsv
* title.basics.tsv

Once you've downloaded the datasets, unpack their contents into a directory of your choosing.

Next, configure the program. Default settings can be saved in the config.json file and these can be overwritten for a single run using flags. The values that can be configured are:

* datasetDirectory: The path of the directory containing your IMDB datasets.
* imdbTitleFilePath: Path to file containing title information parsed from the datasets. The contents of this file are used when matching input to IMDB titles.
* inputFileDelimiter: Delimiter used in the input file.
* maximumCandidateScore: Any IMDB titles with a distance larger than this will not be considered as candidates for a search title.
* maximumChoiceCandidateScore: If an IMDB title has a score equal or lower than this, it will be considered a choice candidate for the search title. If a choice candidate is   encountered, all other candidate for that search title are scrapped and the search will be terminated for the search title in question. 

Next, create the title data file used by the program by running the update command. This will parse the IMDB datasets and create a single, unified title file from them that will be used for the title search.

Now create a file containing the titles you wish to search for. This consists of delimited rows of title, year and director (for example "Amelie;2001;Jean-Pierre Jeunet"). The enrich command requires the input and output files to be provided via the -i and -o flags. If necessary, the default data file, maximum candidate score and choice candidate score can be overwritten using the -d, --max-candidate-score and --max-choice-candidate-score arguments. The program will then compare the given titles to the data file and output those which have a candidate score lower or equal to the maximum candidate score. If an IMDB title is found that is lower or equal to the maximum choice candidate score, it will be chosen as a choice candidate and no other candidates will be outout for that search title.

### Summary

* Get datasets
* Update datafile with update -d datasetDirectoryPath -o dataFilePath
* Create input file
* Enrich input file with enrich -i inputFilePath -o outputFilePath -d dataFilePath --max-candidate-score 8 --max-choice-candidate-score 2

## Technical walkthrough

The program uses a relatively simple structure based on interfaces. The main components are the readers, the comparer and the algorithms.

The readers handle dataset, title file and input file hadling. The ImdbDataSetUpdater-class also handles file writing, as it creates the data file used by the program.
The comparer is a minimal interface consisting of a single method which accepts a search title and an imdb title as input. The implementation is expected to handle all logic relating to the process of comparing these two, such as how to handle missin information in either the search title or the IMDB title.
Like the comparer, the algorithm interfaces are also meant to be as minimal as possible, generally consisting of a single method that accepts two parameters of the same type and returns an integer score representing how different the two objects are. 

This structure should allow easy the components to be easily swapped for other implementations. For example, the current string distance algorithm is an implementation of Levenshtein distance, so it measures how many insterts, deletions or substitutions it takes to convert one string into another. Since the interface is minimal, it would be very easy to create another implementation which uses Hamming distance instead. Likewise, the current year distance algorithm is a very simple implementation that only takes the absolute distance between the two values. This is a relatively naive method, but due to the architecture the rest of the program does not care about the details of the implementation this can be easily swapped out.

## TODO

* More configuration options, for example how to handle null values in search and IMDB titles
* More algorithm implementations, particularily for comparing years
* Automatic dataset updates that don't require the user to download the files
* Better input / output format
* Automated tests

