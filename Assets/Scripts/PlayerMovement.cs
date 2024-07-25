using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody2D rb;
    private Collider2D capsuleCollider;
    public Button leftButton;   // Reference to the left movement button
    public Button rightButton;  // Reference to the right movement button
    public Button jumpButton;  // Reference to the jump button in the UI

    private Vector2 velocity;
    private float inputAxis;

    private bool moveleft, moveright;

    public float moveSpeed = 8f;
    public float maxJumpHeight = 5f;
    public float maxJumpTime = 1f;
    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);

    public bool grounded { get; private set; }
    public bool jumping { get; private set; }
    public bool running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;
    public bool sliding => (inputAxis > 0f && velocity.x < 0f) || (inputAxis < 0f && velocity.x > 0f);
    public bool falling => velocity.y < 0f && !grounded;

    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<Collider2D>();

        // Add listeners to the jump button
        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(Jump);
        }
    }

    private void OnEnable()
    {
        rb.isKinematic = false;
        capsuleCollider.enabled = true;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void OnDisable()
    {
        rb.isKinematic = true;
        capsuleCollider.enabled = false;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void Update()
    {
        HandleInput();
        HorizontalMovement();

        grounded = rb.Raycast(Vector2.down);

        if (grounded)
        {
            GroundedMovement();
        }

        ApplyGravity();
    }

    private void FixedUpdate()
    {
        // Move player based on velocity
        Vector2 position = rb.position;
        position += velocity * Time.fixedDeltaTime;

        // Clamp within the screen bounds
        Vector2 leftEdge = mainCamera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

        rb.MovePosition(position);
    }

    private void HandleInput()
    {
        // Handle touch input
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // Check if touch is on the left or right half of the screen
            if (touch.position.x < Screen.width / 4)
            {
                // Left side of the screen (move left)
                velocity.x = -moveSpeed;
            }
            else
            {
                // Right side of the screen (move right)
                velocity.x = moveSpeed;
            }
        }

        // If no touch is detected, reset horizontal movement
        if (Input.touchCount == 0)
        {
            velocity.x = inputAxis * moveSpeed;
        }
    }

    private void HorizontalMovement()
    {
        // Accelerate / decelerate
       inputAxis = Input.GetAxis("Horizontal");

        // Apply movement based on input
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * moveSpeed, moveSpeed * Time.deltaTime);

        // Check if running into a wall
        if (rb.Raycast(Vector2.right * velocity.x))
        {
            velocity.x = 0f;
        }

        // Flip sprite to face direction
        if (velocity.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (velocity.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void GroundedMovement()
    {
        // Prevent gravity from infinitely building up
        velocity.y = Mathf.Max(velocity.y, 0f);
        jumping = velocity.y > 0f;

        // Perform jump
        if (Input.GetButtonDown("Jump") || (jumpButton != null && jumpButton.onClick.GetPersistentEventCount() > 0))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (grounded)
        {
            // Ensure consistent jump height
            velocity.y = Mathf.Sqrt(1f * jumpForce * Mathf.Abs(gravity));
            jumping = true;
        }
    }

    public void MoveLeft()
    {
        inputAxis = -1;
        print("MoveLeft");
    }

    public void MoveRight()
    {
        inputAxis = 1;

        print("MoveRight");
    }

    public void Cancel()
    {
        inputAxis = 0;
    }

    private void ApplyGravity()
    {
        // Check if falling
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        float multiplier = falling ? 2f : 1f;

        // Apply gravity and terminal velocity
        velocity.y += gravity * multiplier * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Bounce off enemy head
            if (transform.DotTest(collision.transform, Vector2.down))
            {
                velocity.y = jumpForce / 2f;
                jumping = true;
            }
        }
        else if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            // Stop vertical movement if player hits head
            if (transform.DotTest(collision.transform, Vector2.up))
            {
                velocity.y = 0f;
            }
        }
    }

    private void OnDestroy()
    {
        // Clean up the button listener when the object is destroyed
        if (jumpButton != null)
        {
            jumpButton.onClick.RemoveListener(Jump);
        }
    }

    private void UpdateVelocity()
    {
        // Update velocity based on current input states
        if (moveleft)
        {
            velocity.x = -moveSpeed;
        }
        else if (moveright)
        {
            velocity.x = moveSpeed;
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * moveSpeed, moveSpeed * Time.deltaTime);
        }
    }
}
