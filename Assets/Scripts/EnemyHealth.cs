using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    
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
    
    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        
        // If no audio source, add one
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
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
        
        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        if (isDead) return;
        
        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
    }
    
    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
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
        
        // Destroy after delay
        Destroy(gameObject, deathDelay);
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    // Method to reset health (useful for respawning)
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
    }
}
