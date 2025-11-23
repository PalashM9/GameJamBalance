using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SupportCube : MonoBehaviour
{
    [Header("Blinking")]
    public float blinkSpeed = 2f;   // how fast alpha changes
    public float minAlpha = 0.3f;
    public float maxAlpha = 1f;

    [Header("Touches")]
    public int maxTouches = 2;      // after this it disappears

    private int touchCount = 0;
    private MeshRenderer rend;
    private Color baseColor;

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        if (rend != null)
            baseColor = rend.material.color;

        // make sure collider is SOLID (not trigger)
        Collider col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    void Update()
    {
        if (rend == null) return;

        // simple blink by changing alpha
        float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f;
        float a = Mathf.Lerp(minAlpha, maxAlpha, t);

        Color c = baseColor;
        c.a = a;
        rend.material.color = c;
    }

    // called from PlayerMovement when player hits this collider
    public void OnPlayerHit()
    {
        touchCount++;

        if (touchCount < maxTouches)
        {
            // first (or intermediate) warning
            UIManager.Instance?.ShowWarning(" You might fall! Your support is weakening.");
        }
        else
        {
            // final message
            UIManager.Instance?.ShowSupportLost(" You lost support – now you’re on your own!");

            // disable collider immediately so player can fall
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            // destroy after short delay (so message can be read)
            StartCoroutine(DeleteAfterDelay(0.6f));
        }
    }

    private System.Collections.IEnumerator DeleteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
