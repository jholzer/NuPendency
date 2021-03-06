# NuPendency
NuPendency is a small tool that let´s you explore your nuGet-dependency structure. Simply tell NuPendency the package name and it will show you the depending packages...

![alt tag](https://ci.appveyor.com/api/projects/status/gum550q6d7agra1a?svg=true)

NuPendency will show you the dependencies as "force-directed-graph"

![alt tag](https://raw.githubusercontent.com/jholzer/NuPendency/master/NuPendencyScreenshot.png)

## How to use:
### Basics:
- Press "Add new graph"
- Enter the name of the package
- Wait for graph to be built!
- To resolve all packages for a Visual Studio solution open the solution using "Open Project/Sln"

### Some more:
#### Taking too long?
It may happen that large graphs may take some time, e.g when having set up a large depth in "Settings". In that case, simply presse the "Stop"-button.
####  Versions
NuPendency gets the newest version when adding a new graph. It will show you all available version of the root package an the left side. You can select an earlier version of the package. To get the graph for another version presse the "reload" button for the grahh.
#### Highlighting
When selecting a package all packages "between" the selection and the root package will be highlighed. Can be turned on/off in "Settings" (default "on").
#### Repositories
The NuGet-standard feed is already pre-installed. You can add/remove further NuGet-feeds (e.g. private) in the "Repositories"-section
#### Exlude packages
If you´re not interested in some packages, you can exclude them from the search. Simply add the name of the package to the exclude list. You can also leading and trailing wildcards (e.g. "System.*")
#### Settings
There are some settings, most regarding on how the graph is bein animated. Feel free to play with them, but be careful. There may be some funny effects with wrong values.
## Planed features:
- "Explore from here" (e.g. right-click on node and use it as root-node for a new dependency graph)
- Find package across all open graphs
- Improve graph layout (suggestions welcome)
- Logging-window
- Improve this documentation... ;-)

# Participate? Yes please!
Feel free to bring up ideas, fork, send improvements!

# Download
Wanna get it and don´t want to build it yourself? Ok... 
Find the latest build in the [Release section] (https://github.com/jholzer/NuPendency/releases)
###### Credits:
The force-directed-graph is based on https://github.com/Orbifold/Graphite (Thanks for the good example!)
