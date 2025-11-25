# Setting Up Different Player Models for Host and Client

This guide shows you how to have the host and client spawn with different character models/prefabs.

## Step 1: Create Your Player Prefabs

1. **Create Prefab Variants:**
   - Since your model prefabs can't have components directly, create prefab variants:
   - Drag your model prefab into the scene
   - Add all required components:
     - `NetworkObject`
     - `NetworkTransform`
     - `PlayerController`
     - `PlayerStateMachine`
     - `Rigidbody`
     - `Animator`
     - Any other components your player needs
   - Drag the GameObject back to Project to create a prefab variant
   - Name it something like "PlayerHost" or "PlayerMarisa"

2. **Repeat for Second Model:**
   - Do the same for your second character model
   - Name it "PlayerClient" or "PlayerReimu"
   - Make sure both prefabs have identical components (just different models)

## Step 2: Add Prefabs to Network Prefabs List

1. **In NetworkManager:**
   - Select your NetworkManager GameObject
   - In NetworkManager component, find "Network Prefabs List"
   - Add both player prefabs to the list
   - This allows them to be spawned over the network

## Step 3: Configure NetworkPlayerSpawner

1. **Set Up Player Prefabs Array:**
   - Select the GameObject with `NetworkPlayerSpawner` component
   - In the Inspector, find "Player Prefabs" array
   - Set the array size to 2 (or however many different models you want)
   - Assign:
     - Element 0: Your host player prefab (e.g., "PlayerHost" or "PlayerMarisa")
     - Element 1: Your client player prefab (e.g., "PlayerClient" or "PlayerReimu")

2. **Optional - Set Default Prefab:**
   - You can also set a "Default Player Prefab" as a fallback
   - This will be used if the array is empty or invalid

## Step 4: Disable Auto-Spawn (Important!)

1. **In NetworkManager:**
   - Select NetworkManager GameObject
   - In NetworkManager component, expand "Network Config"
   - Find "Auto Spawn Player Prefab Client Side"
   - **Uncheck this** (set to false)
   - This prevents NetworkManager from automatically spawning the default player prefab
   - Your NetworkPlayerSpawner will handle all player spawning instead

## How It Works

- **Host (clientId = 0):** Spawns with `playerPrefabs[0]` (first prefab in array)
- **Client (clientId = 1):** Spawns with `playerPrefabs[1]` (second prefab in array)
- **Additional clients:** Will cycle through the array (clientId 2 gets prefab[0], clientId 3 gets prefab[1], etc.)

## Example Setup

```
NetworkPlayerSpawner:
├── Player Prefabs (Array):
│   ├── [0] PlayerMarisa (Host model)
│   └── [1] PlayerReimu (Client model)
├── Default Player Prefab: (optional fallback)
└── Spawn Points: [SpawnPoint1, SpawnPoint2]
```

## Testing

1. **Host:**
   - Click "Host" button
   - Should spawn with the first prefab (PlayerMarisa)

2. **Client:**
   - Click "Client" button
   - Should spawn with the second prefab (PlayerReimu)

3. **Verify:**
   - Both players should see each other with different models
   - Both should be able to move independently
   - Both should have all their components working

## Troubleshooting

**Both players have the same model:**
- Check that Player Prefabs array has different prefabs assigned
- Verify both prefabs are in Network Prefabs List
- Make sure Auto Spawn is disabled

**Client doesn't spawn:**
- Check NetworkPlayerSpawner is in the scene
- Verify spawn points are assigned
- Check console for error messages

**Models don't have components:**
- Make sure you created prefab variants, not editing the original model prefab
- Verify all required components are on both prefabs

**Only one player moves:**
- This is normal if testing in same editor instance
- Use ParrelSync or build to test with separate processes
- Each process needs its own input to control its player

---

That's it! Now your host and client will have different character models!

