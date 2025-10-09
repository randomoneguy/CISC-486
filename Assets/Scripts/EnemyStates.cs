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
        if (enemyAI.player == null) return;
        
        float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, enemyAI.player.position);
        
        // transitions from idle
        // If player moves too far away, start walking
        if (distanceToPlayer > enemyAI.stoppingDistance + 1f)
        {
            stateMachine.ChangeState(stateMachine.walkState);
            return;
        }
        
        // If player is in melee range, attack directly (regardless of charge cooldown)
        if (distanceToPlayer <= enemyAI.meleeRange && enemyAI.CanMeleeAttack())
        {
            stateMachine.ChangeState(stateMachine.meleeAttackState);
            return;
        }

        
        // Face the player while idle
        Vector3 direction = (enemyAI.player.position - stateMachine.transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            stateMachine.transform.rotation = Quaternion.Slerp(
                stateMachine.transform.rotation, 
                targetRotation, 
                enemyAI.rotationSpeed * Time.deltaTime
            );
        }
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
        if (enemyAI.player == null) return;
        
        float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, enemyAI.player.position);
        
        // Check if we should transition to charge state (only if cooldown is ready)
        if (distanceToPlayer <= enemyAI.chargeRange && enemyAI.CanCharge())
        {
            stateMachine.ChangeState(stateMachine.chargeState);
            return;
        }

        
        // If we're close enough, go to idle, small offset to avoid being stuck just before stopping distance and can't transition
        if (distanceToPlayer <= enemyAI.stoppingDistance + 0.15f)
        {
            stateMachine.ChangeState(stateMachine.idleState);
            return;
        }
        
        // Move towards player but maintain a comfortable distance
        Vector3 directionToPlayer = (enemyAI.player.position - stateMachine.transform.position).normalized;
        Vector3 targetPosition = enemyAI.player.position - (directionToPlayer * enemyAI.stoppingDistance);
        agent.SetDestination(targetPosition);
        
        // Smooth rotation during movement
        if (agent.velocity.magnitude > 0.1f)
        {
            Vector3 moveDirection = agent.velocity.normalized;
            moveDirection.y = 0; // Keep rotation on horizontal plane
            
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                stateMachine.transform.rotation = Quaternion.Slerp(
                    stateMachine.transform.rotation, 
                    targetRotation, 
                    enemyAI.rotationSpeed * Time.deltaTime
                );
            }
        }
        
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
        agent.speed = enemyAI.chargeSpeed;
        
        // Face the player
        if (enemyAI.player != null)
        {
            enemyAI.StartCharge();
            Vector3 direction = (enemyAI.player.position - stateMachine.transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                stateMachine.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
    
    public override void Update()
    {
        if (!isCharging) return;
        
        chargeTimer += Time.deltaTime;
        
        // Check if we're in melee range during charge - trigger kick attack immediately
        if (enemyAI.player != null)
        {
            float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, enemyAI.player.position);
            
            if (distanceToPlayer <= enemyAI.meleeRange)
            {
                // Stop moving and face player
                agent.ResetPath();
                
                // Face the player
                Vector3 direction = (enemyAI.player.position - stateMachine.transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    stateMachine.transform.rotation = Quaternion.LookRotation(direction);
                }
                
                // Transition to melee attack state
                stateMachine.ChangeState(stateMachine.meleeAttackState);
                return;
            }
            
            // Move towards player but maintain a comfortable distance
            Vector3 directionToPlayer = (enemyAI.player.position - stateMachine.transform.position).normalized;
            Vector3 targetPosition = enemyAI.player.position - (directionToPlayer * enemyAI.stoppingDistance);
            agent.SetDestination(targetPosition);
            
            // Smooth rotation during charge
            if (agent.velocity.magnitude > 0.1f)
            {
                Vector3 moveDirection = agent.velocity.normalized;
                moveDirection.y = 0; // Keep rotation on horizontal plane
                
                if (moveDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    stateMachine.transform.rotation = Quaternion.Slerp(
                        stateMachine.transform.rotation, 
                        targetRotation, 
                        enemyAI.rotationSpeed * Time.deltaTime
                    );
                }
            }

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
        
        // Check if charge is complete
        /*
        if (chargeTimer >= enemyAI.chargeDuration)
        {
            // Decide between melee or range attack based on distance
            float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, enemyAI.player.position);
            
            if (distanceToPlayer <= enemyAI.meleeRange)
            {
                stateMachine.ChangeState(stateMachine.meleeAttackState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.rangeAttackState);
            }
        }
        */
    }
    
    public override void Exit()
    {
        Debug.Log("Enemy exiting Charge state");
        
        isCharging = false;
        
        // Reset speed back to normal
        agent.speed = enemyAI.moveSpeed;
        
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
        if (enemyAI.player != null)
        {
            enemyAI.StartMeleeAttack();
            Vector3 direction = (enemyAI.player.position - stateMachine.transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                stateMachine.transform.rotation = Quaternion.LookRotation(direction);
            }

            PerformMeleeAttack();
        }
    }
    
    public override void Update()
    {
        attackTimer += Time.deltaTime;
        
        // Check if attack animation is complete
        if (attackTimer >= enemyAI.meleeAttackDuration)
        {
            // Return to idle state
            stateMachine.ChangeState(stateMachine.idleState);
        }
    }
    
    private void PerformMeleeAttack()
    {
        if (enemyAI.player == null) return;
        
        float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, enemyAI.player.position);
        Debug.Log("distanceToPlayer: " + distanceToPlayer);
        
        if (distanceToPlayer <= enemyAI.meleeRange)
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
                UnityEngine.Object.Instantiate(enemyAI.meleeAttackEffect, stateMachine.transform.position + stateMachine.transform.forward * enemyAI.meleeRange, stateMachine.transform.rotation);
            }
        }
    }
    
    public override void Exit()
    {
        Debug.Log("Enemy exiting Melee Attack state");
        
        enemyAI.ResetMeleeAttackCooldown();
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
        if (enemyAI.player != null)
        {
            Vector3 direction = (enemyAI.player.position - stateMachine.transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                stateMachine.transform.rotation = Quaternion.LookRotation(direction);
            }
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
        if (!hasAttacked && attackTimer >= enemyAI.meleeAttackDelay)
        {
            PerformRangeAttack();
            hasAttacked = true;
        }
        
        // Check if attack animation is complete
        if (attackTimer >= enemyAI.rangeAttackDuration)
        {
            // Return to idle state
            stateMachine.ChangeState(stateMachine.idleState);
        }
    }
    
    private void PerformRangeAttack()
    {
        if (enemyAI.player == null) return;
        
        // Create projectile
        if (enemyAI.projectilePrefab != null)
        {
            Vector3 spawnPosition = stateMachine.transform.position + stateMachine.transform.forward * 1f + Vector3.up * 1f;
            GameObject projectile = UnityEngine.Object.Instantiate(enemyAI.projectilePrefab, spawnPosition, stateMachine.transform.rotation);
            
            // Set up projectile
            EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
            if (projectileScript != null)
            {
                projectileScript.SetTarget(enemyAI.player);
                projectileScript.SetDamage(enemyAI.rangeDamage);
                projectileScript.SetSpeed(enemyAI.projectileSpeed);
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
