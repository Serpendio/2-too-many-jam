using System.Collections;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    private IEnumerator Start()
    {
        var scales = new[] { 1, 2, 4 };
        foreach (var scale in scales)
        {
            var path = $"{Application.persistentDataPath}/sincantation{scale}.png";
            ScreenCapture.CaptureScreenshot(path, scale);
            Debug.Log($"Screenshot saved to: {path}");
            yield return new WaitForEndOfFrame();
        }
    }
}
