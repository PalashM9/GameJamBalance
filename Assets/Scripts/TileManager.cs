using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public int positivesToPlace = 18;
    public int negativesToPlace = 18;

    [Header("Tile Materials")]
    public Material glassMaterial;

    [Header("Tile Cover Sprites")]
    public Sprite[] positiveTileIcons;  
    public Sprite[] negativeTileIcons;  

    [Header("Icon Settings")]
    [Range(0.3f, 1f)]
    public float iconSizePercent = 0.85f;

    public float iconYOffset = 0.21f;

    private Tile[] tiles;

    // 18 texts each
    private readonly string[] positiveTexts = new string[]
    {
        "Daily exercise", "Healthy meals", "Regular sleep", "Short breaks",
        "Saying 'no'", "Delegating tasks", "Time-blocking", "Family weekends",
        "Hobbies & creativity", "Clear boundaries", "Meditation", "Hydration",
        "Planning ahead", "Vacations", "Social friends", "Organized desk",
        "Listening to music", "Logging off after work"
    };

    private readonly string[] negativeTexts = new string[]
    {
        "Late night overwork", "Skipping meals", "Bed Drowning", "Constant stress",
        "No breaks", "Doomscrolling", "Always online", "Poor sleep",
        "Too much caffeine", "Procrastination", "Multitasking", "No vacations",
        "Working in bed", "Comparing others", "Staying inside", "Ignoring health",
        "Toxic pressure", "Neglecting family"
    };

    void Start()
    {
        tiles = GetComponentsInChildren<Tile>();
        AssignValuesAndIcons();
    }

    List<Sprite> BuildRepeatedList(Sprite[] baseList)
    {
        List<Sprite> result = new List<Sprite>();
        foreach (Sprite s in baseList)
        {
            result.Add(s);
            result.Add(s); 
        }
        return result;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    void AssignValuesAndIcons()
    {
        var posIcons = BuildRepeatedList(positiveTileIcons);
        var negIcons = BuildRepeatedList(negativeTileIcons);

        Shuffle(posIcons);
        Shuffle(negIcons);

        List<int> values = new List<int>();

        for (int i = 0; i < positivesToPlace; i++) values.Add(2);
        for (int i = 0; i < negativesToPlace; i++) values.Add(-2);

        Shuffle(values);

        int posIndex = 0;
        int negIndex = 0;

        for (int i = 0; i < tiles.Length; i++)
        {
            Tile t = tiles[i];
            t.Id = i;
            int v = values[i];
            t.SetValue(v);

            // assign material
            var rend = t.GetComponent<Renderer>();
            if (rend != null && glassMaterial != null)
            {
                rend.material = glassMaterial;
                t.baseMat = glassMaterial;
            }

            Sprite cover = null;

            if (v > 0)
            {
                t.Description = positiveTexts[posIndex];
                cover = posIcons[posIndex];
                posIndex++;
            }
            else
            {
                t.Description = negativeTexts[negIndex];
                cover = negIcons[negIndex];
                negIndex++;
            }

            CreateIconForTile(t, cover);
        }
    }

    void CreateIconForTile(Tile tile, Sprite assignedSprite)
    {
        GameObject iconGO = new GameObject("TileCover");
        iconGO.transform.SetParent(tile.transform);

        BoxCollider col = tile.GetComponent<BoxCollider>();

        float width = col.size.x * tile.transform.localScale.x;
        float depth = col.size.z * tile.transform.localScale.z;

        iconGO.transform.localPosition = new Vector3(0, iconYOffset, 0);
        iconGO.transform.localRotation = Quaternion.Euler(90, 0, 0);

        SpriteRenderer sr = iconGO.AddComponent<SpriteRenderer>();
        sr.sprite = assignedSprite;
        sr.sortingOrder = 10;
        sr.color = Color.white; 
        Vector2 s = assignedSprite.bounds.size;

        float targetW = width * iconSizePercent;
        float targetD = depth * iconSizePercent;

        iconGO.transform.localScale = new Vector3(
            targetW / s.x,
            targetD / s.y,
            1f
        );

        tile.iconRenderer = sr;
    }
}
