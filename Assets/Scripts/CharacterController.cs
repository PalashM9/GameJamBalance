using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;

    [Header("Jump Settings")]
    public float jumpHeight = 1.5f;           // vertical hop height
    public float forwardJumpDistance = 1.2f;  // distance between tiles
    public float forwardJumpSpeed = 4f;       

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isJumpingForward;

    // scoring
    public int totalScore = 0;
    public int positiveHits = 0;
    public int negativeHits = 0;

    // game over thresholds
    public int positiveGameOverScore = 8;
    public int negativeGameOverScore = -8;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    
    {
        if (transform.position.y < 2f)
        {
            if (UIManager.Instance != null)
                UIManager.Instance.TriggerGameOver(totalScore);

            enabled = false;
            return;
        }

        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            isJumpingForward = false;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (z < 0) z = 0;

        Vector3 move = transform.right * x + transform.forward * z;
        
        if (!isJumpingForward)
        {
            controller.Move(move * speed * Time.deltaTime);
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            bool wantsForward = z > 0.1f;

            if (wantsForward && !isJumpingForward)
            {
                StartCoroutine(ForwardHop());
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }


    private System.Collections.IEnumerator ForwardHop()
    {
        isJumpingForward = true;

        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * forwardJumpDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * forwardJumpSpeed;
            Vector3 newPos = Vector3.Lerp(start, end, t);

            controller.Move(newPos - transform.position);
            yield return null;
        }

        isJumpingForward = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        if (tile.IsConsumed) return;

        tile.RevealPermanent();

        int value = tile.Value;
        totalScore += value;

        if (value > 0) positiveHits++;
        else negativeHits++;

        Debug.Log(
            $"STEPPED ON TILE -> Id: {tile.Id}, Name: {tile.name}, Value: {value} | " +
            $"TotalScore: {totalScore}, +Tiles: {positiveHits}, -Tiles: {negativeHits}"
        );

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreUI(tile, totalScore, positiveHits, negativeHits);
        }

        if (totalScore >= positiveGameOverScore || totalScore <= negativeGameOverScore)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.TriggerGameOver(totalScore);
            }

            enabled = false;
        }
    }
}
