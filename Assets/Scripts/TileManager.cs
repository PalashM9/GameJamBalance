using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IconEntry
{
    public string id;
    public string description;
}

[System.Serializable]
public class IconConfig
{
    public IconEntry[] positiveIcons;
    public IconEntry[] negativeIcons;
}

public class TileManager : MonoBehaviour
{
    public int positivesToPlace = 18;
    public int negativesToPlace = 18;

    [Header("Tile Materials")]
    public Material glassMaterial;

    [Header("Tile Cover Sprites (18 each)")]
    public Sprite[] positiveTileIcons;     
    public Sprite[] negativeTileIcons;   

    [Header("Icon Settings")]
    [Range(0.3f, 1f)]
    public float iconSizePercent = 0.85f;
    public float iconYOffset = 0.21f;

    [Header("Icon Text Config (JSON)")]
    public TextAsset iconConfigJson;        

    private Tile[] tiles;

    private Dictionary<string, string> positiveDesc = new Dictionary<string, string>();
    private Dictionary<string, string> negativeDesc = new Dictionary<string, string>();

    // ---------------- UNITY ----------------

    void Awake()
    {
        LoadIconConfigFromJson();
    }

    void Start()
    {
        tiles = GetComponentsInChildren<Tile>();
        AssignValuesAndIcons();
    }

    // ---------------- CONFIG LOADING ----------------

    void LoadIconConfigFromJson()
    {
        if (iconConfigJson == null)
        {
            Debug.LogError("TileManager: iconConfigJson is not assigned!");
            return;
        }

        IconConfig config = JsonUtility.FromJson<IconConfig>(iconConfigJson.text);
        if (config == null)
        {
            Debug.LogError("TileManager: Failed to parse icon_config.json");
            return;
        }

        positiveDesc.Clear();
        negativeDesc.Clear();

        if (config.positiveIcons != null)
        {
            foreach (var entry in config.positiveIcons)
            {
                if (!string.IsNullOrEmpty(entry.id))
                    positiveDesc[entry.id] = entry.description;
            }
        }

        if (config.negativeIcons != null)
        {
            foreach (var entry in config.negativeIcons)
            {
                if (!string.IsNullOrEmpty(entry.id))
                    negativeDesc[entry.id] = entry.description;
            }
        }
    }

    // ---------------- HELPERS ----------------

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    string GetPositiveDescription(Sprite sprite)
    {
        if (sprite == null) return "";
        if (positiveDesc.TryGetValue(sprite.name, out string desc))
            return desc;
        return "Positive life choice";
    }

    string GetNegativeDescription(Sprite sprite)
    {
        if (sprite == null) return "";
        if (negativeDesc.TryGetValue(sprite.name, out string desc))
            return desc;
        return "Negative life choice";
    }

    // ---------------- MAIN ASSIGNMENT ----------------

    void AssignValuesAndIcons()
    {
        if (positiveTileIcons.Length != positivesToPlace)
            Debug.LogWarning("Positive icons count != positivesToPlace");

        if (negativeTileIcons.Length != negativesToPlace)
            Debug.LogWarning("Negative icons count != negativesToPlace");

        tiles = GetComponentsInChildren<Tile>();
        if (tiles.Length != positivesToPlace + negativesToPlace)
            Debug.LogWarning("Tile count != positivesToPlace + negativesToPlace");

        List<Sprite> posIcons = new List<Sprite>(positiveTileIcons);
        List<Sprite> negIcons = new List<Sprite>(negativeTileIcons);

        Shuffle(posIcons);
        Shuffle(negIcons);

        // build values list
        List<int> values = new List<int>();
        for (int i = 0; i < positivesToPlace; i++) values.Add(2);
        for (int i = 0; i < negativesToPlace; i++) values.Add(-2);
        Shuffle(values);

        int posIndex = 0;
        int negIndex = 0;

        for (int i = 0; i < tiles.Length && i < values.Count; i++)
        {
            Tile t = tiles[i];
            t.Id = i;

            int v = values[i];
            t.SetValue(v);

            // material
            var rend = t.GetComponent<Renderer>();
            if (rend != null && glassMaterial != null)
            {
                rend.material = glassMaterial;
                t.baseMat = glassMaterial;
            }

            Sprite cover;
            if (v > 0)
            {
                cover = posIcons[posIndex];
                t.Description = GetPositiveDescription(cover);
                posIndex++;
            }
            else
            {
                cover = negIcons[negIndex];
                t.Description = GetNegativeDescription(cover);
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
