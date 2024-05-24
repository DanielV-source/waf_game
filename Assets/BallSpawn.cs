using System.Collections.Generic;
using UnityEngine;
using SRandom = UnityEngine.Random;
using SDebug = UnityEngine.Debug;
using System.Collections;

public class BallSpawn : MonoBehaviour
{
    public List<GameObject> objectsGenerated;
    public List<GameObject> sequenceGenerated;
    public GameObject prefab;
    public GameObject enemy;
    private Animator enemyAnimator;
    private Transform enemyPosition;

    public float delaySeconds = 2f; // Time to wait before performing the action
    private float timer = 0f;       // Timer to track the delay

    public bool actionDone = true; // Flag to control the action execution

    // Start is called before the first frame update
    void Start()
    {
        objectsGenerated = new();
        enemyAnimator = enemy.GetComponent<Animator>();
        enemyPosition = enemy.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= delaySeconds && !actionDone && sequenceGenerated.Count > 0)
        {
            PerformAction();
            enemyAnimator.Play("enemyAttack");
            StartCoroutine(Throw());
            if (objectsGenerated.Count > 25)
            {
                Destroy(objectsGenerated[0]);
                objectsGenerated.RemoveAt(0);
            }
            timer = 0f;
        }
        else if (sequenceGenerated.Count > 0 && !actionDone)
        {
            timer += Time.deltaTime;
        }

        if (sequenceGenerated.Count == 0)
        {
            enemyAnimator.Play("enemyIdle");
        }

        if (sequenceGenerated.Count == 0 && !actionDone && CheckIfAllThrown())
        {
            objectsGenerated.ForEach(obj => {
                Destroy(obj);
            });
            objectsGenerated.Clear();
            MainGame.allThrown = true;
            actionDone = true;
        }

        StartCoroutine(UpdatePositions());
    }

    bool CheckIfAllThrown()
    {
        for (int i = 0; i < objectsGenerated.Count; i++)
        {
            GameObject obj = objectsGenerated[i];
            if(obj.GetComponent<Transform>().position.y > -5) {
                return false;
            }
        }

        return true;
    }

    void PerformAction()
    {
        SDebug.Log("Action performed after " + delaySeconds + " seconds");
    }

    IEnumerator Throw()
    {
        
        GameObject instance = Instantiate(prefab, new Vector3(enemyPosition.position.x, 2f, 0), Quaternion.identity);
        Vector2 speed = new Vector2(SRandom.Range(-1.15f, 1.15f), -MainGame.currentMagicBallSpeed);
        instance.GetComponent<BallScript>().doDoubleDamage = (sequenceGenerated[0].GetComponent<BallScript>().doDoubleDamage);
        instance.GetComponent<BallScript>().doDamage = (sequenceGenerated[0].GetComponent<BallScript>().doDamage);
        if (instance.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.velocity = speed;
        }
        else
        {
            SDebug.LogError("Rigidbody2D component not found on the instantiated prefab");
        }

        objectsGenerated.Add(instance);

        if (sequenceGenerated.Count > 0)
            sequenceGenerated.RemoveAt(0);
  
        yield return new WaitForSeconds(1f);
    }

    public void SendSequence(List<GameObject> gameObjects)
    {
        sequenceGenerated = gameObjects;
        actionDone = false;
    }


    IEnumerator UpdatePositions()
    {
        while (BlowScript.isBlowing)
        {
            for (int i = 0; i < objectsGenerated.Count; i++)
            {
                GameObject obj = objectsGenerated[i];
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                Vector2 slowSpeed = new Vector2(rb.velocity.x, -1f);
                rb.velocity = slowSpeed;
            }
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < objectsGenerated.Count; i++)
        {
            GameObject obj = objectsGenerated[i];
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            Vector2 slowSpeed = new Vector2(rb.velocity.x, -5f);
            rb.velocity = slowSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        SDebug.Log("Collision Detected with " + collision.gameObject.name);
    }
}