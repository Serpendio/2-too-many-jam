using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuStart : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene("Rooms");
        }
    }
}
