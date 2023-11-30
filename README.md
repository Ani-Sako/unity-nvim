# unity-nvim

## About
Set neovim as a functional editor for Unity 2020 and above. Opens neovim when editing script from unity, and also regenerate files without having to install VisualStudio/VSCode.

## Requirements 
1. Unity >= 2020.3 
2. neovim >= 0.9 
3. Omnisharp lsp: For code completion etc
    - Note Omnisharp lsp requires dotnet version 6.
4. dotnet >= 6.0

## Setup
1. With some version of dotnet >= 6.0 installed, run the following command from this projects root folder.
```
dotnet publish -c Release -r win-x64 --self-contained  -o ./publish -p:PublishSingleFile=true /p:AssemblyName=code /p:DebugSymbols=false /p:DebugType=None
```
This creates an executable `code.exe` in a new folder `publish`. The executable opens neovim with command line options. 

2. In Unity go to `Edit->Preferences->External Tools`, and select the `browse` in the `External Script Editor` option. Navigate to `publish` and select `code.exe`. 
The menu should update with a list of checkboxes for generating files. Check `Embedded Packages` and `Local Packages`. Then click `Regenerate project files`. 
3. Paste `$(File) + $(Line)` in the `External Script Editor Args` option.

## Why?
Regenrating project files is esssential to get the benefits of LSP, such as suggestions and autocomplete. 
Unity only allows regenerating project files when using VSChode/Visual Studio Code or Rider. 
However, unity only checks the name of the executable. We name the executable `code.exe` to make unity think we are using VSChode and allow regenerating project files. 



