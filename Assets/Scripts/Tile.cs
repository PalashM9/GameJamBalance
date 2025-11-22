using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
    public int Id;
    public int Value;
    public bool IsConsumed = false;
    public string Description;      // <--- NEW

    private Renderer rend;
    private Coroutine blinkRoutine;

    [HideInInspector] public Material baseMat;
    [HideInInspector] public Material positiveMat;
    [HideInInspector] public Material negativeMat;

    [Header("Blink Settings")]
    public float flashVisibleTime = 0.5f;
    public float timeBetweenFlashes = 2.0f;

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
