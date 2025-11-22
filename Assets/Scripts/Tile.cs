using UnityEngine;
using System.Collections;   // for IEnumerator

public class Tile : MonoBehaviour
{
    public int Id;
    public int Value;
    public bool IsConsumed = false;

    private Renderer rend;

    // Filled by TileManager
    [HideInInspector] public Material baseMat;      // GlassMaterial
    [HideInInspector] public Material positiveMat;  // GreenChildMaterial
    [HideInInspector] public Material negativeMat;  // RedChildMaterial

    void Awake()
    {
        rend = GetComponent<Renderer>();

        if (rend != null && baseMat == null)
        {
            baseMat = rend.material;
        }
    }

    public void SetValue(int v)
    {
        Value = v;
    }

    public void StartBlink()
    {
        if (rend == null) return;
        StopAllCoroutines();
        StartCoroutine(BlinkRoutine());
    }

    private IEnumerator BlinkRoutine()
    {
        Material blinkMat = Value > 0 ? positiveMat : negativeMat;

        int flashes = 4;
        float flashTime = 0.3f;

        for (int i = 0; i < flashes; i++)
        {
            rend.material = blinkMat;      // ON (green or red)
            yield return new WaitForSeconds(flashTime * 0.5f);

            rend.material = baseMat;       // OFF (glass)
            yield return new WaitForSeconds(flashTime * 0.5f);
        }

        rend.material = blinkMat;
    }
}
