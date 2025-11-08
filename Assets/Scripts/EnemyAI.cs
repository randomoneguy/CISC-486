using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float chargeRange = 20f;
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float chargeSpeed = 6f; // How much faster the enemy moves during charge
    [SerializeField] private float chargeCooldown = 10f; // Cooldown time before enemy can charge again
    [SerializeField] private float stoppingDistance = 0.5f; // How close to get before stopping to attack
    
    [Header("Attack Settings")]
    [SerializeField] private float chargeDuration = 1.5f; // How long the enemy charges before giving up
    [SerializeField] private float meleeAttackDelay = 1.5f;
    [SerializeField] private float meleeAttackDuration = 1.5f;
    [SerializeField] private float rangeAttackDuration = 2f;
    [SerializeField] private int meleeDamage = 15;
    [SerializeField] private int rangeDamage = 10;
    
    [Header("Laser Beam Settings")]
    [SerializeField] private float laserBeamRange = 15f;
    [SerializeField] private float laserBeamDuration = 3f;
    [SerializeField] private float laserBeamCooldown = 15f;
    [SerializeField] private int laserBeamDamage = 25;
    [SerializeField] private float laserBeamWidth = 0.5f;
    [SerializeField] private float laserBeamStateBuffer = 1.5f;
    
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    
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
    
    // Laser beam cooldown tracking
    private bool canLaserBeam = true;
    private float lastLaserBeamTime = -12f;
    
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

    public float getStoppingDistance()
    {
        return stoppingDistance;
    }

    public float getMeleeRange()
    {
        return meleeRange;
    }

    public float getChargeRange()
    {
        return chargeRange;
    }

    public float getChargeSpeed()
    {
        return chargeSpeed;
    }
    
    public float getChargeDuration()
    {
        return chargeDuration;
    }

    public float getMoveSpeed()
    {
        return moveSpeed;
    }

    public float getMeleeAttackDuration()
    {
        return meleeAttackDuration;
    }

    public float getMeleeAttackDelay()
    {
        return meleeAttackDelay;
    }

    public float getRangeAttackDuration()
    {
        return rangeAttackDuration;
    }

    public int getRangeDamage()
    {
        return rangeDamage;
    }

    public float getProjectileSpeed()
    {
        return projectileSpeed;
    }
    
    // Laser beam getters
    public float getLaserBeamRange()
    {
        return laserBeamRange;
    }
    
    public float getLaserBeamDuration()
    {
        return laserBeamDuration;
    }
    
    public float getLaserBeamCooldown()
    {
        return laserBeamCooldown;
    }
    
    public int getLaserBeamDamage()
    {
        return laserBeamDamage;
    }
    
    public float getLaserBeamWidth()
    {
        return laserBeamWidth;
    }

    public float getLaserBeamStateBuffer()
    {
        return laserBeamStateBuffer;
    }
    
    // Laser beam cooldown methods
    public bool CanLaserBeam()
    {
        return canLaserBeam && (Time.time - lastLaserBeamTime >= laserBeamCooldown);
    }
    
    public void StartLaserBeam()
    {
        canLaserBeam = false;
    }
    
    public void ResetLaserBeamCooldown()
    {
        lastLaserBeamTime = Time.time;
        canLaserBeam = true;
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
    
    // Generalized method to face the player
    public void FacePlayer()
    {
        if (player == null) return;
        
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Keep rotation on horizontal plane
        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
    
    // Generalized method to face player instantly (no smooth rotation)
    public void FacePlayerInstantly()
    {
        if (player == null) return;
        
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Keep rotation on horizontal plane
        
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    
    // Generalized method to face movement direction (for walking/charging)
    public void FaceMovementDirection(Vector3 velocity)
    {
        if (velocity.magnitude > 0.1f)
        {
            Vector3 moveDirection = velocity.normalized;
            moveDirection.y = 0; // Keep rotation on horizontal plane
            
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.deltaTime
                );
            }
        }
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
