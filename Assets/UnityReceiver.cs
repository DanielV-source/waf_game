using TMPro;
using UnityEngine;

public class UnityReceiver : MonoBehaviour
{
    public static string isWin = "lose";
    public static int difficulty = -1;
    public TextMeshPro textMeshPro;
    public static int fails = 0;
    public static int ballAmount = 0;
    public static int ultimates = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = "UnityReceiver";
        Debug.Log("UnityReceiver initialized and named.");
    }

    public void SetPlayerName(string playerName)
    {
        Debug.Log("PlayerName received from Android: " + playerName);
        textMeshPro.text = playerName;
    }

    public void SetDifficulty(string difficulty)
    {
        Debug.Log("Difficulty received from Android: " + difficulty);
        UnityReceiver.difficulty = int.Parse(difficulty);        
    }

    public static void SendDataToAndroid(string signal, string result, bool exit)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
                intentObject.Call<AndroidJavaObject>("setAction", "com.judc.walkfight.GAME_RESULT");
                intentObject.Call<AndroidJavaObject>("putExtra", signal, result);
                context.Call("sendBroadcast", intentObject);
                Debug.Log("Broadcast sent with data: " + result);
            }
        }
        Debug.Log("Data sent to Android: " + result);

        if(exit)
        {
            Application.Quit();
        }
    }
}
