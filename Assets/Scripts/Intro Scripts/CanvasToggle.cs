using UnityEngine;

public class CanvasToggle : MonoBehaviour
{
    public GameObject uiCanvas; 

    void Start()
    {
        uiCanvas.SetActive(false); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            uiCanvas.SetActive(true);
        }
    }
}
