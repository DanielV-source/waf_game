using UnityEngine;

public class GyroController : MonoBehaviour
{
    private Gyroscope gyro;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private bool isGyroAvailable;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Check if the device has a gyroscope
        isGyroAvailable = SystemInfo.supportsGyroscope;

        if (isGyroAvailable)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            Input.compass.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isGyroAvailable)
        {
            UpdateGyroMovementDirection();
        }
        else
        {
            UpdateTouchOrMouseMovementDirection();
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(movementDirection * 10f);
    }

    private void UpdateGyroMovementDirection()
    {
        Quaternion rotation = ConvertGyroQuaternion(gyro.attitude);

        Vector3 forward = rotation * Vector3.forward;

        movementDirection = new Vector2(forward.x, 0f);
    }

    private Quaternion ConvertGyroQuaternion(Quaternion q)
    {
        return new Quaternion(q.x, 0.0f, -q.z, 0.0f);
    }

    private void UpdateTouchOrMouseMovementDirection()
    {
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            Vector3 touchPosition3D = Input.touchCount > 0 ?
                Input.GetTouch(0).position : (Vector3)Input.mousePosition;
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touchPosition3D);
            Vector2 touchDirection = touchPosition - rb.position;
            touchDirection.y = 0f;
            movementDirection = touchDirection.normalized;
        }
    }

}
