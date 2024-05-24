using UnityEngine;

public class BallScript : MonoBehaviour
{
    private new Transform transform;
    private float rotationSpeed = 0f;

    private float timer = 0f; // Timer to control the bounce effect

    // Configuration parameters
    public float bounceFrequency = 2f; // Controls the speed of the bounce
    public float bounceMagnitude = 0.00000001f; // Controls the height/size of the bounce
    public bool doDoubleDamage = false;
    public bool doDamage = true;

    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * bounceFrequency;

        float scale = Mathf.Min(0.12f + Mathf.Sin(timer) * bounceMagnitude, 0.12f);
        transform.localScale = new Vector3(scale, scale, scale);

        // Set the object's rotation to a specific angle around the Z axis
        transform.rotation = Quaternion.Euler(0, 0, (rotationSpeed%360));
        //transform.localScale = new Vector3(Mathf.Min(Mathf.Max(transform.localScale.x * Mathf.Cos(rotationSpeed), 0.1f), 0.15f), Mathf.Max(Mathf.Min(transform.localScale.y * Mathf.Sin(rotationSpeed)+0.15f, 0.1f), 0.15f), 0f);
        rotationSpeed += 5*Time.deltaTime;

        if (timer > 2 * Mathf.PI)
        {
            timer -= 2 * Mathf.PI;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with " + collision.gameObject.name);
    }
}
