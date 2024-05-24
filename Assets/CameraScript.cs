using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public class CameraScript : MonoBehaviour
{
    private WebCamTexture webcamTexture;

    IEnumerator Start()
    {
        // Request camera permissions
        #if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
                yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.Camera));
                StartCoroutine(StartCamera());
            }
            else
            {
                StartCoroutine(StartCamera());
            }
        #endif

    }

    IEnumerator StartCamera()
    {
        WebCamDevice? backCamera = null;
        foreach (var device in WebCamTexture.devices)
        {
            if (!device.isFrontFacing)
            {
                backCamera = device;
                break;
            }
        }

        if (backCamera == null)
        {
            yield break;
        }

        webcamTexture = new WebCamTexture(backCamera.Value.name);
        RawImage rawImage = GetComponent<RawImage>();
        rawImage.color = new(255f, 255f, 255f, 255f);
        rawImage.texture = webcamTexture;
        webcamTexture.Play();

        yield return new WaitUntil(() => webcamTexture.width > 100);

        CorrectOrientation(rawImage);
        AdjustScale(rawImage);
    }

    void CorrectOrientation(RawImage rawImage)
    {
        RectTransform rectTransform = rawImage.rectTransform;
        rectTransform.localEulerAngles = new Vector3(0, 0, -webcamTexture.videoRotationAngle);

        if (webcamTexture.videoRotationAngle == 90 || webcamTexture.videoRotationAngle == 270)
        {
            float width = rectTransform.sizeDelta.x;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.y, width);
        }
    }

    void AdjustScale(RawImage rawImage)
    {
        RectTransform rectTransform = rawImage.rectTransform;
        int webcamWidth = webcamTexture.width;
        int webcamHeight = webcamTexture.height;
        float webcamAspectRatio = (float)webcamWidth / webcamHeight;

        float width = rectTransform.sizeDelta.x;
        float height = width / webcamAspectRatio;

        rectTransform.sizeDelta = new Vector2(width, height);
    }
}
