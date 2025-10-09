using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState
{
    protected EnemyStateMachine stateMachine;
    protected EnemyAI enemyAI;
    protected NavMeshAgent agent;
    protected Animator animator;
    
    public EnemyState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.enemyAI = stateMachine.enemyAI;
        this.agent = stateMachine.agent;
        this.animator = stateMachine.animator;
    }
    
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
    public virtual void OnTriggerEnter(Collider other) { }
    public virtual void OnTriggerExit(Collider other) { }
}

public class EnemyStateMachine : MonoBehaviour
{
    [Header("State Machine Settings")]
    public EnemyState currentState;
    public EnemyState previousState;
    
    [Header("References")]
    public EnemyAI enemyAI;
    public NavMeshAgent agent;
    public Animator animator;
    
    // State instances
    public IdleState idleState;
    public WalkState walkState;
    public ChargeState chargeState;
    public MeleeAttackState meleeAttackState;
    public RangeAttackState rangeAttackState;
    
    void Start()
    {
        // Get components
        enemyAI = GetComponent<EnemyAI>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        // Initialize states
        idleState = new IdleState(this);
        walkState = new WalkState(this);
        chargeState = new ChargeState(this);
        meleeAttackState = new MeleeAttackState(this);
        rangeAttackState = new RangeAttackState(this);
        
        // Start with walk state
        ChangeState(walkState);
    }
    
    void Update()
    {
        currentState?.Update();
    }
    
    public void ChangeState(EnemyState newState)
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
    
    void OnTriggerEnter(Collider other)
    {
        currentState?.OnTriggerEnter(other);
    }
    
    void OnTriggerExit(Collider other)
    {
        currentState?.OnTriggerExit(other);
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
}
