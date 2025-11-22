using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI lastEventText;
    [SerializeField] private TextMeshProUGUI lastValueText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI positiveCountText;
    [SerializeField] private TextMeshProUGUI negativeCountText;

    [Header("Blink Settings")]
    [SerializeField] private int blinkTimes = 4;
    [SerializeField] private float blinkInterval = 0.12f;

    private Coroutine blinkRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateScoreUI(Tile tile, int totalScore, int positiveHits, int negativeHits)
    {
        if (tile == null) return;

        // texts
        lastEventText.text  = tile.Description;
        lastValueText.text  = tile.Value > 0 ? $"+{tile.Value}" : tile.Value.ToString();
        totalScoreText.text = $"Score: {totalScore}";
        positiveCountText.text = $"Positive : {positiveHits}";
        negativeCountText.text = $"Negative : {negativeHits}";

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        bool isPositive = tile.Value > 0;
        blinkRoutine = StartCoroutine(BlinkTexts(isPositive));
    }

    private System.Collections.IEnumerator BlinkTexts(bool positive)
    {
        Color baseEvent = lastEventText.color;
        Color baseValue = lastValueText.color;

        Color flashColor = positive
            ? new Color(0.3f, 1f, 0.3f)    // soft green
            : new Color(1f, 0.3f, 0.3f);   // soft red

        for (int i = 0; i < blinkTimes; i++)
        {
            lastEventText.color = flashColor;
            lastValueText.color = flashColor;
            yield return new WaitForSeconds(blinkInterval);

            lastEventText.color = baseEvent;
            lastValueText.color = baseValue;
            yield return new WaitForSeconds(blinkInterval);
        }

        // ensure we end on base colors
        lastEventText.color = baseEvent;
        lastValueText.color = baseValue;
    }
}
