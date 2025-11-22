using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float forwardJumpBoost = 2f;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Ground check
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

       
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            
            if (z > 0.1f)
            {
                Vector3 boost = transform.forward * forwardJumpBoost;
                controller.Move(boost * Time.deltaTime);
            }

            Debug.Log("Jump!");
        }

        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        
        Debug.Log("Grounded: " + controller.isGrounded + " | velY: " + velocity.y);
    }
}
