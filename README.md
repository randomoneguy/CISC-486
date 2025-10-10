# CISC-486
Arena Boss Rush TouHou inspired

# 🎮 Title: Touhou Souls


# UPDATE - Current FSM for enemy Robot NPC

<img width="640" height="680" alt="Blank diagram (1)" src="https://github.com/user-attachments/assets/593e9574-2593-4eeb-a871-5db2003bf474" />

Idle - Enemy AI stand still, idle animation playing<br>
Walk - Enemy AI slowly approaches player, walking animation playing<br>
Charge - Enemy AI sprints towards player, running animation playing<br>
Kick - Enenmy AI performs a kick attack, kicking animation playing<br>
Laser - Enemy AI performs a laser attack, tracing line beam generated<br>
<br>
The enemy starts from Walk state as they spawn and chase the player.<br>
From Walk they can transition to Laser - when the player is in laser attack range and laser is not on cooldown (currentTime - lastLaserUseTime > laserCoolDown)<br>
From Walk they can transition to Charge - when they're in charge range of the player and Charge is not on cooldown<br>
From Walk they will transition to Idle - when they are currently in stopping distance to player (1.5f radius)<br>
From Idle they can transition to Kick attack - when they're still in melee range (1.75f) of player and Kick is not on cooldown<br>
From Idle they can also transition to Walk - once player walks out of the stopping distance range<br>
From Kick they will always transition back to Idle after animation finish playing and in Idle state it will decide to transition other states when applicable<br>
From Charge they can transition to Kick immediate without Idle if they charged within the attack range. <br>
From Charge they can transition to Walk if duration of Charge runs out before reaching the player (ie player dashes away) <br>
From Laser they will always transition to Idle after laser duration + an brief offset is over so they can decide their next course of action from Idle<br>
<br>
<iframe width="560" height="315" src="https://youtu.be/oilqU67yj-Y" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
<br>
This is the demo video for assignment 2 report<br>
## 📌 Overview
Touhou Souls is designed as a 3D arena boss rush game. The player(s), originally from the Touhou universe, namely Reimu and/or Marisa, will be transported to an arena in another world. In the arena, they will have to survive and win against numerous boss encounters to beat the game.

## 🕹️ Core Gameplay
Each player can control either Reimu or Marisa to fight the incoming bosses.
Perform attack combos and special abilities to do damage to bosses.
Avoid taking damage from bosses by dodging.
Win by beating every boss encounter.
Lose if all player health drops to 0.

## 🎯 Game Type
The game is going to follow the arena battle/boss fight genre similar to boss fights in games like the Binding of Isaac or Hades (bosses use telegraphed area of attack patterns with some featuring bullet hell element) with the player utilizing the combo-based combat feature similar to that of Dark Souls or Monster Hunter (mix of light and heavy attacks to string different attack combos with special gimmick between two characters). 

## 👥 Player Setup
We plan to develop the game as mainly a single or two-player game locally. While the objective of the game is same between single and two-player, the enemies in two-player will be stronger requiring the players to cooperate or fight together. 

## 🤖 AI Design
The enemies will aggro on the player immediately after spawning/entering the arena. The enemies will have some basic attack patterns that are telegraphed and predictable with slight variations depending on the player’s position (ranged attack if they are far, sweep attack if they are too close). But sometimes they will throw in special attacks based on a controlled randomizer which the players will have to react to. The harder bosses will have phases as well that will trigger upon failure to stop the boss’ mechanics, such as powering up, which will enhance their current move-sets/attacks. 

Example enemy FSM<br>
Idle<br>
Move towards player<br>
Make a sweep attack<br>
Charge for throwing ranged projectile<br>
Summon destroyable totems<br>
If totem not destroyed in time - add damage multiplier and heal hp<br>
If totem is destroyed - increase damage taken multiplier and remain idle for certain time<br>

## 🎬 Scripted Events
The first boss will spawn immediately after a short cinematic/dialog scene. On some boss phases, it will feature timed conditions (such as hitting them down to HP threshold, avoiding boss attacks, hitting levers to deactivate the boss gimmicks, etc.), where the player is penalized if they are unable to fulfill the condition until the timer hits 0. The sub-sequent bosses will spawn after the previous boss dies.

## 🌍 Environment
The game will occur in an arena. It will be themed as an underground dungeon. 

## 🎮 Controls (proposed)
Mouse and Keyboard control
W A S D move  
Mouse look  
Left click and Right click to light and heavy attack
Esc pause

## 📂 Project Setup
Assets such as character models, particles and effects will mostly be made in blender, and 3DS max will be used for modelling animation. Some other minor assets may be pulled from the unity asset store, such as music and sound effects, which will be license free or commission based depending on the need. 

## Group Information
Members:
Aniss Hamouda - 20348807
Sungmoon Choi – 20359170 
Tim Zhang – 20294394 
