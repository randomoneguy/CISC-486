using UnityEngine;
using UnityEngine.AI;

public class IdleState : EnemyState
{
    public IdleState(EnemyStateMachine stateMachine) : base(stateMachine) { }
    
    public override void Enter()
    {
        Debug.Log("Enemy entering Idle state");
        
        // Stop moving
        agent.ResetPath();
        
        // Set animation
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("MotionSpeed", 0f);
        }
    }
    
    public override void Update()
    {
        if (enemyAI.targetPlayer == null) return;
        
        float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, enemyAI.targetPlayer.position);
        
        // transitions from idle
        // If player moves too far away, start walking
        if (distanceToPlayer > enemyAI.getStoppingDistance() + 1f)
        {
            stateMachine.ChangeState(stateMachine.walkState);
            return;
        }
        
        // If player is in melee range, attack directly (regardless of charge cooldown)
        if (distanceToPlayer <= enemyAI.getMeleeRange() && enemyAI.CanMeleeAttack())
        {
            stateMachine.ChangeState(stateMachine.meleeAttackState);
            return;
        }

        
        // Face the player while idle
        enemyAI.FacePlayer();
    }
    
    public override void Exit()
    {
        Debug.Log("Enemy exiting Idle state");
    }
}

public class WalkState : EnemyState
{
    public WalkState(EnemyStateMachine stateMachine) : base(stateMachine) { }
    
    public override void Enter()
    {
        Debug.Log("Enemy entering Walk state");

    }
    
    public override void Update()
    {
        if (enemyAI.targetPlayer == null) return;
        
        float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, enemyAI.targetPlayer.position);
        
        // Check if we should transition to laser beam state (priority over charge)
        if (distanceToPlayer <= enemyAI.getLaserBeamRange() && enemyAI.CanLaserBeam())
        {
            stateMachine.ChangeState(stateMachine.laserBeamState);
            return;
        }
        
        // Check if we should transition to charge state (only if cooldown is ready and laser beam not available)
        if (distanceToPlayer <= enemyAI.getChargeRange() && enemyAI.CanCharge())
        {
            stateMachine.ChangeState(stateMachine.chargeState);
            return;
        }

        
        // If we're close enough, go to idle, small offset to avoid being stuck just before stopping distance and can't transition
        if (distanceToPlayer <= enemyAI.getStoppingDistance() + 0.15f)
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
        
        // Move towards player but maintain a comfortable distance
        Vector3 directionToPlayer = (enemyAI.targetPlayer.position - stateMachine.transform.position).normalized;
        Vector3 targetPosition = enemyAI.targetPlayer.position - (directionToPlayer * enemyAI.getStoppingDistance());
        agent.SetDestination(targetPosition);
        
        // Smooth rotation during movement
        enemyAI.FaceMovementDirection(agent.velocity);
        
        // Update animation speed
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
    
    public override void Exit()
    {
        Debug.Log("Enemy exiting Walk state");
    }
}

public class ChargeState : EnemyState
{
    private float chargeTimer;
    private bool isCharging;
    
    public ChargeState(EnemyStateMachine stateMachine) : base(stateMachine) { }
    
    public override void Enter()
    {
        Debug.Log("Enemy entering Charge state");
        
        chargeTimer = 0f;
        isCharging = true;
        
        // Increase speed for charging
        agent.speed = enemyAI.getChargeSpeed();
        
        // Face the player
        if (enemyAI.targetPlayer != null)
        {
            enemyAI.StartCharge();
            enemyAI.FacePlayerInstantly();
        }
    }
    
    public override void Update()
    {
        if (!isCharging) return;
        
        chargeTimer += Time.deltaTime;
        
        // Check if we're in melee range during charge - trigger kick attack immediately
        if (enemyAI.targetPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, enemyAI.targetPlayer.position);
            
            if (distanceToPlayer <= enemyAI.getMeleeRange())
            {
                
                // Transition to melee attack state
                stateMachine.ChangeState(stateMachine.meleeAttackState);
                return;
            }
            
            // Move towards player but maintain a comfortable distance
            Vector3 directionToPlayer = (enemyAI.targetPlayer.position - stateMachine.transform.position).normalized;
            Vector3 targetPosition = enemyAI.targetPlayer.position - (directionToPlayer * enemyAI.getStoppingDistance());
            agent.SetDestination(targetPosition);
            
            // Smooth rotation during charge
            enemyAI.FaceMovementDirection(agent.velocity);

            // Update animation speed
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
        
        // Check if charge duration has expired
        if (chargeTimer >= enemyAI.getChargeDuration())
        {
            Debug.Log("Charge duration expired, returning to idle");
            // Charge duration expired, return to idle state
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
    }
    
    public override void Exit()
    {
        Debug.Log("Enemy exiting Charge state");
        
        isCharging = false;
        
        // Reset speed back to normal
        agent.speed = enemyAI.getMoveSpeed();
        
        // Reset charge cooldown
        enemyAI.ResetChargeCooldown();
    }
}

public class MeleeAttackState : EnemyState
{
    private float attackTimer;
    private bool hasAttacked;
    
    public MeleeAttackState(EnemyStateMachine stateMachine) : base(stateMachine) { }
    
    public override void Enter()
    {
        Debug.Log("Enemy entering Melee Attack state");
        
        attackTimer = 0f;
        
        // Stop moving
        agent.ResetPath();
        
        // Face the player
        if (enemyAI.targetPlayer != null)
        {
            enemyAI.StartMeleeAttack();
            enemyAI.FacePlayerInstantly();
            PerformMeleeAttack();
        }
    }
    
    public override void Update()
    {
        attackTimer += Time.deltaTime;
        
        // Check if attack animation is complete
        if (attackTimer >= enemyAI.getMeleeAttackDuration())
        {
            // Return to idle state
            stateMachine.ChangeState(stateMachine.idleState);
        }
    }
    
    private void PerformMeleeAttack()
    {
        if (enemyAI.targetPlayer == null) return;
        
        float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, enemyAI.targetPlayer.position);
        Debug.Log("distanceToPlayer: " + distanceToPlayer);
        
        if (distanceToPlayer <= enemyAI.getMeleeRange())
        {
            // Set animation
            if (animator != null)
            {
                animator.SetTrigger("MeleeAttack");
            }
            // Deal damage to player
            /*
            PlayerHealth playerHealth = enemyAI.player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(enemyAI.meleeDamage);
            }
            */
            Debug.Log("Enemy performing Melee Attack");
            
            // Spawn attack effect
            if (enemyAI.meleeAttackEffect != null)
            {
                UnityEngine.Object.Instantiate(enemyAI.meleeAttackEffect, stateMachine.transform.position + stateMachine.transform.forward * enemyAI.getMeleeRange(), stateMachine.transform.rotation);
            }
        }
    }
    
    public override void Exit()
    {
        Debug.Log("Enemy exiting Melee Attack state");
        
        enemyAI.ResetMeleeAttackCooldown();
    }
}

public class LaserBeamState : EnemyState
{
    private float beamTimer;
    private bool hasStartedBeam;
    private LineRenderer laserLine;
    private Vector3 currentDirection; // Current beam direction
    private float trackingSpeed = 2f; // How fast the beam tracks the player
    
    public LaserBeamState(EnemyStateMachine stateMachine) : base(stateMachine) { }
    
    public override void Enter()
    {
        Debug.Log("Enemy entering Laser Beam state");
        
        beamTimer = 0f;
        hasStartedBeam = false;
        
        // Stop moving and disable agent
        agent.ResetPath();
        agent.enabled = false;
        
        // Face the player instantly before starting beam
        if (enemyAI.targetPlayer != null)
        {
            enemyAI.StartLaserBeam();
            enemyAI.FacePlayerInstantly();
            
            // Initialize the current beam direction
            Vector3 startPos = stateMachine.transform.position + Vector3.up * 1.5f;
            currentDirection = (enemyAI.targetPlayer.position - startPos).normalized;
            currentDirection.y = 0; // Keep it horizontal
            currentDirection = currentDirection.normalized;
        }
        
        // Create laser line renderer
        CreateLaserLine();

        // Set animation
        /*
        if (animator != null)
        {
            animator.SetTrigger("LaserBeam");
        }
        */
    }
    
    public override void Update()
    {
        beamTimer += Time.deltaTime;
        
        // Start the laser beam after a brief delay
        if (!hasStartedBeam && beamTimer >= enemyAI.getLaserBeamStateBuffer())
        {
            StartLaserBeam();
            hasStartedBeam = true;
        }
        
        // Continue the laser beam
        if (hasStartedBeam && beamTimer < enemyAI.getLaserBeamDuration())
        {
            UpdateLaserBeam();
        }
        else if (hasStartedBeam && beamTimer >= enemyAI.getLaserBeamDuration()) {
            // Destroy laser line
            if (laserLine != null)
            {
                UnityEngine.Object.Destroy(laserLine.gameObject);
            }
        }
        
        // Check if state is complete
        if (beamTimer >= enemyAI.getLaserBeamDuration() + enemyAI.getLaserBeamStateBuffer())
        {
            // Return to idle state
            stateMachine.ChangeState(stateMachine.idleState);
        }
    }
    
    private void CreateLaserLine()
    {
        // Create a LineRenderer for the laser beam
        GameObject laserObj = new GameObject("LaserBeam");
        laserObj.transform.SetParent(stateMachine.transform);
        laserLine = laserObj.AddComponent<LineRenderer>();
        
        // Configure the line renderer
        laserLine.material = new Material(Shader.Find("Sprites/Default"));
        laserLine.material.color = Color.red;
        laserLine.startWidth = enemyAI.getLaserBeamWidth();
        laserLine.endWidth = enemyAI.getLaserBeamWidth();
        laserLine.positionCount = 2;
        laserLine.enabled = false; // Start disabled
    }
    
    private void StartLaserBeam()
    {
        if (laserLine != null)
        {
            laserLine.enabled = true;
        }
    }
    
    private void UpdateLaserBeam()
    {
        if (laserLine == null) return;
        
        // Update beam direction to slowly follow player
        UpdateBeamDirection();
        
        // Calculate laser positions
        Vector3 startPos = stateMachine.transform.position + Vector3.up * 1.5f; // Eye level
        Vector3 endPos = startPos + currentDirection * enemyAI.getLaserBeamRange();
        
        // Set laser line positions
        laserLine.SetPosition(0, startPos);
        laserLine.SetPosition(1, endPos);
        
        // Deal damage to player if in laser path
        DealLaserDamage(startPos, currentDirection);
    }
    
    private void UpdateBeamDirection()
    {
        if (enemyAI.targetPlayer == null) return;
        
        // Calculate direction to player
        Vector3 startPos = stateMachine.transform.position + Vector3.up * 1.5f;
        Vector3 toPlayer = (enemyAI.targetPlayer.position - startPos).normalized;
        toPlayer.y = 0; // Keep it horizontal
        toPlayer = toPlayer.normalized;
        
        // Smoothly rotate current direction towards player
        currentDirection = Vector3.Slerp(currentDirection, toPlayer, trackingSpeed * Time.deltaTime);
        currentDirection.y = 0; // Ensure it stays horizontal
        currentDirection = currentDirection.normalized;
        
        // Rotate the enemy model to face the beam direction
        if (currentDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
            stateMachine.transform.rotation = Quaternion.Slerp(
                stateMachine.transform.rotation, 
                targetRotation, 
                trackingSpeed * Time.deltaTime
            );
        }
    }
    
    private void DealLaserDamage(Vector3 startPos, Vector3 direction)
    {
        // Raycast to check for player in laser path
        RaycastHit[] hits = Physics.RaycastAll(startPos, direction, enemyAI.getLaserBeamRange());
        
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                /* 
                // Deal damage to player
                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(enemyAI.getLaserBeamDamage());
                }
                */
                break; // Only damage once per frame
            }
        }
    }
    
    public override void Exit()
    {
        Debug.Log("Enemy exiting Laser Beam state");
        
        // Re-enable agent
        agent.enabled = true;
        
        // Reset cooldown
        enemyAI.ResetLaserBeamCooldown();
    }
}

public class RangeAttackState : EnemyState
{
    private float attackTimer;
    private bool hasAttacked;
    
    public RangeAttackState(EnemyStateMachine stateMachine) : base(stateMachine) { }
    
    public override void Enter()
    {
        Debug.Log("Enemy entering Range Attack state");
        
        attackTimer = 0f;
        hasAttacked = false;
        
        // Stop moving
        agent.ResetPath();
        
        // Face the player
        if (enemyAI.targetPlayer != null)
        {
            enemyAI.FacePlayerInstantly();
        }
        
        // Set animation
        if (animator != null)
        {
            animator.SetTrigger("RangeAttack");
        }
    }
    
    public override void Update()
    {
        attackTimer += Time.deltaTime;
        
        // Perform attack at the right moment
        if (!hasAttacked && attackTimer >= enemyAI.getMeleeAttackDelay())
        {
            PerformRangeAttack();
            hasAttacked = true;
        }
        
        // Check if attack animation is complete
        if (attackTimer >= enemyAI.getRangeAttackDuration())
        {
            // Return to idle state
            stateMachine.ChangeState(stateMachine.idleState);
        }
    }
    
    private void PerformRangeAttack()
    {
        if (enemyAI.targetPlayer == null) return;
        
        // Create projectile
        if (enemyAI.projectilePrefab != null)
        {
            Vector3 spawnPosition = stateMachine.transform.position + stateMachine.transform.forward * 1f + Vector3.up * 1f;
            GameObject projectile = UnityEngine.Object.Instantiate(enemyAI.projectilePrefab, spawnPosition, stateMachine.transform.rotation);
            
            // Set up projectile
            EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
            if (projectileScript != null)
            {
                projectileScript.SetTarget(enemyAI.targetPlayer);
                projectileScript.SetDamage(enemyAI.getRangeDamage());
                projectileScript.SetSpeed(enemyAI.getProjectileSpeed());
            }
        }
        
        // Spawn attack effect
        if (enemyAI.rangeAttackEffect != null)
        {
            UnityEngine.Object.Instantiate(enemyAI.rangeAttackEffect, stateMachine.transform.position + stateMachine.transform.forward * 1f, stateMachine.transform.rotation);
        }
    }
    
    public override void Exit()
    {
        Debug.Log("Enemy exiting Range Attack state");
    }
}
