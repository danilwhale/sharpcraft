## SharpCraft
### clone of minecraft, but on raylib (5.0) and c# (.net 8)

this is a clone of minecraft (ancient versions), that is using own codebase (kinda)

## won't fix
- no shadows like in the original
  - this is a second implementation attempt, I had to do a lot of optimizations, yet it still runs like a shit.
  - TLDR: nop, I won't add them

## how to get the latest version??????
[click here to go to GitHub actions](https://github.com/danilwhale/SharpCraft/actions/),
then select top most link with checkmark from the left, click on it,
scroll down, download artifact for your platform (windows or linux)

> [!IMPORTANT] 
> read a section below before downloading and running the game

> [!WARNING] 
> untested on linux, except bugs

## assets
now you need to extract assets from the original game yourselves.
this is done so microsoft won't murder me for using their assets in my project.

where to find rd-132328.jar:
- official launcher: `%appdata%/.minecraft/versions/rd-132328/rd-132328.jar` (can't tell path on linux)
- prism launcher: `%appdata%/PrismLauncher/libraries/com/mojang/minecraft/rd-132328/minecraft-rd-132328-client.jar` (can't tell path on linux)

find and open rd-132328.jar in any archiver (for example, 7-zip)
and extract the following files:
- terrain.png
- char.png

after this, create folder 'Assets' inside SharpCraft's directory and move them to this folder

## how to build
1. clone repo (using download button, git clone or ide's vcs clone system)
2. open project in any ide you want
   - in case you don't have any ide, open project's folder in command prompt
3. build it!
   - no ide? dotnet build inside project's folder

## fixed bugs/finished tasks
- [x] you can go through a block if you walk diagonally into it (can't reproduce in original)
- [x] fix flashing chunks on block placing/removing
- [x] find out why chunks go black sometimes
- [x] refactor code a lot
- [x] make GitHub actions work (build for linux and windows)

**NOT AN OFFICIAL MINECRAFT PRODUCT. NOT APPROVED BY OR ASSOCIATED WITH MOJANG OR MICROSOFT**
