using TMPro;
using UnityEngine;

namespace UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ProjectVersionText : MonoBehaviour
    {
        private void OnEnable() => GetComponent<TextMeshProUGUI>().text = $"v{Application.version}";
    }
}