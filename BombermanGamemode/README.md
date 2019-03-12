Bomberman
======
made by Joker 119
## Description
Various ways of blowing up all your friends.

### Features
 - Settings+commands to set spawn type.
 - Supports normal rounds aswell as unique ones.
 - Configurable delays for grenade drops.
 - Grenades drop faster and in larger quantities until there's one person left alive (or one team in warmode).

### Config Settings
Config option | Config Type | Default Value | Description
:---: | :---: | :---: | :------
bomber_min | Int | 15 | The minimum period of time between grenade drops.
bomber_max | Int | 30 | The maximum period of time between grenade drops.
bomber_class | String | | The type of class to spawn for the round. See description below.
bomber_medkit | Bool | True | If players should spawn with an extra medkit.

### Commands
  Command |  |  | Description
:---: | :---: | :---: | :------
**Aliases** | **bomberman** | **bomb**
bomb enable | | | Enables the gamemode.
bomb disable | | | Disables the gamemode.
bomb drop | | | Repeates the previous grenade drop.
bomb flash | | | Drops a flash grenade on everyone.
bomb select ClassName | | | Selects the class to spawn for this round. (See below).
bomb bosswave | | | Cuts the drop times in half.

### Class Options
Class Name | | | Description
:---: | :---: | :---: | :------
classd | | | Spawns everyone as Class-D in Light Containment CD-01.
sci | | | Spawns everyone as Scientists in their normal random locations.
guard | | | Spawns everyone as Facility Guards in their normal random locations.
ntf | | | Spawns everyone as NTF commanders in the NTF spawn.
chaos | | | Spawns everyone as Chaos Insurgency at their normal spawn.
war | | | Half the server spawns as Sci, the other half as Class-D. They are immune to their own grenades, and must use them to kill the other team.