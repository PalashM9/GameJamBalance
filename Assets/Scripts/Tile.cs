using UnityEngine;

public class Tile : MonoBehaviour
{
    public int Id;
    public int Value;            // +2 or -2
    public bool IsConsumed = false;
    public string Description;   // work-life text

    [HideInInspector] public SpriteRenderer iconRenderer;  
    [HideInInspector] public Material baseMat;

    public void SetValue(int v)
    {
        Value = v;
    }

    public void RevealPermanent()
    {
        IsConsumed = true;
    }
}
