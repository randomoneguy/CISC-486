# CISC-486
Arena Boss Rush TouHou inspired

# 🎮 Touhou Souls

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

## 🎬 Scripted Events
The first boss will spawn immediately after a short cinematic/dialog scene. On some boss phases, it will feature timed conditions (such as hitting them down to HP threshold, avoiding boss attacks, hitting levers to deactivate the boss gimmicks, etc.), where the player is penalized if they are unable to fulfill the condition until the timer hits 0. The sub-sequent bosses will spawn after the previous boss dies.

## 🌍 Environment
The game will occur in an arena. 

## 🧪 Physics Scope
Rigidbody on baby, babysitter, and throwables  
Colliders and triggers on hands, bins, hazards, and gates  
Physics materials for toys rubber ball high bounciness, blocks low bounciness  
Layer-based collision matrix hands catchable, world static, hazard  
Force-based throws calculated from baby arm animation and intent  
Catch window defined by trigger volume on babysitter hands with relative velocity threshold  
Simple ragdoll or joint-based flail for baby during Mischief if time permits

## 🧠 FSM Scope
State machines implemented for all the bosses and special projectiles.
Event driven transitions using Unity events and C# events  
Timers for diaper bomb arming and detonation  
Blackboard style memory for baby last seen hazard, last seen babysitter, favorite toy  
Debug overlay that prints current states for grading

## 🧩 Systems and Mechanics
Scoring gain points for catches, lose points for misses, streak bonus for consecutive catches  
Safety meter decreases when hazards occur and replenishes slowly when calm  
Diaper bomb rules arm on spawn, can be caught and disposed into bin, detonates if timer expires or if it collides with stove zone  
Object tagging Toy, Hazard, Catchable, Bin, StoveZone  
Camera third person follow with collision handling  
Audio cues baby giggle when Mischief, rising beep for bomb, thud on missed catch  
VFX confetti on perfect catch, smoke puff on bomb disposal, stink cloud on bomb near detonation

## 🎮 Controls (proposed)
Mouse and Keyboard control
W A S D move  
Mouse look  
Left click and Right click to light and heavy attack
Esc pause

## 📂 Project Setup aligned to course topics
Unity (version)  
C# scripts for PlayerController, BabyAIController, ThrowManager, CatchZone, BombController  
NavMesh for AI pathing  
Animator controllers for baby and babysitter with parameters speed, isHolding, isThrowing, isCalm  
Physics materials and layers configured in Project Settings  
GitHub Classroom repository with regular commits and meaningful messages  
Readme and in game debug UI showing FPS, state names, and safety meter for assessment

Assets such as character models, particles and effects will mostly be made in blender, and 3DS max will be used for modelling animation. Some other minor assets may be pulled from the unity asset store, such as music and sound effects, which will be license free or commission based depending on the need. 

## Timeline
W4 - Basic box arena environment and project set up with basic assets.
W5 - Start creating the first boss and the first player Reimu. (Movements and animations, collision scripts)
W6 - Player attack and combos, boss attacks and patterns.
W7 - Player health bar and taking damage scripts/interactions. Boss health bar and taking damage scripts.
W8 - Test interactions and finalize gameplay.
W9 - Start creating second playable character, Marisa and second enemy AI, implemented based on current player and enemy.
W10 - Create different attack types and patterns for player 2 and boss.
W11 - Game playable with 2 players on same device and control.
W12 - Implement LAN co-op.
W13 - Finalize game.
