using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class EnemyHealth : NetworkBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    
    // Network variable for health synchronization
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    
    [Header("Death Settings")]
    public float deathDelay = 2f;
    public GameObject[] deathEffects;
    public AudioClip deathSound;
    
    [Header("Damage Settings")]
    public AudioClip[] damageSounds;
    public GameObject[] damageEffects;
    
    private bool isDead = false;
    private AudioSource audioSource;
    private Animator animator;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // Initialize health on server
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }
        
        // Subscribe to health changes
        currentHealth.OnValueChanged += OnHealthChanged;
        
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        
        // If no audio source, add one
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
        base.OnNetworkDespawn();
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        // This is called on all clients when health changes
        // You can add visual feedback here if needed
        if (newValue < oldValue && newValue > 0)
        {
            // Health decreased - play damage effects on all clients
            PlayDamageEffects();
        }
    }

    private void PlayDamageEffects()
    {
        // Play damage sound
        if (damageSounds != null && damageSounds.Length > 0)
        {
            AudioClip randomSound = damageSounds[Random.Range(0, damageSounds.Length)];
            if (audioSource != null && randomSound != null)
            {
                audioSource.PlayOneShot(randomSound);
            }
        }
        
        // Spawn damage effects
        if (damageEffects != null)
        {
            foreach (GameObject effect in damageEffects)
            {
                if (effect != null)
                {
                    Instantiate(effect, transform.position, Quaternion.identity);
                }
            }
        }
    }
    
    public void TakeDamage(int damage)
    {
        // Only server can process damage
        if (!IsServer) return;
        if (isDead) return;
        
        int newHealth = currentHealth.Value - damage;
        newHealth = Mathf.Max(0, newHealth);
        currentHealth.Value = newHealth;
        
        // Check if dead
        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        // Only server can process healing
        if (!IsServer) return;
        if (isDead) return;
        
        int newHealth = currentHealth.Value + healAmount;
        newHealth = Mathf.Min(maxHealth, newHealth);
        currentHealth.Value = newHealth;
    }
    
    public void Die()
    {
        // Only server can process death
        if (!IsServer) return;
        if (isDead) return;
        
        isDead = true;
        
        // Play death effects on all clients
        PlayDeathEffectsClientRpc();
        
        // Disable AI and other components
        EnemyAI enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }
        
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }
        
        // Trigger death animation
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
        
        // Despawn after delay (server only)
        StartCoroutine(DespawnAfterDelay());
    }

    [ClientRpc]
    private void PlayDeathEffectsClientRpc()
    {
        // Play death sound
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        
        // Spawn death effects
        if (deathEffects != null)
        {
            foreach (GameObject effect in deathEffects)
            {
                if (effect != null)
                {
                    Instantiate(effect, transform.position, transform.rotation);
                }
            }
        }
    }

    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.IsSpawned)
        {
            networkObject.Despawn(true);
        }
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    public float GetHealthPercentage()
    {
        return (float)currentHealth.Value / maxHealth;
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth.Value;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    // Method to reset health (useful for respawning)
    public void ResetHealth()
    {
        if (!IsServer) return;
        currentHealth.Value = maxHealth;
        isDead = false;
    }
}
