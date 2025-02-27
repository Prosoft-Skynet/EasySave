# Project EasySave

This application is designed to create, execute, and manage backup jobs efficiently.
There's two version of this project (one which is only console-oriented and the other one with an interface).

# 1. Installation

```
git clone https://github.com/Prosoft-Skynet/EasySave.git

cd EasySave

dotnet build 

dotnet run --projet <EasySaveGUI> <EasysaveConsole>
```

# 2. Architecture 
- **EasySaveConsole**: This is the console part of the project, you won't have an interface but all the features are working perfectly
- **EasySaveGUI**: This is the project with a the graphical interface

### MVVM Model: 
The code is split in distinct parts:
#### - **Models**: Models contains the definitions of all the objects used in the projects
#### - **Views**: Views contains all the files which will be seen by the users
#### - **ViewModels**: ViewModels contains all the files used to make the link between the Models and the View

# 3.Configuration
Only available on Windows

Need .Net 8 configuration

# 4. Documentation
Here you can find further explainations about the projet:

https://flat-ketch-6e5.notion.site/Documentation-192ce649365f80dc9ed2f35cc36515c1?pvs=4


# 5. Publish

```
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true -p:IncludeAllContent=true
````