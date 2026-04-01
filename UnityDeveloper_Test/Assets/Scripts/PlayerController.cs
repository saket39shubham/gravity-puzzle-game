using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public Transform cameraTransform;

    [Header("Gravity")]
    public float jumpForce = 5f;
    public float gravityStrength = 35f;
    private Vector3 gravityDirection = Vector3.down;
    private Vector3 selectedGravityDirection;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckDistance = 1.2f;

    [Header("Hologram")]
    public GameObject hologram;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    Animator animator;
    public GameManager gameManager;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        selectedGravityDirection = gravityDirection;
        animator = GetComponent<Animator>();
        if (hologram != null)
            hologram.SetActive(true);
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleRotation();
        HandleJump();
        HandleGravityInput();
        ApplyGravity();
        HandleAnimations();
        CheckFallDeath();
    }

    void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && Vector3.Dot(velocity, gravityDirection) > 0)
        {
            velocity = Vector3.zero;
        }
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, -gravityDirection).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, -gravityDirection).normalized;

        Vector3 move = forward * z + right * x;

        controller.Move(move * speed * Time.deltaTime);
    }

    void HandleRotation()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, 0, z);

        if (move.magnitude > 0.1f)
        {
            Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, -gravityDirection).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(forward, -gravityDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity = -gravityDirection * Mathf.Sqrt(jumpForce * 2f * gravityStrength);
        }
    }

    void HandleGravityInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            selectedGravityDirection = Vector3.forward;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            selectedGravityDirection = Vector3.back;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            selectedGravityDirection = Vector3.left;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            selectedGravityDirection = Vector3.right;

        // Show hologram ONLY when holding arrow keys
        bool arrowPressed =
      Input.GetKeyDown(KeyCode.UpArrow) ||
      Input.GetKeyDown(KeyCode.DownArrow) ||
      Input.GetKeyDown(KeyCode.LeftArrow) ||
      Input.GetKeyDown(KeyCode.RightArrow);

        if (hologram != null)
        {
            if (arrowPressed)
            {
                hologram.SetActive(true);

                Quaternion lookRot = Quaternion.LookRotation(selectedGravityDirection);
                hologram.transform.rotation = lookRot;

                hologram.transform.position =
                    transform.position + selectedGravityDirection * 1.5f;
            }
        }

        // Apply gravity direction on Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            gravityDirection = selectedGravityDirection;
            AlignPlayerToGravity();

            if (hologram != null)
                hologram.SetActive(false);
        }
    }

    void ApplyGravity()
    {
        // Apply gravity
        velocity += gravityDirection * gravityStrength * Time.deltaTime;

        if (!isGrounded && Vector3.Dot(velocity, gravityDirection) > 0)
        {
            velocity += gravityDirection * gravityStrength * 2f * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }
    void AlignPlayerToGravity()
    {
        Quaternion targetRotation =
            Quaternion.FromToRotation(transform.up, -gravityDirection) * transform.rotation;

        transform.rotation = targetRotation;
    }

    void HandleAnimations()
    {
        if (animator == null) return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float speedValue = new Vector3(x, 0, z).magnitude;

        bool isFalling = !isGrounded && Vector3.Dot(velocity, gravityDirection) > 0;

        if (isFalling)
        {
            animator.Play("Falling Idle"); // exact animation name
        }
        else if (speedValue > 0.1f)
        {
            animator.Play("Running");
        }
        else
        {
            animator.Play("Idle");
        }
    }
    void CheckFallDeath()
    {
        if (gameManager == null || gameManager.isGameOver) return;

        // if player is far from ground (falling into void)
        if (!isGrounded && transform.position.y < -10f)
        {
            gameManager.GameOver("Fell Off!");
        }
    }
}