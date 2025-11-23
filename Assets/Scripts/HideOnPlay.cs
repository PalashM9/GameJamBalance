using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    public GameObject imageToHide;

    void Start()
    {
        if (imageToHide != null)
            imageToHide.SetActive(false);   
    }
}
