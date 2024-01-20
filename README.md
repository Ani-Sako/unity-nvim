# unity-nvim

## About
Setup and instructions to use Neovim as the editor for Unity on Windows. 

Does not require modifying your Noevim config. 

This project allows you to:
1.  Open files in nvim from Unity GUI. All files opened from the same Unity project will open in the same Neovim instance!
2. (Re)Generate c# project files from within Unity (necessary for getting nvim LSP working with project) without having to download VSCode/Visual Studio/Rider. 

Note: This project does not include nvim LSP setup. 

## Requirements 
- Windows >= 10
- Unity 2020.3 LTS, Unity 2022.3.17f1 LTS 
- neovim >= 0.9 
- dotnet >= 6.0 (if publishing the module, alternatively download the latest release executable). 

## Setup
Setup differs for different versions of Unity.

Note that the executable is called code to trick Unity into thinking the external editor is VSChode and allow regenerating project files (necessary for getting nvim LSP working with project). This is necessary for this project to work. 

### Unity 2020.3 LTS

With some version of dotnet >= 6.0 installed, run the following command from this projects root folder.
```
dotnet publish -c Release -r win-x64 --self-contained  -o ./publish -p:PublishSingleFile=true /p:AssemblyName=code /p:DebugSymbols=false /p:DebugType=None
```
Alternatively, download `code.exe` from release [v2.0](https://github.com/Ani-Sako/unity-nvim/releases/tag/v2.0).


Now simply go to `Edit->Preferences->External Tools`, and `browse` in the `External Script Editor` field to select the above published executable.
In the `External Script Editor Args` field, paste `+$(Line) $(File)`.

The menu will update with a list of checkboxes below `External Script Editor`. Check `Embedded Packages` and `Local Packages` and click `Regenerate project files`.

### Unity 2020.3 LTS

With some version of dotnet >= 6.0 installed, run the following command from this projects root folder.
```
dotnet publish -c Release -r win-x64 --self-contained  -o ./publish -p:PublishSingleFile=true /p:AssemblyName=code /p:DebugSymbols=false /p:DebugType=None
```
Alternatively, download `code.exe` from release [v3.0](https://github.com/Ani-Sako/unity-nvim/releases/tag/v3.0).

## Usage
Clicking `Edit Script` in Unity or clicking on error messages in the console will now open neovim. If you have an LSP setup, the LSP will be able to successfully detect the root project directory and attach to the file. 

Note: Omnisharp LSP seems to work the best with Unity. However, it can be slow on startup and it may take a while for LSP functionality (eg. renames/code actions) to start functioning. 

## Todo
- Currently files open in the current window, regardless of if the file is currently already open in another window. Ideally the executable would check to see if the file is already in a buffer and open in a window, and navigate to the window instead of opening in the current window. 
