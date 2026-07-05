using UnityEngine;

public class CameraFollowMouse : MonoBehaviour
{
    public Transform player; // Reference to the player object
    public float sensitivity = 1000f; // Mouse sensitivity
    private float rotationX = 0f; // Vertical rotation
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the screen
    }
    
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Limit vertical rotation
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f); // Rotate camera vertically
        player.Rotate(Vector3.up * mouseX); // Rotate player horizontally
    }
}