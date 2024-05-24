using System.Collections;
using UnityEngine;

public class AccelerometerScript : MonoBehaviour
{
    public GameObject ultimate;
    public Transform magicBar;
    private bool isCoroutineRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        if (SystemInfo.supportsAccelerometer)
        {
            Debug.Log("Accelerometer is available on this device.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCoroutineRunning)
        {
            StartCoroutine(CheckShake());
        }
    }

    IEnumerator CheckShake()
    {
        isCoroutineRunning = true;

        Vector3 acceleration = Input.acceleration;
        if (((Mathf.Abs(acceleration.x) > 1.6f || Mathf.Abs(acceleration.y) > 1.6f || Mathf.Abs(acceleration.z) > 1.6f)) && !MainGame.allThrown)
        {
            if (magicBar.localScale.x >= MainGame.MAX_MAGIC)
            {
                ultimate.SetActive(true);
                UnityReceiver.ultimates += 1;
                BeamLogic.doingUlt = true;
                yield return new WaitForSeconds(4f);
                magicBar.localScale = new Vector2(0f, magicBar.localScale.y);
                BeamLogic.doingUlt = false;
                ultimate.SetActive(false);
            }
        }

        isCoroutineRunning = false;

        /* Old implementation - pause when shake */
        /*
        Vector3 acceleration = Input.acceleration;
        if (Mathf.Abs(acceleration.x) > 1.4f || Mathf.Abs(acceleration.y) > 1.4f || Mathf.Abs(acceleration.z) > 1.4f)
        {
            if (pause)
            {
                Time.timeScale = 1.0f;
                pause = false;
            }
            else
            {
                Time.timeScale = 0.0f;
                pause = true;
            }
            yield return new WaitForSeconds(1.75f);
        }*/
    }
}
