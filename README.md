# MikoUtils

[*中文*](https://github.com/Kirisoup/MikoUtils/blob/main/README/zh.md)

A Human: Fall Flat plugin using [BePinEx](https://github.com/BepInEx/BepInEx), that aimed at simplicity and quality-of-life aspects of online session hosting. 

Supports multi-language! Currently only supports Chinese (simplified) and English, feel welcomed to ask for other language support <3

## Usage: 
1. Download the [latest release of BePinEx](https://github.com/BepInEx/BepInEx/releases/latest). It should be the `BepInEx_x86... .zip`; 
2. Unzip it and move it to the game directory.
  - i.e. where your `Human.exe` is located;
  - should be `SteamLibrary\steamapps\common\Human Fall Flat` by default;
  - After you moved them, make sure that the folder named `BepInEx` is directly under the said directory!
3. Launch your game once, and close it; 
  - You should now see numeral folders under your `Human Fall Flat\BepInEx` directory, including one named `Plugins`
4. Download the [latest release of MikoUtils](https://github.com/Kirisoup/MikoUtils/releases/latest), and move it into the `Human Fall Flat\BepInEx\Plugins` folder; 
5. The plugin should be installed successfully! 

## Features:

> Notice: The features of this plugin are made for multiplayer lobby hosting, and should not be used as a client playing in others' lobbies (they won't do anything even if you tried).  
> Though many of them work in single player too :D

Everything can be found with the vanilla `help` `/help` commands!

### Multiplayer Utilities

**Anti-Anti-Kick:**  
Prevents players with anti-kick to annoy you and your friends.  
Due to an oversight of this game, it is very easy for players with a modified game to bypass /kick from lobby host. This fixes the issue, by force disconnect the player (any further attempt to connect back to your lobby is prevented in vanilla, and will somehow crash their game lol)

**Disconnect Player:**  
> Syntax: `/disconnect(dc) <player#>`  
Disconnect the selected player from the lobby.
- Unlike /kick, the player can still join the lobby after being disconnected;
- Note: the implementation is not well polished, that clients won't realize themselves being disconnected, so their character freeze (They can still exit manually).

**Join & Left Message**  
Print message in chat whenever a player joins or leaves the game.

**Broadcast Toggle:**  
> Syntax: `/broadcast(bc)`  
Toggle broadcasting commands to public chat (by default notifications are only visible to yourself).
- If a notification begins with `=`, then only yourself can see it; if it starts with `#`, everyone can see it.

**Synchronize Level:**  
> Syntax: `/synclvl(sync)`  
An attempt to fix client-host level desync (i.e. the level not being properly updated for players due to connection issues, for example your lobby has finished Mansion and progressed to Train, but on the players' side the level they see is still Mansion).

### Load cheats (Console commands):

#### Quality of life changes made for the vanilla `level(l)` and `cp(c)` commands:  

**`level` Command**  
> Syntax: `level(l) <level> <checkpoint?>`  
- `level` command now won't require the second <checkpoint> value;
  > `level 0` will simply load Mansion
  > `level 0 3` will still load the third cp of Mansion, like in vanilla (vanilla means the original game without modification)
- `level` command can load extra levels! When you load a level with a number larger than the number of regular levels, extra levels are loaded (e.g. `level 14` will load Thermal);
- you can use `level +/-<number?>` to load levels relative to the current level.
  > - `level +1` to load the next level, `level -2` to load the second previous level
  > - `level +` is equal to `level +1`, the same applies for `-`

**`cp` Command:**  
> Syntax: `cp(c) <checkpoint>`  
- Similar to `level`, `cp +/-<number?>` will load checkpoint relative to the current one.

#### New commands:

**Sub-objective Command:**  
> Syntax: `subobj(s) <sub-obj>`  
Loads sub-objectives under the current checkpoint (*this is currently used in the first checkpoint in Factory -- the lever furnace puzzle*).
- Will simply reload checkpoint if there is no sub-objectives under the current checkpoint.

**Change Checkpoint Command:**  
> Syntax: `changecp(cc) <checkpoint>`  
Change current checkpoint without reload.
- pretty usefull when you want to go to a checkpoint without disrupting other players and revert the map;
- Especially useful when playing custom maps with complicated puzzles that cannot handle checkpoint reload properly!

### Lobby settings (Chat commands, some accessible from console):

Vanilla lobby settings can now be changed while inside level using commands (So you don't have to quit to lobby and lose your progress).

**"Invire Only" Toggle:**  
> Syntax: `/invonly(io)`  

**"Join Game in Progress" Toggle:**  
> Syntax: `/joingame(jg)`  

**"Lock Level" Toggle:**  
> Syntax: `/locklvl(lockl)`  
Prevent players from finishing the level;
- Also accessible in single player from console with `locklvl(lockl)`;
- Single player setting and online lobby setting will be synchronized.

**"Lock Checkpoint" Toggle** (*This is a new setting introduced with the plugin*):  
> Syntax: `/lockcp(lockc)`  
Prevent players from progressing to other checkpoints;
- Single player setting and online lobby setting will be synchronized.
- Also accessible in single player from console with `lockcp(lockc)`;
- Does *not* prevent players from respawning at the current checkpoint;
- Makes levels harder & prevents your progress to be messed up by strangers.

## TODO: 
- Some random ideas:
  - PartyPass: pass the level only if every players have entered the pass trigger
  - An option to let the host decide whether to accept a connection when new players are trying to join

# Any suggestion and bug reports are apppreciated!
