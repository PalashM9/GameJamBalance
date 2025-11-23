using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SupportCube : MonoBehaviour
{
    [Header("Blinking")]
    public float blinkSpeed = 2f;     // blink speed
    public float minAlpha = 0.3f;
    public float maxAlpha = 1f;

    [Header("Touches")]
    public int maxTouches = 2;       
    private int touchCount = 0;
    private MeshRenderer rend;
    private Color baseColor;
    private bool isBreaking = false;  

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        if (rend != null)
            baseColor = rend.material.color;

        Collider col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    void Update()
    {
        if (rend == null) return;

        float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f;
        float a = Mathf.Lerp(minAlpha, maxAlpha, t);
        
        Debug.Log("Support alpha: " + a);

        Color c = baseColor;
        c.a = a;
        rend.material.color = c;
        
    }

    public void OnPlayerHit()
    {
        if (isBreaking) return; 

        touchCount++;

        if (touchCount < maxTouches)
        {
            UIManager.Instance?.ShowWarning("You might fall! Your support is weakening.");
        }
        else
        {
            isBreaking = true;

            UIManager.Instance?.ShowSupportLost("You lost support – now you're on your own!");

            StartCoroutine(RemoveSupportAfterDelay(5f));
        }
    }

    private System.Collections.IEnumerator RemoveSupportAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // remove collider (player falls)
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // destroy cube
        Destroy(gameObject);
    }
}
