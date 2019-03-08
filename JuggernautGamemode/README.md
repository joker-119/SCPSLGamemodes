Juggernaut Gamemode
======
## Description
 One player is selected randomly to spawn as the Juggernaut, a Chaos Insurgency, with the objective of eliminating all NTF Personnel.
 Rest of the server is spawned in as NTF Commanders initialy, and respawned through natural NTF Waves.
 The round will end when either the Juggernaut or all NTF are eliminated.

### Features
 - Players who join before the round has started will be notified that Juggernaut Gamemode is starting.
 - All players will be given a broadcast of their role and their objective.
 - Juggernaut gains 500 HP for every player at round start.
 - Using the femur breaker and the 106 recontainment prodecure will cause the Juggernaut to be teleported to a random location and take massive damage as a critical hit.
 - Juggernaut spawns with a Logicer, O5 Keycard, and 6 Frag Grenades. Increased 7.62mm Ammo.

### Config Settings
Config option | Config Type | Default Value | Description
:---: | :---: | :---: | :------
juggernaut_base_health | Int | 500 | The amount of starting health the Juggernaut should spawn with.
juggernaut_increase_amount |Int | 500 | The amount of additional health the Juggernaut gets per player on the server.
juggernaut_jugg_grenades | Int | 6 | The amount of grenades (0-6) the Juggernaut should spawn with.
juggernaut_ntf_disarmer | Bool | false | Wether or not the NTF should spawn with Disarmers in place of their second Frag Grenade.
juggernaut_ntf_health | Int | 150 | The amount of health NTF should spawn with.
juggernaut_critical_damage | Float | 0.15 | The % of the Juggernaut's current health the Femur Breaker should deal in damage to the Jugg. (0-1)
juggernaut_jugg_infinite_nades | Bool | true | If the Juggernaut should have infinite grenades.
juggernaut_health_bar_type | String | bar | The type of text used to display the Juggernaut health at the top of the screen. Accepts: bar, raw and percent

### Commands
  Command |  |  | Description
:---: | :---: | :---: | :------
**Aliases** | **Juggernaut** | **Jugg** | **Jug**
Juggernaut Enable | ~~ | ~~ | Enable Juggernaut for the next round
Juggernaut Disable | ~~ | ~~ | Disable Juggernaut this and following rounds
Juggernaut Select | [PlayerName] | ~~ | Select player as next juggernaut if possible
