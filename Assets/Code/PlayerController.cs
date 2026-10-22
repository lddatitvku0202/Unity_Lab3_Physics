using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    private Rigidbody2D body;
    private ContactFilter2D groundFilter;
    private float moveInput;
    private bool jumpRequested;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        // Ground = any contact whose normal points mostly up (not walls or ceiling).
        groundFilter = new ContactFilter2D();
        groundFilter.SetNormalAngle(45f, 135f);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        moveInput = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            moveInput -= 1f;
        }
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            moveInput += 1f;
        }

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        body.linearVelocity = new Vector2(moveInput * moveSpeed, body.linearVelocity.y);

        if (jumpRequested && body.IsTouching(groundFilter))
        {
            body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        jumpRequested = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[Collision] Enter: {collision.gameObject.name}");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log($"[Collision] Exit: {collision.gameObject.name}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Trigger] Enter: {other.gameObject.name}");
    }
}
