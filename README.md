# GW2RaidTool
![build status](https://daniel-fischer.visualstudio.com/_apis/public/build/definitions/745bf3ce-6aa9-400c-a2f7-0e34d260a006/3/badge)

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/M4xZ3r0/GW2RaidTool/master/LICENSE)

[![GitHub issues](https://img.shields.io/github/issues/M4xZ3r0/GW2RaidTool.svg)](https://github.com/M4xZ3r0/GW2RaidTool/issues)
[![GitHub issues](https://img.shields.io/github/issues-closed/M4xZ3r0/GW2RaidTool.svg)](https://github.com/M4xZ3r0/GW2RaidTool/issues?utf8=%E2%9C%93&q=is%3Aissue%20is%3Aclosed)

## Description
GW2 Raid Tool is used for automatically parsing newly created ArcDps files and display the results for the user.
In order to do this the application uses a modified version of the EVTC log parser and RaidHeros.

## Shoutout
RaidHeros: https://raidheroes.tk/  
EVTC Log Parser: https://github.com/phoenix-oosd/EVTC-Log-Parser

## Installation
- Download the application from here: [latest version](https://github.com/M4xZ3r0/GW2RaidTool/raw/master/Versions/1.0.4/GW2RaidTool_1.0.4.zip)
- Unzip the content
- Run the "GW2 Raid Tool.bat" file

## Workflow
So if you aks yourself "What the hell is this thing doing?" here is a small list:
- Keeps an eye on the ArcDps log file folder
- As soon as a new file is detected / imported it is parsed by the modified EVTC log parser
- Afterwards the same file is parsed by RaidHeors and the html file saved to a defined location
That's it.

## Manual
### Main view
The main view is divided into 4 parts:
- the button area
- the encouter list
- the details area
- the log area

#### Button area
You can manually import evtc and evtc.zip files by using the add button, the clear button will let you remove a single encounter and the clear all button will remove all encounters (even the currently not visible / filtered ones).

#### Encounter list
The encount list shows all parsed encounters. Newly added encounters are automatically selected.
By double clicking on the encounter you can open the RaidHeros html file (as soon as it is available).

#### Details area
The details area shows the stats for the characters that have been present in the encounter, the details area opens as soon as an encounter is selected.

#### Log area
The log area is collapsed by default, it informs you what the application is currently doing.

### Settings
To open the options panel press the settings icon on top, in the options area you can open the folder where the RaidHeros html files are stored and filter the display in the application to display all, latest or succeeded encounters.

