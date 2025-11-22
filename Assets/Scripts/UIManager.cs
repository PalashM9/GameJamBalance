using UnityEngine;
using UnityEngine.UI;
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

    [Header("Balance Scale")]
    [SerializeField] private Image balanceImage;          // UI Image in the panel
    [SerializeField] private Sprite[] balanceSprites;    
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

    /// <summary>
    /// Called by PlayerMovement whenever a tile is stepped on.
    /// </summary>
    public void UpdateScoreUI(Tile tile, int totalScore, int positiveHits, int negativeHits)
    {
        if (tile == null) return;

        // TEXTS
        lastEventText.text = tile.Description;
        lastValueText.text = tile.Value > 0 ? $"+{tile.Value}" : tile.Value.ToString();

        if (tile.Value > 0)
        {
            lastEventText.color = new Color(0.2f, 1f, 0.2f);   // soft green
            lastValueText.color = new Color(0.2f, 1f, 0.2f);
        }
        else
        {
            lastEventText.color = new Color(1f, 0.2f, 0.2f);   // soft red
            lastValueText.color = new Color(1f, 0.2f, 0.2f);
        }

        totalScoreText.text = $"Score: {totalScore}";
        positiveCountText.text = $"+ Tiles: {positiveHits}";
        negativeCountText.text = $"- Tiles: {negativeHits}";

        UpdateScaleVisual(positiveHits, negativeHits);

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        bool isPositive = tile.Value > 0;
        blinkRoutine = StartCoroutine(BlinkTexts(isPositive));
    }

    private System.Collections.IEnumerator BlinkTexts(bool positive)
        
    {
            Color baseEvent = lastEventText.color;
            Color baseValue = lastValueText.color;

            Color flashColor = baseEvent * 1.5f;
            flashColor.a = 1f;

            for (int i = 0; i < blinkTimes; i++)
            {
                lastEventText.color = flashColor;
                lastValueText.color = flashColor;
                yield return new WaitForSeconds(blinkInterval);

                lastEventText.color = baseEvent;
                lastValueText.color = baseValue;
                yield return new WaitForSeconds(blinkInterval);
            }

            lastEventText.color = baseEvent;
            lastValueText.color = baseValue;
        
    }


    private void UpdateScaleVisual(int positiveHits, int negativeHits)
    {
        if (balanceImage == null || balanceSprites == null || balanceSprites.Length == 0)
            return;

        int diff = positiveHits - negativeHits;

        // how strong the tilt is (0..4)
        int magnitude = Mathf.Clamp(Mathf.Abs(diff), 0, balanceSprites.Length - 1);

        balanceImage.sprite = balanceSprites[magnitude];

        float xScale = diff >= 0 ? 1f : -1f;
        var rt = balanceImage.rectTransform;
        rt.localScale = new Vector3(xScale, 1f, 1f);
    }
}
