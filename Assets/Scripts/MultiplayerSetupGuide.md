# Multiplayer Setup Guide

This guide will help you set up multiplayer networking for your game using Unity Netcode for GameObjects.

## Step 1: Install Required Packages

The following packages should already be installed (check in Package Manager):
- `com.unity.netcode.gameobjects` (version 2.7.0)
- `com.unity.multiplayer.playmode` (version 1.6.2) - for testing in Play Mode

## Step 2: Set Up NetworkManager in Your Scene

1. **Create NetworkManager GameObject:**
   - In your play scene, create an empty GameObject named "NetworkManager"
   - Add the `NetworkManager` component (found in Unity Netcode for GameObjects)
   - Add the `UnityTransport` component (found in Unity Netcode Transports)

2. **Configure NetworkManager:**
   - Set the **Player Prefab** to your player prefab (the one with `NetworkObject` and `PlayerController`)
   - Set **Network Prefabs List** to include:
     - Your Player Prefab
     - Your Enemy Prefabs (if they're spawned dynamically)

3. **Configure UnityTransport:**
   - **Connection Data** → **Address**: Leave as default (127.0.0.1 for localhost)
   - **Connection Data** → **Port**: 7777 (or any port you prefer)
   - **Server Listen Address**: 0.0.0.0 (allows connections from any IP)

## Step 3: Set Up Player Prefab

1. **Add Network Components to Player Prefab:**
   - Add `NetworkObject` component
   - Add `NetworkTransform` component (for position/rotation sync)
   - Ensure `PlayerController` script is attached (already converted to `NetworkBehaviour`)

2. **Configure NetworkObject:**
   - **Ownership**: Leave as default
   - **Dont Destroy With Owner**: Unchecked (unless you want players to persist)

3. **Configure NetworkTransform:**
   - **Sync Position**: ✓
   - **Sync Rotation**: ✓
   - **Sync Scale**: Usually unchecked
   - **Interpolate**: ✓ (for smooth movement)

## Step 4: Set Up Enemy Prefabs

1. **Add Network Components to Enemy Prefabs:**
   - Add `NetworkObject` component
   - Add `NetworkTransform` component
   - Ensure `EnemyAI` and `EnemyHealth` scripts are attached (already converted)

2. **Configure Enemy NetworkObject:**
   - **Ownership**: None (enemies are server-owned)
   - **Spawn With Scene**: Check this if enemies are placed in the scene
   - OR add them to Network Prefabs List if spawning dynamically

## Step 5: Set Up Player Spawning

1. **Create Spawn Points:**
   - Create empty GameObjects in your scene to mark spawn points
   - Position them where you want players to spawn
   - Name them "SpawnPoint1", "SpawnPoint2", etc.

2. **Add NetworkPlayerSpawner:**
   - Create an empty GameObject named "PlayerSpawner"
   - Add the `NetworkPlayerSpawner` component
   - Assign the spawn point Transforms to the **Spawn Points** array
   - Optionally assign a Player Prefab (if different from NetworkManager's default)

## Step 6: Set Up Connection UI

1. **Create UI Canvas:**
   - Create a Canvas in your scene (UI → Canvas)
   - Add the following UI elements:
     - **Panel** (background for menu)
     - **InputField** (for IP address) - use TMP_InputField
     - **Button** (Host)
     - **Button** (Client)
     - **Button** (Disconnect)
     - **Text** (Status) - use TextMeshProUGUI

2. **Add NetworkManagerUI Script:**
   - Add `NetworkManagerUI` component to a GameObject (or the Canvas)
   - Assign all UI references:
     - Menu Panel
     - IP Address Input Field
     - Host Button
     - Client Button
     - Disconnect Button
     - Status Text

## Step 7: Testing in Play Mode

### Option A: Using Multiplayer Play Mode (Recommended)

1. **Enable Multiplayer Play Mode:**
   - Window → Netcode for GameObjects → Multiplayer Play Mode Tools
   - This allows you to test with multiple clients in the editor

2. **Configure Play Mode:**
   - Set number of clients (2 for your game)
   - Click "Start All" to run host + clients simultaneously

### Option B: Build and Test

1. **Build the Game:**
   - File → Build Settings
   - Add your scene to Scenes in Build
   - Build to a folder

2. **Test Locally:**
   - Run the build
   - In Unity Editor, click "Host" button
   - In the build, click "Client" and enter "127.0.0.1"
   - Both should connect

### Option C: Test Over Internet

1. **Host Setup:**
   - Host needs to port forward (or use Unity Relay - see below)
   - Find your public IP address (google "what is my ip")
   - Client connects to that IP

2. **Using Unity Relay (Recommended for Internet):**
   - Unity Relay handles NAT traversal automatically
   - Requires Unity Cloud setup
   - More complex but works behind firewalls

## Step 8: Important Notes

### Network Authority:
- **Players**: Only the owner processes input and movement
- **Enemies**: Only the server runs AI logic
- **Health**: Only the server modifies health values

### Common Issues:

1. **"NetworkObject not found" error:**
   - Make sure all networked prefabs have `NetworkObject` component
   - Add prefabs to NetworkManager's Network Prefabs List

2. **Players not spawning:**
   - Check that `NetworkPlayerSpawner` is in the scene
   - Verify spawn points are assigned
   - Ensure Player Prefab has `NetworkObject`

3. **Movement not syncing:**
   - Ensure `NetworkTransform` is on player prefab
   - Check that `IsOwner` checks are in place in `PlayerController`

4. **Enemies not moving:**
   - Enemies only run AI on server
   - Check that `IsServer` checks are in `EnemyAI`
   - Verify enemies have `NetworkObject` and are spawned correctly

## Step 9: Additional Features to Consider

1. **Player Health System:**
   - Convert player health to use `NetworkVariable` (similar to `EnemyHealth`)

2. **Synchronized Animations:**
   - Use `NetworkAnimator` component for animation sync

3. **Projectile Synchronization:**
   - Convert enemy projectiles to `NetworkBehaviour`
   - Spawn them with `NetworkObject.Spawn()`

4. **Game State Management:**
   - Create a `NetworkBehaviour` for game state (score, round, etc.)
   - Use `NetworkVariable` for shared state

## Quick Start Checklist

- [ ] NetworkManager GameObject in scene with NetworkManager + UnityTransport
- [ ] Player Prefab has NetworkObject + NetworkTransform + PlayerController
- [ ] Enemy Prefabs have NetworkObject + NetworkTransform + EnemyAI + EnemyHealth
- [ ] NetworkPlayerSpawner in scene with spawn points assigned
- [ ] NetworkManagerUI set up with all UI references
- [ ] All networked prefabs added to NetworkManager's Network Prefabs List
- [ ] Test in Multiplayer Play Mode or with build

Good luck with your multiplayer implementation!

