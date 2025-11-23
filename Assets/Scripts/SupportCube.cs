using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SupportCube : MonoBehaviour
{
    [Header("Blinking")]
    public float blinkSpeed = 2f;  
    public float minAlpha = 0.3f;
    public float maxAlpha = 1f;

    [Header("Touches")]
    public int maxTouches = 2;      
    private int touchCount = 0;
    private MeshRenderer rend;
    private Color baseColor;

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        Debug.Log("rend" + rend);
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

        Color c = baseColor;
        c.a = a;
        rend.material.color = c;
        Debug.Log("Base Color" + baseColor);
    }

    public void OnPlayerHit()
    {
        //Debug.Log("Touch Count With the Floor" + touchCount);
        touchCount++;

        if (touchCount < maxTouches)
        {
            UIManager.Instance?.ShowWarning(" You might fall! Your support is weakening.");
        }
        else
        {
            UIManager.Instance?.ShowSupportLost(" You lost support – now you’re on your own!");

            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            StartCoroutine(DeleteAfterDelay(0.6f));
        }
    }

    private System.Collections.IEnumerator DeleteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
