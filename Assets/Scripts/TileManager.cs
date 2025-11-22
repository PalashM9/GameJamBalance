using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public int positivesToPlace = 18;
    public int negativesToPlace = 18;

    [Header("Tile Visuals")]
    public Material glassMaterial;      
    public Sprite positiveIconSprite;    // + tile icon
    public Sprite negativeIconSprite;    // - tile icon

    [Range(0.1f, 1f)]
    public float iconSizePercent = 0.8f; 

    public float iconYOffset = 0.18f;    
       

    private Tile[] tiles;

    private readonly string[] positiveTexts = new string[]
    {
        "Daily exercise",
        "Healthy meals",
        "Regular sleep",
        "Short breaks",
        "Saying 'no'",
        "Delegating tasks",
        "Time-blocking work",
        "Weekend family time",
        "Hobbies & creativity",
        "Clear boundaries",
        "Meditation & breathing",
        "Staying hydrated",
        "Planning your day",
        "Taking vacations",
        "Social time with friends",
        "Organized workspace",
        "Listening to music",
        "Logging off after work"
    };

    private readonly string[] negativeTexts = new string[]
    {
        "Overworking late nights",
        "Skipping meals",
        "No physical activity",
        "Constant stress",
        "No breaks",
        "Doomscrolling",
        "Always online for work",
        "Poor sleep habits",
        "Too much caffeine",
        "Procrastination",
        "Too much multitasking",
        "Skipping vacations",
        "Working from bed",
        "Comparing to others",
        "Staying inside all day",
        "Ignoring mental health",
        "Toxic work pressure",
        "Neglecting family & friends"
    };

    void Start()
    {
        tiles = GetComponentsInChildren<Tile>();
        AssignValuesAndIcons();
    }

    void AssignValuesAndIcons()
    {
        List<int> values = new List<int>();

        for (int i = 0; i < positivesToPlace; i++)
            values.Add(2);

        for (int i = 0; i < negativesToPlace; i++)
            values.Add(-2);

        for (int i = values.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        int posCount = 0, negCount = 0;
        int posLabelIndex = 0, negLabelIndex = 0;

        for (int i = 0; i < tiles.Length; i++)
        {
            Tile t = tiles[i];

            t.Id = i;
            int v = values[i];
            t.SetValue(v);

            var rend = t.GetComponent<Renderer>();
            if (rend != null && glassMaterial != null)
            {
                rend.material = glassMaterial;
                t.baseMat = glassMaterial;
            }

            if (v > 0)
            {
                t.Description = positiveTexts[posLabelIndex % positiveTexts.Length];
                posLabelIndex++;
                posCount++;
            }
            else
            {
                t.Description = negativeTexts[negLabelIndex % negativeTexts.Length];
                negLabelIndex++;
                negCount++;
            }

            CreateIconForTile(t);
        }

        Debug.Log($"RESULT: {posCount} positive tiles, {negCount} negative tiles.");
    }

    void CreateIconForTile(Tile tile)
    {
    if (positiveIconSprite == null || negativeIconSprite == null)
    {
        Debug.LogWarning("TileManager: positive/negative icon sprites are not assigned.");
        return;
    }

    GameObject iconGO = new GameObject("TileIcon");
    iconGO.transform.SetParent(tile.transform);

    BoxCollider col = tile.GetComponent<BoxCollider>();
    float width, height, depth;

    if (col != null)
    {
        width  = col.size.x * tile.transform.localScale.x;
        height = col.size.y * tile.transform.localScale.y;
        depth  = col.size.z * tile.transform.localScale.z;
    }
    else
    {
        width  = tile.transform.localScale.x;
        height = tile.transform.localScale.y;
        depth  = tile.transform.localScale.z;
    }

    iconGO.transform.localPosition = new Vector3(0f, iconYOffset, 0f);

    iconGO.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

    SpriteRenderer sr = iconGO.AddComponent<SpriteRenderer>();
    sr.sprite = tile.Value > 0 ? positiveIconSprite : negativeIconSprite;
    sr.sortingOrder = 10;
    sr.drawMode = SpriteDrawMode.Simple;

    Vector2 spriteWorldSize = sr.sprite.bounds.size;

    float targetWidth  = width  * iconSizePercent;
    float targetDepth  = depth  * iconSizePercent;

    float scaleX = targetWidth  / spriteWorldSize.x;
    float scaleY = targetDepth  / spriteWorldSize.y;

    iconGO.transform.localScale = new Vector3(scaleX, scaleY, 1f);

    tile.iconRenderer = sr;
    }

}
