using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private PlayerInput playerInput;
    private InputAction moveAction;
    private Rigidbody rb;

    [SerializeField] Transform cam;
    [SerializeField] private float speed = 5;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Animator animator;

    Vector3 camForward;
    Vector3 camRight;
    private Vector3 moveDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        camForward = cam.forward;
        camRight = cam.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
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
        Vector3 targetPos = rb.position + moveDir * speed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }
}
