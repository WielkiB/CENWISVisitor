using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private CharacterController controller;
    private Camera playerCamera;

    public float walkingSpeed = 20f;
    public float mouseSensitivity = 150f;
    public float jumpHeight = 3f;
    public float gravity = 9.81f;

    private float verticalVelocity = 0f;
    private float mouseInputX = 0;
    private float mouseInputY = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float speed = walkingSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            speed = 2 * walkingSpeed;

        float controlsX = Input.GetAxis("Horizontal");
        float controlsZ = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * controlsX + transform.forward * controlsZ) * speed;

        if (Input.GetKey(KeyCode.Space) && controller.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * gravity);
        else if (controller.isGrounded)
            verticalVelocity = 0f;
        else
            verticalVelocity -= gravity * Time.deltaTime;

        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        mouseInputX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseInputY += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        mouseInputY = Mathf.Clamp(mouseInputY, -60f, 60f);

        transform.rotation = MyEuler.Euler(0f, mouseInputX, 0f);
        playerCamera.transform.localRotation = MyEuler.Euler(-mouseInputY, 0f, 0f);
    }
}
