using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    [SerializeField] private Image balanceImage;
    [SerializeField] private Sprite[] balanceSprites; 

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverTitleText;
    [SerializeField] private TextMeshProUGUI gameOverMessageText;
    [SerializeField] private Image gameOverImage;      
    [SerializeField] private float restartDelay = 3f;  

    [Header("Game Over Animation")]
    [SerializeField] private Sprite[] fireFrames;   
    [SerializeField] private float fireFrameTime = 0.15f;

    private Coroutine fireAnimationRoutine;


    private Coroutine blinkRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void UpdateScoreUI(Tile tile, int totalScore, int positiveHits, int negativeHits)
    {
        if (tile == null) return;

        lastEventText.text = tile.Description;
        lastValueText.text = tile.Value > 0 ? $"+{tile.Value}" : tile.Value.ToString();

        if (tile.Value > 0)
        {
            lastEventText.color = new Color(0.2f, 1f, 0.2f);
            lastValueText.color = new Color(0.2f, 1f, 0.2f);
        }
        else
        {
            lastEventText.color = new Color(1f, 0.2f, 0.2f);
            lastValueText.color = new Color(1f, 0.2f, 0.2f);
        }

        totalScoreText.text = $"Score: {totalScore}";
        positiveCountText.text = $"+ Tiles: {positiveHits}";
        negativeCountText.text = $"- Tiles: {negativeHits}";

        // SCALE
        UpdateScaleVisual(positiveHits, negativeHits);

        // BLINK
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
        int magnitude = Mathf.Clamp(Mathf.Abs(diff), 0, balanceSprites.Length - 1);

        balanceImage.sprite = balanceSprites[magnitude];

        float xScale = diff >= 0 ? 1f : -1f;
        var rt = balanceImage.rectTransform;
        rt.localScale = new Vector3(xScale, 1f, 1f);
    }

    public void TriggerGameOver(int finalScore)
    {
        if (gameOverPanel == null)
        {
            Debug.LogWarning("UIManager: GameOverPanel not assigned.");
            StartCoroutine(RestartAfterDelay());
            return;
        }

        bool positiveEnd = finalScore >= 8;

        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        if (gameOverTitleText != null)
        {
            gameOverTitleText.text = "Game Over";
        }

        if (gameOverMessageText != null)
        {
            gameOverMessageText.text = positiveEnd
                ? "You overloaded the work side!\nBalance is lost."
                : "You burned out on negatives.\nBalance is broken.";
        }

    if (fireAnimationRoutine != null)
        StopCoroutine(fireAnimationRoutine);

    fireAnimationRoutine = StartCoroutine(PlayFireAnimation());



        StartCoroutine(RestartAfterDelay());
    }

    private System.Collections.IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void ShowWarning(string msg)
    {
        lastEventText.text = msg;
        lastEventText.color = Color.yellow;
    }

    private IEnumerator PlayFireAnimation()
{
    if (gameOverImage == null || fireFrames == null || fireFrames.Length == 0)
        yield break;

    int index = 0;

    while (true)
    {
        gameOverImage.sprite = fireFrames[index];
        index = (index + 1) % fireFrames.Length;   // alternate frames
        yield return new WaitForSeconds(fireFrameTime);
    }
}

public void ShowSupportLost(string msg)
    {
        lastEventText.text = msg;
        lastEventText.color = Color.red;
    }

}
