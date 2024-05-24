using UnityEngine;
using System.Linq;
using UnityEngine.Android;
using System.Collections;

public class BlowScript : MonoBehaviour
{
    private AudioClip microphoneInput;
    public static bool isBlowing = false;
    public static bool canBlow = false;
    public float sensitivity = 100;
    public float[] threshold = { 0.05f, 0.05f, 0.05f };
    public int difficulty = 0;
    public Transform playerMagic;
    float timer = 0.0f;

    IEnumerator Start()
    {
        // Request microphone permission at runtime for Android
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.Microphone));
        }
        InitializeMicrophone();
    }

    void InitializeMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneInput = Microphone.Start(null, true, 1, 44100);
            Debug.Log("Microphone started successfully. Total available mics: " + Microphone.devices.Length);
        }
        else
        {
            Debug.LogError("No microphone devices found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (microphoneInput == null) return;

        timer += Time.deltaTime;
        int micPosition = Microphone.GetPosition(null);
        if (micPosition < 2048) return;

        micPosition -= 2048;
        int numSamples = Mathf.Min(2048, microphoneInput.samples - micPosition);
        if (numSamples <= 0) return;

        float[] waveData = new float[numSamples];
        microphoneInput.GetData(waveData, micPosition);

        float levelMax = GetMaxAbsoluteValue(waveData);

        CheckBlowing(levelMax);
    }

    float GetMaxAbsoluteValue(float[] data)
    {
        float max = 0f;
        foreach (float d in data)
        {
            float abs = Mathf.Abs(d);
            if (abs > max) max = abs;
        }
        return max;
    }

    void CheckBlowing(float levelMax)
    {
        bool currentlyBlowing = levelMax > 0.275f;

        if (currentlyBlowing && playerMagic.localScale.x > 0.35f)
        {
            Debug.Log("Blow detected, Level: " + levelMax);
            BlowScript.isBlowing = true;

            if(playerMagic.localScale.x > 0)
            {
                Vector3 newScale = transform.localScale;
                newScale.x = 2.5f * Time.deltaTime;
                newScale.y = 0.0f;
                newScale.z = 0.0f;
                if ((playerMagic.localScale.x - newScale.x) <= 0.0f)
                {
                    newScale.x = playerMagic.localScale.x;
                    playerMagic.localScale -= newScale;
                }
                else
                {
                    playerMagic.localScale -= newScale;
                }
            }
        }
        else if (!currentlyBlowing && BlowScript.isBlowing)
        {
            Debug.Log("Blowing stopped.");
            BlowScript.isBlowing = false;
            canBlow = false;
        }
    }
}
