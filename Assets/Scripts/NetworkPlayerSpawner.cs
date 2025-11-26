using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] playerPrefabs; // Array of player prefabs (index 0 = host, 1 = client, etc.)
    [SerializeField] private GameObject defaultPlayerPrefab; // Fallback if array is empty

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        // Subscribe to client connection events
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        // Spawn the host player immediately
        if (NetworkManager.Singleton.IsHost)
        {
            SpawnPlayer(NetworkManager.Singleton.LocalClientId);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        // Skip host (clientId 0) - it's already spawned in OnNetworkSpawn
        // Only spawn for actual clients connecting
        if (clientId == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.IsHost)
        {
            return;
        }

        // Spawn player for the newly connected client
        SpawnPlayer(clientId);
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        // Use clientId to determine spawn point (0 for host, 1 for first client, etc.)
        int spawnIndex = (int)(clientId % (ulong)spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];

        // Determine which player prefab to use based on clientId
        // Host (clientId 0) gets prefab[0], first client (clientId 1) gets prefab[1], etc.
        GameObject prefabToSpawn = null;
        
        if (playerPrefabs != null && playerPrefabs.Length > 0)
        {
            // Use clientId to index into the prefab array
            int prefabIndex = (int)(clientId % (ulong)playerPrefabs.Length);
            
            // Make sure the prefab at this index exists and is valid
            if (prefabIndex < playerPrefabs.Length && playerPrefabs[prefabIndex] != null)
            {
                prefabToSpawn = playerPrefabs[prefabIndex];
            }
        }
        
        // Fallback to default prefab if array is empty or invalid
        if (prefabToSpawn == null)
        {
            if (defaultPlayerPrefab != null)
            {
                prefabToSpawn = defaultPlayerPrefab;
            }
            else
            {
                // Last resort: use NetworkManager's default player prefab
                prefabToSpawn = NetworkManager.Singleton.NetworkConfig.PlayerPrefab;
            }
        }

        if (prefabToSpawn == null)
        {
            Debug.LogError($"No player prefab assigned for client {clientId}! Please assign player prefabs in NetworkPlayerSpawner.");
            return;
        }

        // Instantiate and spawn the player
        GameObject playerInstance = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();

        if (networkObject != null)
        {
            networkObject.SpawnAsPlayerObject(clientId);
            Debug.Log($"Spawned player '{prefabToSpawn.name}' for client {clientId} at spawn point {spawnIndex}");
        }
        else
        {
            Debug.LogError($"Player prefab {prefabToSpawn.name} does not have a NetworkObject component!");
            Destroy(playerInstance);
        }
    }
}

