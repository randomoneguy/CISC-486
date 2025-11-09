using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    
    public override void Enter()
    {
        // Reset any attack-related animator parameters
        if (animator != null)
        {
            animator.SetInteger("AttackCombo", 0);
            animator.ResetTrigger("Attack");
        }
    }
    
    public override void Update()
    {
        // Attack input is handled in PlayerController
        // May want to move logic here
    }
}

// Base class for attack states with common functionality
public abstract class PlayerAttackStateBase : PlayerState
{
    protected string attackStateName;
    protected int comboNumber;
    // Time are normalized
    protected float comboWindowStart = 0.3f;
    protected float comboWindowEnd = 0.8f;
    protected float movementResumeTime = 0.9f;
    protected bool animationStarted = false;
    
    public PlayerAttackStateBase(PlayerStateMachine stateMachine, string stateName, int comboNum) : base(stateMachine)
    {
        attackStateName = stateName;
        comboNumber = comboNum;
    }
    
    // Check if the animation has actually started playing
    protected bool IsAnimationPlaying()
    {
        if (animator == null) return false;
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(attackStateName);
    }
    
    // Check if we're currently in the combo window
    public bool IsInComboWindow()
    {
        if (animator == null || !animationStarted) return false;
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        // Check if we're in the correct attack state
        if (!stateInfo.IsName(attackStateName))
            return false;
        
        float normalizedTime = stateInfo.normalizedTime % 1.0f;
        return normalizedTime >= comboWindowStart && normalizedTime <= comboWindowEnd;
    }
    
    // Check if the attack animation has progressed far enough to allow movement/interruptions
    public bool CanResumeMovement()
    {
        if (animator == null || !animationStarted) return false;
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if (!stateInfo.IsName(attackStateName))
            return true;
        
        float normalizedTime = stateInfo.normalizedTime % 1.0f;
        return normalizedTime >= movementResumeTime;
    }
    
    // Check if the attack animation is complete
    public bool IsAnimationComplete()
    {
        if (animator == null) return false;
        
        // Wait for animation to start before checking completion
        if (!animationStarted)
            return false;
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        // If we're no longer in the attack state, animation is complete (do we need to add buffer time for rest animations?)
        if (!stateInfo.IsName(attackStateName))
            return true;
        
        // Animation is complete when normalized time >= 1.0
        return stateInfo.normalizedTime >= 1.0f;
    }

    public void attackHit(Collider col)
    {
        Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("Enemy"));
        foreach (Collider c in cols){
            Debug.Log(c.name);
        }
    }
}

public class PlayerAttack1State : PlayerAttackStateBase
{
    private bool comboQueued = false;
    
    public PlayerAttack1State(PlayerStateMachine stateMachine) 
        : base(stateMachine, "Attack1", 1) 
    {
        /* Customize timing for attack 1 if needed
        comboWindowStart = 0.3f;
        comboWindowEnd = 0.8f;
        movementResumeTime = 0.9f;
        */
    }
    
    public override void Enter()
    {
        animationStarted = false;
        comboQueued = false;
        
        // Trigger attack 1 animation
        if (animator != null)
        {
            Debug.Log("trigger anim 1");
            animator.SetInteger("AttackCombo", comboNumber);
            animator.SetTrigger("Attack");
        }
    }
    
    public override void Update()
    {
        // Wait for animation to actually start playing
        if (!animationStarted)
        {
            if (IsAnimationPlaying())
            {
                animationStarted = true;
                Debug.Log("Attack 1 animation started");
            }
            else
            {
                return;
            }
        }

        attackHit(atkHitboxes[0]);
        
        // Check if attack input was pressed during combo window (queue it)
        if (IsInComboWindow() && stateMachine.playerController.IsAttackPressed())
        {
            comboQueued = true;
            Debug.Log("Combo queued for attack 2");
        }
        
        // Check if movement input is pressed and movement can resume (allows interruption after attack completes)
        // This allows walking after the attack animation finishes
        if (CanResumeMovement() && stateMachine.playerController.HasMovementInput())
        {
            // Movement will be handled by PlayerController, but we can transition to idle
            // The movement system will handle the transition naturally
        }
        
        // Check if attack animation is complete
        if (IsAnimationComplete())
        {
            Debug.Log("attack 1 complete");
            
            // If combo was queued, transition to next attack
            if (comboQueued)
            {
                stateMachine.ChangeState(stateMachine.attack2State);
            }
            else
            {
                // Return to idle state
                stateMachine.ChangeState(stateMachine.idleState);
            }
        }
    }
    
    public override void Exit()
    {
    }
}

public class PlayerAttack2State : PlayerAttackStateBase
{
    private bool comboQueued = false; // Track if next attack in combo was queued
    
    public PlayerAttack2State(PlayerStateMachine stateMachine) 
        : base(stateMachine, "Attack2", 2) 
    {
        // Customize timing for attack 2 if needed
        //comboWindowStart = 0.3f;
        comboWindowEnd = 0.89f;
        //movementResumeTime = 0.7f;
    }
    
    public override void Enter()
    {
        animationStarted = false;
        comboQueued = false;
        
        // Trigger attack 2 animation
        if (animator != null)
        {
            animator.SetInteger("AttackCombo", comboNumber);
            animator.SetTrigger("Attack");
        }
    }
    
    public override void Update()
    {
        // Wait for animation to actually start playing
        if (!animationStarted)
        {
            if (IsAnimationPlaying())
            {
                animationStarted = true;
            }
            else
            {
                return;
            }
        }

        attackHit(atkHitboxes[1]);

        // Check if attack input was pressed during combo window (queue it, don't transition immediately)
        if (IsInComboWindow() && stateMachine.playerController.IsAttackPressed())
        {
            comboQueued = true;
        }
        
        // Check if movement input is pressed and movement can resume (allows interruption after attack completes)
        if (CanResumeMovement() && stateMachine.playerController.HasMovementInput())
        {
            // Movement will be handled by PlayerController
        }
        
        // Check if attack animation is complete
        if (IsAnimationComplete())
        {
            // If combo was queued, transition to next attack
            if (comboQueued)
            {
                stateMachine.ChangeState(stateMachine.attack3State);
            }
            else
            {
                // Return to idle state
                stateMachine.ChangeState(stateMachine.idleState);
            }
        }
    }
    
    public override void Exit()
    {
    }
}

public class PlayerAttack3State : PlayerAttackStateBase
{
    public PlayerAttack3State(PlayerStateMachine stateMachine) 
        : base(stateMachine, "Attack3", 3) 
    {
        // No combo window for final attack, but can still resume movement
        // Set over 1f so combo window is always closed
        comboWindowStart = 1.0f;
        comboWindowEnd = 1.0f;
        movementResumeTime = 0.99f;
    }
    
    public override void Enter()
    {
        animationStarted = false;
        
        // Trigger attack 3 animation (final attack in combo)
        if (animator != null)
        {
            animator.SetInteger("AttackCombo", comboNumber);
            animator.SetTrigger("Attack");
        }
    }
    
    public override void Update()
    {
        // Wait for animation to actually start playing
        if (!animationStarted)
        {
            if (IsAnimationPlaying())
            {
                animationStarted = true;
            }
            else
            {
                return;
            }
        }

        attackHit(atkHitboxes[2]);

        // No combo continuation from attack 3, just wait for completion
        // Check if attack animation is complete
        if (IsAnimationComplete())
        {
            // Return to idle state (combo complete)
            stateMachine.ChangeState(stateMachine.idleState);
        }
    }
    
    public override void Exit()
    {
    }
}

