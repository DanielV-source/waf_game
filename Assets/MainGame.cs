using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    private int dif = 0; // Difficult
    private readonly List<float> difPlayerDamage = new() { 1.0f, 0.5f, 0.25f };
    private readonly List<float> difEnemyDamage = new() { 12.5f, 25f, 50f };
    private readonly List<float> difSpeed = new() { 2.5f, 5f, 10f };
    public static float currentPlayerDamage = 0f;
    public static float currentEnemyDamage = 0f;
    public static float currentMagicBallSpeed = 0f;
    public BlowScript blowScript;
    public BallSpawn ballSpawn;
    public GameObject enemy;
    public GameObject player;
    public GameObject hints;
    public GameObject magicBallULeft;
    public GameObject magicBallUCenter;
    public GameObject magicBallURight;
    public GameObject magicBallDLeft;
    public GameObject magicBallDCenter;
    public GameObject magicBallDRight;
    private readonly Color enableDamage = new(1f, 1f, 1f);
    private readonly Color disableDamage = new(0.22f, 0.22f, 0.22f);
    private List<GameObject> magicBalls = new();
    public static bool allThrown = false;
    public Transform magicPlayer;
    public readonly static float MAX_MAGIC = 6.9f;
    public readonly static float MAGIC_REGENERATION = 0.25f;

    public readonly static float HALF_HEALTH = 3.45f;
    public readonly static float LOW_HEALTH = 2f;
    public readonly static Color HALF_HEALTH_COLOR = new(1.0f, 153f / 255f, 0f, 1f);
    public readonly static Color LOW_HEALTH_COLOR = new(1.0f, 19f / 255f, 0f, 1f);
    

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        StartCoroutine(WaitForDifficulty());
    }

    IEnumerator WaitForDifficulty()
    {

        while (UnityReceiver.difficulty < 0)
        {
            Debug.Log("Waitting for difficulty!");
            yield return new WaitForSeconds(0.5f);
        }

        this.dif = UnityReceiver.difficulty % 3;

        MainGame.currentEnemyDamage = difEnemyDamage[dif];
        MainGame.currentPlayerDamage = difPlayerDamage[dif];
        MainGame.currentMagicBallSpeed = difSpeed[dif];
        magicBalls.Add(magicBallULeft);
        magicBalls.Add(magicBallUCenter);
        magicBalls.Add(magicBallURight);
        magicBalls.Add(magicBallDLeft);
        magicBalls.Add(magicBallDCenter);
        magicBalls.Add(magicBallDRight);
        hints.gameObject.SetActive(true);
        StartCoroutine(GenRandomSequence());
    }
    

    IEnumerator GenRandomSequence()
    {
        if(allThrown)
        {
            allThrown = false;
        }        

        hints.SetActive(true);
        for (int i = 0; i < magicBalls.Count; i++)
        {
            GameObject magicBall = magicBalls[i];
            magicBall.GetComponent<SpriteRenderer>().color = disableDamage;
            magicBall.GetComponent<BallScript>().doDoubleDamage = false;
            magicBall.GetComponent<BallScript>().doDamage = true;
        }
        

        int damageBalls = Random.Range(1, 5);
        List<GameObject> magicBallDamaged = new(magicBalls);
        for(int i = 0; i < damageBalls; i++)
        {
            int randomBall = Random.Range(0, magicBallDamaged.Count);
            GameObject damage = magicBallDamaged[randomBall];
            damage.GetComponent<BallScript>().doDoubleDamage = true;
            damage.GetComponent<SpriteRenderer>().color = enableDamage;
            magicBallDamaged.Remove(damage);
        }
        magicBallDamaged.Clear();

        yield return new WaitForSeconds(2f);

        hints.SetActive(false);

        UnityReceiver.ballAmount += magicBalls.Count;
        ballSpawn.SendSequence(new(magicBalls));
    }

    // Update is called once per frame
    void Update()
    {
        if(magicPlayer.localScale.x < MAX_MAGIC)
        {
            Vector3 magicRegeneration = new();
            magicRegeneration.x = MAGIC_REGENERATION;
            magicRegeneration.y = 0.0f;
            magicRegeneration.z = 0.0f;
            magicPlayer.localScale += magicRegeneration * Time.deltaTime;
        }
        if(allThrown && !BeamLogic.doingUlt)
        {
            StartCoroutine(GenRandomSequence());
        }
    }
}
