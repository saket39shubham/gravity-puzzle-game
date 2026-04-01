using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 5f;

    [Header("Gravity")]
    public float gravityStrength = 9.8f;
    private Vector3 gravityDirection = Vector3.down;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.6f;
    public LayerMask groundMask;

    private Vector3 selectedGravityDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        selectedGravityDirection = gravityDirection;
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleGravityInput();
        ApplyGravity();
    }
    void HandleGroundCheck()
    {
        RaycastHit hit;

        if (Physics.Raycast(groundCheck.position, gravityDirection, out hit, groundDistance, groundMask))
        {
            isGrounded = true;

            if (Vector3.Dot(velocity, gravityDirection) > 0)
            {
                velocity = Vector3.zero;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            gravityDirection = selectedGravityDirection;
            AlignPlayerToGravity();
        }
    }

    void ApplyGravity()
    {
        velocity += gravityDirection * gravityStrength * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, 20f);
        controller.Move(velocity * Time.deltaTime);
    }

    void AlignPlayerToGravity()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -gravityDirection) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }
}