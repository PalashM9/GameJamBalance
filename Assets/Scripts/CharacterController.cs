using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  // small downward force to keep grounded
        }

        // Get player input (WASD/Arrow keys)
        float x = Input.GetAxis("Horizontal"); 
        float z = Input.GetAxis("Vertical");

        // Calculate the movement vector based on input
        Vector3 move = transform.right * x + transform.forward * z;

        // Move the character using the CharacterController
        controller.Move(move * speed * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Debug log for player's current position (coordinates)
        Debug.Log("Player Position: " + transform.position);
    }
}
