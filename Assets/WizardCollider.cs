using System;
using System.Collections;
using UnityEngine;

public class WizardCollider : MonoBehaviour
{
    public GameObject explosion;
    public GameObject playerHealth;
    private Transform healthTransform;

    // Start is called before the first frame update
    void Start()
    {
        healthTransform = playerHealth.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision detected with " + other.gameObject.name);

        if (other.gameObject.name.StartsWith("magicSphere") && healthTransform.localScale.x > 0)
        {
            if (!other.GetComponent<BallScript>().doDamage)
                return;
            StartCoroutine(explosionAnim(other));
            Vector3 damage = new Vector3();
            damage.x = (other.gameObject.GetComponent<BallScript>().doDoubleDamage ? MainGame.currentEnemyDamage * 2f : MainGame.currentEnemyDamage) * Time.deltaTime;
            damage.y = 0.0f;
            damage.z = 0.0f;

            if ((healthTransform.localScale.x - damage.x) <= 0.0f)
            {
                damage.x = healthTransform.localScale.x;
            }

            healthTransform.localScale -= damage;

            if (healthTransform.localScale.x > MainGame.LOW_HEALTH && healthTransform.localScale.x <= MainGame.HALF_HEALTH)
            {
                playerHealth.GetComponent<SpriteRenderer>().color = MainGame.HALF_HEALTH_COLOR;
            }
            else if (healthTransform.localScale.x <= MainGame.LOW_HEALTH)
            {
                playerHealth.GetComponent<SpriteRenderer>().color = MainGame.LOW_HEALTH_COLOR;
            }
            other.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (healthTransform.localScale.x <= 0)
        {
            UnityReceiver.SendDataToAndroid("ultimates", (UnityReceiver.ultimates).ToString(), false);
            UnityReceiver.SendDataToAndroid("enemyAttacks", (UnityReceiver.ballAmount).ToString(), false);
            Debug.Log("Fails: " + UnityReceiver.fails +  " - MagicBalls: " + UnityReceiver.ballAmount);
            int accuracy = (UnityReceiver.ballAmount > 0 ? (Mathf.RoundToInt((float)(1 - UnityReceiver.fails / UnityReceiver.ballAmount) * 100)) : 0);
            int score = accuracy * (UnityReceiver.difficulty+1);
            UnityReceiver.SendDataToAndroid("score", score.ToString(), false);
            UnityReceiver.SendDataToAndroid("result", "lose", true);
            return;
        }
    }


    IEnumerator explosionAnim(Collider2D other)
    {
        GameObject instance = Instantiate(explosion, new Vector3(0, 2f, 0), Quaternion.identity);
        instance.GetComponent<Transform>().position = other.gameObject.GetComponent<Transform>().position;
        yield return new WaitForSeconds(2f);
        if (other.GetComponent<BallScript>().doDamage)
        {
            UnityReceiver.fails += 1;
        }
        Destroy(instance);
    }
}
