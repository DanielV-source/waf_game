using UnityEngine;

public class EnemyIA : MonoBehaviour
{
    private Rigidbody2D rb;
    public Rigidbody2D player;
    private float targetPos = -999f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPos == -999f)
        {
            if (rb.position.x < player.position.x)
            {
                this.rb.velocity = new Vector2(this.rb.velocity.x + 1.25f * Time.deltaTime, 0f);
            }
            else if (rb.position.x > player.position.x)
            {
                this.rb.velocity = new Vector2(this.rb.velocity.x - 1.25f * Time.deltaTime, 0f);
            }
        }
    }
}
