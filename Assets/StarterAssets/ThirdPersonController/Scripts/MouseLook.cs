using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Sensivity Settings")]
    public float mouseSensivity = 100f;
    public Transform playerBody;
    private float xRotation = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX=Input.GetAxis("MouseX") * mouseSensivity * Time.deltaTime;
        float mouseY = Input.GetAxis("MouseY") * mouseSensivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
