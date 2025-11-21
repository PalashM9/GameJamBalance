using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;   // Reference to the starting camera (Main Camera)
    public Camera playerCamera; // Reference to the player's POV camera

    void Start()
    {
        // Initially, we want the main camera active and player camera inactive
        mainCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        // When the player presses a button (e.g., "E"), switch cameras
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        mainCamera.gameObject.SetActive(false);  // Disable main camera
        playerCamera.gameObject.SetActive(true); // Enable player camera
    }
}
