using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
    public int Id;
    public int Value;
    public bool IsConsumed = false;

    private Renderer rend;
    private Coroutine blinkRoutine;

    [HideInInspector] public Material baseMat;      // Glass
    [HideInInspector] public Material positiveMat;  // Green
    [HideInInspector] public Material negativeMat;  // Red

    [Header("Blink Settings")]
    public float flashVisibleTime = 0.5f;      // how long it stays colored
    public float timeBetweenFlashes = 2.0f;    // how long it stays glass before next flash

    void Awake()
    {
        rend = GetComponent<Renderer>();

        if (rend != null && baseMat == null)
        {
            baseMat = rend.material;          // default = glass material
        }
    }

    public void SetValue(int v)
    {
        Value = v;
    }

    public void StartBlink()
    {
        if (rend == null) return;

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        blinkRoutine = StartCoroutine(BlinkLoop());
    }

    public void RevealPermanent()
    {
        if (rend == null) return;

        IsConsumed = true;

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        rend.material = (Value > 0) ? positiveMat : negativeMat;
    }

    private IEnumerator BlinkLoop()
    {
        Material blinkMat = Value > 0 ? positiveMat : negativeMat;

        while (!IsConsumed)
        {
            rend.material = baseMat;
            yield return new WaitForSeconds(timeBetweenFlashes);

            rend.material = blinkMat;
            yield return new WaitForSeconds(flashVisibleTime);

            rend.material = baseMat;
        }
    }
}
