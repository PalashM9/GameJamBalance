using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;

    [Header("Jump Settings")]
    public float jumpHeight = 1.5f;
    public float forwardJumpDistance = 1.2f;
    public float forwardJumpSpeed = 4f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpClip;
    public AudioClip positiveClip;
    public AudioClip negativeClip;

    [Header("Fall Settings")]
    public float fallGameOverHeight = 0f;  

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isJumpingForward;

    public int totalScore = 0;
    public int positiveHits = 0;
    public int negativeHits = 0;

    public int positiveGameOverScore = 12;
    public int negativeGameOverScore = -12;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (transform.position.y < fallGameOverHeight)
        {
            UIManager.Instance?.TriggerGameOver(totalScore);
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
            controller.Move(move * speed * Time.deltaTime);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpClip != null)
                audioSource.PlayOneShot(jumpClip);

            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            bool wantsForward = z > 0.1f;
            if (wantsForward && !isJumpingForward)
                StartCoroutine(ForwardHop());
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
        SupportCube support = hit.collider.GetComponent<SupportCube>();
        if (support != null)
        {
            support.OnPlayerHit();
            return;  
        }

        
        Tile tile = hit.collider.GetComponent<Tile>();
        if (tile == null) return;

        if (tile.IsConsumed) return;

        tile.RevealPermanent();

        int value = tile.Value;
        totalScore += value;

        if (value > 0)
        {
            positiveHits++;
            if (positiveClip != null)
                audioSource.PlayOneShot(positiveClip);
        }
        else
        {
            negativeHits++;
            if (negativeClip != null)
                audioSource.PlayOneShot(negativeClip);
        }

        Debug.Log(
            $"STEPPED ON TILE -> Id: {tile.Id}, Value: {value} | " +
            $"TotalScore: {totalScore}, +Tiles: {positiveHits}, -Tiles: {negativeHits}"
        );

        UIManager.Instance?.UpdateScoreUI(tile, totalScore, positiveHits, negativeHits);

        if (totalScore >= positiveGameOverScore || totalScore <= negativeGameOverScore)
        {
            UIManager.Instance?.TriggerGameOver(totalScore);
            enabled = false;
        }
    }
}
