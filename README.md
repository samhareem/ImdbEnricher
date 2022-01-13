# ImdbEnricher

A simple command line tool for attempting to get IMDB data for uncertain movie titles based on title name, release year and director. Uses fuzzy logic to compare the given information to that of IMDB and outputs a list of candidates or a single candidate if a close match is found.

The main use cases for the tool are...

1) Attempting to confirm unreliable movie titles and get additional information for them. For example, those cases where you can't remember the exact name of a title but remember it came out in the 90s.
2) Attempting to enrich a larger set of inaccurate and / or non-standardised titles with (relatively) standardised IMDB information. For example, if you have a set of titles that have been shown at a film festival, but some of the titles contain mistakes or are missing information.

The inspiration for this tool came from the need to add genre information to a list of movies shown at the [Helsinki International Film Festival](https://hiff.fi/historia/). As the list (at the time) was quite messy, it proved difficult to link the titles to the IMDB datasets. Adding fuzzy logic to the comparison proved to be much more effective, particularily when combined with director names and release years to decrease the number of false positives caused by identical and near identical titles.

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
