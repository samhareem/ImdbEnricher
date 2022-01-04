# ImdbEnricher

A simple command line tool for attempting to get IMDB data for uncertain movie titles based on title name, release year and director. Uses fuzzy logic to compare the given information to that of IMDB and outputs a list of candidates or a single candidate if a close match is found.

## Quickstart

First download the necessary [IMDB Datasets](https://www.imdb.com/interfaces/). The following datasets are required by the program:

* name.basics.tsv
* title.crew.tsv
* title.ratings.tsv
* title.akas.tsv
* title.basics.tsv

Once you've downloaded the datasets unpack their contents into a directory of your choosing.

Next, create the title data file used by the program by running the update command. The dataset directory and data file paths can either be set in the config.json file or using the -d and -o flags.

Now create a file containing the titles you wish to search for. This consists of comma separated rows of title, year and director (for example "Amelie;2001;Jean-Pierre Jeunet"). The enrich command requires the input and output files to be provided via the -i and -o flags. The data file, maximum candidate score and choice candidate score can either be set in the config file or given using the -d, --max-candidate-score and --max-choice-candidate-score arguments. The program will then compare the given titles to the data file and output those which have a candidate score lower or equal to the maximum candidate score. If an IMDB title is found that is lower or equal to the maximum choice candidate score, it will be chosen as a choice candidate and no other candidates will be outout for that search title.

## Summary

* Get datasets
* Update datafile with update -d datasetDirectoryPath -o dataFilePath
* Create input file
* Enrich input file with enrich -i inputFilePath -o outputFilePath -d dataFilePath --max-candidate-score 8 --max-choice-candidate-score 2
