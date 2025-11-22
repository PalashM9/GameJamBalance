using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public int positivesToPlace = 18;
    public int negativesToPlace = 18;

    public Material glassMaterial;
    public Material greenChildMaterial;
    public Material redChildMaterial;

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
        AssignValuesAndDescriptions();
    }

    void AssignValuesAndDescriptions()
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

            t.positiveMat = greenChildMaterial;
            t.negativeMat = redChildMaterial;

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

            t.StartBlink();
        }

        Debug.Log($"RESULT: {posCount} positive tiles, {negCount} negative tiles.");
    }
}
