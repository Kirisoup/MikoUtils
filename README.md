# MikoUtils

A Human: Fall Flat plugin based on [BePinEx](https://github.com/BepInEx/BepInEx), that aimed at simplicity and quality-of-life aspects of online session hosting. 

## Usage: 
1. Download the [latest release of BePinEx](https://github.com/BepInEx/BepInEx/releases/latest). AFAIK it should be the `BepInEx_x86... .zip` in most cases; 
2. Unzip it and move it to the game directory; 
  - i.e. where your `Human.exe` is located
  - should be `SteamLibrary\steamapps\common\Human Fall Flat` by default
  - Make sure that the folder named `BepInEx` is directly under the said directory
3. Launch your game once, and close it; 
  - You should now see numeral folders under your `Human Fall Flat\BepInEx` directory, including one named `Plugins`
4. Download the [latest release of MikoUtils](https://github.com/Kirisoup/MikoUtils/releases/latest), and move it into the `Human Fall Flat\BepInEx\Plugins` folder; 
5. The plugin should be installed successfully! 

## Features:

> Notice: Most, if not all, of the features only works if you are hosting the session. 

- **Force kick**: 

  Due to HFF devs' oversight, it is very easy for players with modified client to bypass /kick from the host (They can stay on the server and play, even after being kicked, untill they are disconnected from the session). 

  This plugin fixed it by force disconnect the player after the original OnKick method. Simply use the `/kick` command or kick players from the pause menu as usual and it will do. 

- **Toggle _Invite Only_, _Join Game in Progress_, _Lock Level_ options with commands, with a new _Lock Checkpoint_ option**

  `/invt`, `/join` and `/locklvl`

  This allows you to change those options while playing a level, instead of going back to the lobby and lose the progress. 

  - Additionally, this plugin introduced a new option which allows you to lock checkpoints, with command `/lockcp`. 

- **Multiple quality-of-life changes to `level(l)` and `cp(c)` console commands, and introduces a new command `subobj(s)`**

  > The console can be opened by pressing `F1` or `~` while in game.

  **General change:**
  - You can now use +/-<integer> to load levels/checkpoints/sub-objectives relative to the current number (+/-1 can be abbreviated as +/-). 

    E.g., `cp -1` or `c -` to load the previous checkpoint, or skip two levels ahead with `level +2` or `l +2`. 

  **Special changes to `level`:**
  - The second parameter of `level` is now optional.

    Level command used to require two parameters -- `level <level> <checkpoint>`, now if the second parameter is left empty, checkpoint 0 is loaded instead.

  - It can now load extra levels.
 
    If `<level>`' has a value that is higher than the number of default levels (as the date of writting this, there are 14 default levels), extra levels will be loaded.

    E.g. `level 14` would load the level "Thermal".

  - It can now be ran in multiplayer sessions!
 
  **Sub-Objectives**

  A new command, `subobj(s) <subobjective>`, which loads sub-objectives under the current checkpoint (AFAIK, sub-objectives is only ever used in the first checkpoint of "Factory")

- **New command `changecp(cc)`**

  This command changes the current checkpoint number, but does not load it, therefore the map won't be reset and players won't be respawned.

## TODO: 
- Support English.
- Some random ideas:
  - PartyPass: pass the level only if every players has entered the pass trigger
  - An option to let the host decide whether to accept a connection when new players are trying to join

# Any suggestion and bug reports are apppreciated!
