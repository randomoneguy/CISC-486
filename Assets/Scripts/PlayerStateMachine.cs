using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected PlayerController playerController;
    protected Rigidbody rb;
    protected Animator animator;
    protected Collider[] atkHitboxes;
    
    public PlayerState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.playerController = stateMachine.playerController;
        this.rb = stateMachine.rb;
        this.animator = stateMachine.animator;
        this.atkHitboxes = stateMachine.atkHitboxes;
    }
    
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}

public class PlayerStateMachine : MonoBehaviour
{
    [Header("State Machine Settings")]
    public PlayerState currentState;
    public PlayerState previousState;
    
    [Header("References")]
    public PlayerController playerController;
    public Rigidbody rb;
    public Animator animator;
    public Collider[] atkHitboxes;
    
    // State instances
    public PlayerIdleState idleState;
    public PlayerAttack1State attack1State;
    public PlayerAttack2State attack2State;
    public PlayerAttack3State attack3State;
    
    void Start()
    {
        // Get components
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        
        // Initialize states
        idleState = new PlayerIdleState(this);
        attack1State = new PlayerAttack1State(this);
        attack2State = new PlayerAttack2State(this);
        attack3State = new PlayerAttack3State(this);
        
        // Start with idle state
        ChangeState(idleState);
    }
    
    void Update()
    {
        currentState?.Update();
    }
    
    public void ChangeState(PlayerState newState)
    {
        if (currentState != null)
        {
            previousState = currentState;
            currentState.Exit();
        }
        
        currentState = newState;
        currentState.Enter();
    }
    
    public void ReturnToPreviousState()
    {
        if (previousState != null)
        {
            ChangeState(previousState);
        }
    }
}

