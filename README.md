## SharpCraft
### clone of minecraft, but on raylib (5.0) and c# (.net 8)

this is a clone of minecraft (ancient versions), that is using own codebase,
only some parts are ~~stolen~~ grabbed from original source code, because I have no clue how to reimplement them lol

## found a bug?
please report it in issues!!!!!!!

## known bugs
- you can go through a block if you walk diagonally into it (can't reproduce in original)

## won't fix
- no shadows like in the original
  - this is a second implementation attempt, I had to do a lot of optimizations, yet it still runs like a shit.
  - TLDR: nop, I won't add them
## how to get the latest version??????
[click here to go to GitHub actions](https://github.com/danilwhale/SharpCraft/actions/),
then select top most link with checkmark from the left, click on it,
scroll down, download artifact for your platform (windows or linux)
> ⚠️ linux build is untested, if not working, build it yourself using the instructions below

## how to build
1. clone repo (using download button, git clone or ide's vcs clone system)
2. open project in any ide you want
   - in case you don't have any ide, open project's folder in command prompt
3. build it!
   - no ide? dotnet build inside project's folder

## fixed bugs/finished tasks
- [x] fix flashing chunks on block placing/removing
- [x] find out why chunks go black sometimes
- [x] refactor code a lot
- [x] make GitHub actions work (build for linux and windows)