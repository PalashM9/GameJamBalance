using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public int totalScore = 0;
    public int positiveHits = 0;
    public int negativeHits = 0;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        if (tile.IsConsumed) return;

        // mark + stop blinking + lock color
        tile.RevealPermanent();

        int value = tile.Value;
        totalScore += value;

        if (value > 0) positiveHits++;
        else negativeHits++;

        Debug.Log(
            $"STEPPED ON TILE -> Id: {tile.Id}, Name: {tile.name}, Value: {value} | " +
            $"TotalScore: {totalScore}, +Tiles: {positiveHits}, -Tiles: {negativeHits}"
        );

        // update UI panel
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreUI(tile, totalScore, positiveHits, negativeHits);
        }
    }
}
