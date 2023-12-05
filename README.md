# unity-nvim

## About
A executable wrapper around `nvim.exe` that 
1. Open files in nvim from Unity GUI
2. Opens all files in the same neovim instance
2. (Re)Generate c# project files from within Unity (useful for getting nvim LSP working with project) without having to download VSCode/Visual Studio/Rider. 

Note: This project does not include nvim LSP setup. 

## Requirements 
- Windows >= 10
- Unity >= 2020.3 
- neovim >= 0.9 
- dotnet >= 6.0 (if publishing the module)

## Setup
With some version of dotnet >= 6.0 installed, run the following command from this projects root folder.
```
dotnet publish -c Release -r win-x64 --self-contained  -o ./publish -p:PublishSingleFile=true /p:AssemblyName=code /p:DebugSymbols=false /p:DebugType=None
```
Note that `AssemblyName=code` is to ensure the executable is named `code.exe`. This tricks Unity into thinking the external editor is VSChode, which allows for regenerating project files.
Alternatively simply download `code.exe` from releases.

Now simply go to `Edit->Preferences->External Tools`, and `browse` in the `External Script Editor` field to select the above published executable.
In the `External Script Editor Args` field, paste `+$(Line) $(File)`.

The menu will update with a list of checkboxes below `External Script Editor`. Check `Embedded Packages` and `Local Packages` and click `Regenerate project files`.

Clicking `Edit Script` in Unity or clicking on error messages in the console will now open neovim. If you have an LSP setup, the LSP will be able to successfully detect the root project directory and attach to the file. 

Note: Omnisharp LSP seems to work the best with Unity. However, it can be slow on startup and it may take a while for LSP functionality (eg. renames/code actions) to start functioning. 
