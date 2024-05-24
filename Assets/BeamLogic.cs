using System.Collections;
using UnityEngine;

public class BeamLogic : MonoBehaviour
{
    public GameObject enemyHBar;
    private Transform enemyHBarTransform;
    private SpriteRenderer enemyHBarSpriteRenderer;
    public GameObject explosion;
    public Rigidbody2D player;
    public static bool doingUlt = false;

    // Start is called before the first frame update
    void Start()
    {
        enemyHBarTransform = enemyHBar.GetComponent<Transform>();
        enemyHBarSpriteRenderer = enemyHBar.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Enemy")) {
            if(!collision.gameObject.GetComponent<AudioSource>().enabled)
            {
                collision.gameObject.GetComponent<AudioSource>().enabled = true;
            }

            if(!collision.gameObject.GetComponent<AudioSource>().isPlaying)
            {
                collision.gameObject.GetComponent<AudioSource>().Play();
            }

            if (enemyHBarTransform.localScale.x > 0)
            {
                Vector3 damage = new(MainGame.currentPlayerDamage, 0f, 0f);

                if ((enemyHBarTransform.localScale.x - damage.x * Time.deltaTime) <= 0.0f)
                {
                    damage.x = enemyHBarTransform.localScale.x;
                    enemyHBarTransform.localScale -= damage;
                }
                else
                {
                    enemyHBarTransform.localScale -= damage * Time.deltaTime;
                }

                if (enemyHBarTransform.localScale.x > MainGame.LOW_HEALTH && enemyHBarTransform.localScale.x <= MainGame.HALF_HEALTH)
                {
                    enemyHBarSpriteRenderer.color = MainGame.HALF_HEALTH_COLOR;
                }
                else if (enemyHBarTransform.localScale.x <= MainGame.LOW_HEALTH)
                {
                    enemyHBarSpriteRenderer.color = MainGame.LOW_HEALTH_COLOR;
                }
            }
            else
            {
                UnityReceiver.SendDataToAndroid("ultimates", (UnityReceiver.ultimates).ToString(), false);
                UnityReceiver.SendDataToAndroid("enemyAttacks", (UnityReceiver.ballAmount).ToString(), false);
                Debug.Log("Fails: " + UnityReceiver.fails + " - MagicBalls: " + UnityReceiver.ballAmount);
                int accuracy = (UnityReceiver.ballAmount > 0 ? Mathf.RoundToInt((1f - (float)UnityReceiver.fails / UnityReceiver.ballAmount) * 100) : 0);
                int score = accuracy * (UnityReceiver.difficulty + 1) * 2 + 100;
                UnityReceiver.SendDataToAndroid("score", score.ToString(), false);
                UnityReceiver.SendDataToAndroid("result", "win", true);
                return;
            }
        }
        else if(collision.gameObject.name.StartsWith("magicSphere"))
        {
            if (collision.gameObject.GetComponent<BallScript>().doDamage)
            { 
                StartCoroutine(explosionAnim(collision));
                collision.gameObject.GetComponent<BallScript>().doDamage = false;
                collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    IEnumerator explosionAnim(Collider2D other)
    {
        if (other == null)
        {
            Debug.LogError("Collision object was already destroyed");
            yield break;
        }
        BallScript ballScript = other.GetComponent<BallScript>();
        if (ballScript == null)
        {
            Debug.LogError("BallScript component is not found on the collided object");
            yield break;
        }
        GameObject instance = Instantiate(explosion, new Vector3(0, 2f, 0), Quaternion.identity);
        instance.GetComponent<Transform>().position = other.gameObject.GetComponent<Transform>().position;
        yield return new WaitForSeconds(2f);
        Destroy(instance);
    }
}
