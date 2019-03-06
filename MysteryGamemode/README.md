Murder Mystery
======
made by Joker 119
## Description
A random selection of players will be choosen as Detectives (Scientists), Murderers(Class-D) and Monsters(SCPs). Everyone else will be spawned as Civilians(Class-D).

Detectives spawn with a COM-15, Disarmer, Medkit, Flashlight and Containment Engineer card.
Murderers spawn with a USP, Medkit, Flashlight, Coin and Zone Manager Card.
Civilians spawn with a Flashlight, Coin and Janitor card.
SCPs spawn normally.

The objective of the game is for the Murderers to try and kill all the other humans. Civilians and Detectives must work together to find and eliminate the murderers.
The monsters can choose to help the humans, or the murderers, or no one.

The round will end when all Detectives and Civilians have been killed, or when all Murderers have been killed. (Monsters alive or dead won't hinder round progression)




### Features
 - Objectives are broadcasted to players upon spawning.
 - Instead of normal respawn events, players will be respawned as more Civilians, however 2 random players will be selected to become a new Murderer and Detective.
 - Configurable number of Monsters, Murderers and Detectives.
 - Configurable health values for all non-monsters.

### Config Settings
Config option | Config Type | Default Value | Description
:---: | :---: | :---: | :------
myst_murd_health | Int | 150 | How much health Murderers will spawn with.
myst_civ_health | Int | 100 | How much health Civilians will spawn with.
myst_det_health | Int | 150 | How much health Detectives will spawn with.
myst_murd_num | Int | 3 | How many Murderers are chosen when the round starts.
myst_det_num | Int | 3 | How many Detectives are chosen when the round starts.
myst_monster_num | Int | 3 | How many monsters will be spawned.
myst_murd_respawn | Bool | True | If a random murderer should be choosen during respawns.
myst_det_respawn | Bool | True | If a random detective should be choosen during respawns.

### Commands
  Command |  |  | Description
:---: | :---: | :---: | :------
**Aliases** | **mystery** | **murder**
mystery enable | | | Enables the Murder Mystery Gamemode.
mystery disable | | | Disables the Murder Mystery Gamemode.
