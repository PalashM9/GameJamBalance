using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public int positivesToPlace = 18;
    public int negativesToPlace = 18;

    public Material glassMaterial;        // GlassMaterial
    public Material greenChildMaterial;   // GreenChildMaterial
    public Material redChildMaterial;     // RedChildMaterial

    private Tile[] tiles;

    void Start()
    {
        tiles = GetComponentsInChildren<Tile>();
        AssignValues();
    }

    void AssignValues()
    {
        List<int> values = new List<int>();

        for (int i = 0; i < positivesToPlace; i++)
            values.Add(2);

        for (int i = 0; i < negativesToPlace; i++)
            values.Add(-2);

        // shuffle values
        for (int i = values.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        int pos = 0;
        int neg = 0;

        for (int i = 0; i < tiles.Length; i++)
        {
            Tile t = tiles[i];

            t.Id = i;
            int v = values[i];
            t.SetValue(v);

            var rend = t.GetComponent<Renderer>();
            if (rend != null && glassMaterial != null)
            {
                rend.material = glassMaterial; // start as glass
                t.baseMat = glassMaterial;
            }

            t.positiveMat = greenChildMaterial;
            t.negativeMat = redChildMaterial;

            if (v > 0) pos++; else neg++;

            // start blink
            t.StartBlink();
        }

        Debug.Log($"RESULT: {pos} positive tiles, {neg} negative tiles.");
    }
}
