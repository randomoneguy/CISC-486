using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction attackAction;
    private Rigidbody rb;

    [SerializeField] Transform cam;
    [SerializeField] private float speed = 5;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Animator animator;

    Vector3 camForward;
    Vector3 camRight;
    private Vector3 moveDir;
    
    // State machine reference
    private PlayerStateMachine stateMachine;
    
    // Attack input flag (set by input callback, checked by states)
    [HideInInspector] public bool attackInputPressed = false;
    
    // Check if player is currently attacking
    public bool IsAttacking()
    {
        return stateMachine != null && 
               (stateMachine.currentState is PlayerAttack1State || 
                stateMachine.currentState is PlayerAttack2State || 
                stateMachine.currentState is PlayerAttack3State);
    }
    
    // Check if player can resume movement (attack animation has progressed enough)
    public bool CanResumeMovement()
    {
        if (stateMachine == null || stateMachine.currentState == null)
            return true;
        
        // If in an attack state, check if movement can resume
        if (stateMachine.currentState is PlayerAttackStateBase attackState)
        {
            return attackState.CanResumeMovement();
        }
        
        // Not in an attack state, allow movement
        return true;
    }
    
    // Check if player is in a combo window (can chain next attack)
    public bool IsInComboWindow()
    {
        if (stateMachine == null || stateMachine.currentState == null)
            return false;
        
        if (stateMachine.currentState is PlayerAttackStateBase attackState)
        {
            return attackState.IsInComboWindow();
        }
        
        return false;
    }
    
    // Check if player has movement input (for interruptions)
    public bool HasMovementInput()
    {
        if (moveAction == null) return false;
        Vector2 input = moveAction.ReadValue<Vector2>();
        return input.sqrMagnitude > 0.001f;
    }
    
    // Check if attack button is currently pressed
    public bool IsAttackPressed()
    {
        if (attackAction == null) return false;
        return attackAction.WasPressedThisFrame();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        attackAction = playerInput.actions.FindAction("Attack");
        rb = GetComponent<Rigidbody>();
        
        // Get state machine component
        stateMachine = GetComponent<PlayerStateMachine>();
        if (stateMachine == null)
        {
            Debug.LogError("PlayerStateMachine component not found! Please add it to the player GameObject.");
        }

        rb.freezeRotation = true;

        camForward = cam.forward;
        camRight = cam.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
        
        // Subscribe to attack input
        if (attackAction != null)
        {
            attackAction.performed += OnAttackInput;
        }
    }
    
    private void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // If we're in idle state, start the combo
            if (stateMachine != null && stateMachine.currentState is PlayerIdleState)
            {
                attackInputPressed = true;
                Debug.Log("Attack input pressed");
                stateMachine.ChangeState(stateMachine.attack1State);
            }
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe from input events
        if (attackAction != null)
        {
            attackAction.performed -= OnAttackInput;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        // Check if movement can resume (allows movement after attack animation progresses)
        if (!CanResumeMovement())
        {
            animator.SetFloat("Speed", 0);
            moveDir = Vector3.zero;
            return;
        }
        
        // TODO: update this old input system to new one, if there is time
        Vector2 input = moveAction.ReadValue<Vector2>();

        moveDir = camForward * input.y + camRight * input.x;
        if (input.x != 0 || input.y != 0)
        {
            animator.SetFloat("Speed", 1);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }

        // Movement
        if (moveDir.sqrMagnitude > 0.001f)
        {
            // Move player
            transform.position += moveDir * speed * Time.deltaTime;

            // Smoothly rotate toward movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        // Only move if movement is allowed (respects attack animation progress)
        if (!CanResumeMovement())
        {
            return;
        }
        
        Vector3 targetPos = rb.position + moveDir * speed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }
}
