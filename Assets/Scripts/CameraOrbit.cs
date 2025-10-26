using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public EqManager eqManager;

    public float minDistance = 1.5f;
    public float maxDistance = 5f;
    public float currentDistance = 5f;

    public float mouseScrollSpeed = 0.5f;

    public float rotationSpeedX = 100.0f;
    public float rotationSpeedY = 100.0f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    bool orbitMode = false;

    private void OnEnable()
    {
        if (target == null)
        {
            Debug.LogError("Nie przypisano obiektu docelowego (target)!");
            return;
        }

        rotationX = transform.eulerAngles.y;
        rotationY = transform.eulerAngles.x;

        Cursor.lockState = CursorLockMode.None;
        CameraUpdate();
    }

    private void CameraUpdate()
    {
        rotationX += Input.GetAxis("Mouse X") * rotationSpeedX * Time.deltaTime;
        rotationY -= Input.GetAxis("Mouse Y") * rotationSpeedY * Time.deltaTime;

        rotationY = Mathf.Clamp(rotationY, -80, 80);

        Quaternion rotation = MyEuler.Euler(rotationY, rotationX, 0);
        Vector3 position = rotation * new Vector3(0, 0, -currentDistance) + target.position;

        transform.position = position;
        transform.rotation = rotation;
    }

    private void ZoomUpdate(bool onlyChangeDistance)
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        currentDistance -= scroll * mouseScrollSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        if (!onlyChangeDistance)
        {
            Quaternion rotation = MyEuler.Euler(rotationY, rotationX, 0);
            Vector3 position = rotation * new Vector3(0, 0, -currentDistance) + target.position;

            transform.position = position;
            transform.rotation = rotation;
        }
    }

    void LateUpdate()
    {
        if (target == null || eqManager.gameObject.activeInHierarchy) return;

        if (orbitMode)
        {
            ZoomUpdate(true);
            CameraUpdate();

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                orbitMode = false;
                GetComponent<PcAiming>().aimingEnabled = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else 
        {
            ZoomUpdate(false);

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                orbitMode = true;
                GetComponent<PcAiming>().aimingEnabled = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
