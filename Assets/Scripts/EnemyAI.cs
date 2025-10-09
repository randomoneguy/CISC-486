using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float chargeRange = 20f;
    public float meleeRange = 2f;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float chargeSpeed = 6f; // How much faster the enemy moves during charge
    public float chargeCooldown = 10f; // Cooldown time before enemy can charge again
    public float stoppingDistance = 0.5f; // How close to get before stopping to attack
    
    [Header("Attack Settings")]
    //public float chargeDuration = 1f;
    public float meleeAttackDelay = 1.5f;
    public float meleeAttackDuration = 1.5f;
    public float rangeAttackDuration = 2f;
    public int meleeDamage = 15;
    public int rangeDamage = 10;
    
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    
    [Header("Effects")]
    public GameObject meleeAttackEffect;
    public GameObject rangeAttackEffect;
    
    // Public references
    public Transform player;
    public EnemyStateMachine stateMachine;
    
    // Private components
    private NavMeshAgent agent;
    private Animator animator;
    private EnemyHealth enemyHealth;
    
    // Charge cooldown tracking
    private float lastChargeTime = -10f;
    private bool canCharge = true;
    private bool canMeleeAttack = true;
    private float lastMeleeAttackTime = -2f;
    
    void Start()
    {
        // Get components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        stateMachine = GetComponent<EnemyStateMachine>();
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        
        // Set up agent
        agent.speed = moveSpeed;
        agent.angularSpeed = rotationSpeed;
        agent.updateRotation = false; // Disable automatic rotation so we can control it manually
        
        // Set up state machine
        if (stateMachine != null)
        {
            stateMachine.enemyAI = this;
        }
    }
    
    void Update()
    {
        // Check if enemy is dead
        if (enemyHealth != null && enemyHealth.IsDead())
        {
            // Handle death state here if needed
            return;
        }
        
        // Update animator speed
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            
            // Calculate MotionSpeed based on movement direction and magnitude (like ThirdPersonController)
            Vector3 direction = agent.velocity;

            // Calculate the magnitude of the direction
            float motionSpeed = true ? direction.magnitude : 1f;
            animator.SetFloat("MotionSpeed", motionSpeed);
            
        }
    }
    
    // Public method to take damage (called by other scripts)
    public void TakeDamage(int damage)
    {
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
    }
    
    // Method to check if player is in range
    public bool IsPlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) <= range;
    }
    
    // Method to get distance to player
    public float GetDistanceToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Vector3.Distance(transform.position, player.position);
    }
    
    // Method to check if enemy can charge
    public bool CanCharge()
    {
        return canCharge && (Time.time - lastChargeTime >= chargeCooldown);
    }
    
    // Method to start charge (called when entering charge state)
    public void StartCharge()
    {
        canCharge = false;
    }
    
    // Method to reset charge cooldown (called when exiting charge state)
    public void ResetChargeCooldown()
    {
        lastChargeTime = Time.time;
        canCharge = true;
    }
    
    public bool CanMeleeAttack()
    {
        return canMeleeAttack && (Time.time - lastMeleeAttackTime >= meleeAttackDelay);
    }

    public void ResetMeleeAttackCooldown()
    {
        lastMeleeAttackTime = Time.time;
        canMeleeAttack = true;
        animator.ResetTrigger("MeleeAttack");
    }

    public void StartMeleeAttack()
    {
        canMeleeAttack = false;
    }

    // Animation event handlers (to prevent missing receiver errors)
    private void OnFootstep(AnimationEvent animationEvent)
    {
        // Handle footstep sound if needed
        // This prevents the "no receiver" error
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        // Handle landing sound if needed
        // This prevents the "no receiver" error
    }

    // Draw gizmos for debugging
    void OnDrawGizmosSelected()
    {
        // Charge range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chargeRange);
        
        // Melee range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        
        // Player direction
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
