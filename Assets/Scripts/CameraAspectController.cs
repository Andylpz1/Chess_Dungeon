using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspectController : MonoBehaviour
{
    public Color letterboxColor = Color.black;

    private float targetAspect = 16f / 9f;
    private Camera cam;
    private Camera letterboxCam;

    private int lastScreenWidth;
    private int lastScreenHeight;

    void Start()
    {
        cam = GetComponent<Camera>();
        CreateLetterboxCamera();
        UpdateCameraViewport();
    }

    void Update()
    {
        // 动态检测屏幕变化
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            UpdateCameraViewport();
        }
    }

    void UpdateCameraViewport()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // 上下黑边
            cam.rect = new Rect(0, (1 - scaleHeight) / 2f, 1, scaleHeight);
        }
        else
        {
            // 左右黑边
            float scaleWidth = 1f / scaleHeight;
            cam.rect = new Rect((1 - scaleWidth) / 2f, 0, scaleWidth, 1);
        }

        // 更新记录
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    void CreateLetterboxCamera()
    {
        letterboxCam = new GameObject("LetterboxCamera").AddComponent<Camera>();
        letterboxCam.depth = cam.depth - 1;
        letterboxCam.clearFlags = CameraClearFlags.SolidColor;
        letterboxCam.backgroundColor = letterboxColor;
        letterboxCam.cullingMask = 0;
    }
}
