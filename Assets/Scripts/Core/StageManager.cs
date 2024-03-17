using UnityEngine;

//Keeps track of user's current stage - increases when user beats a boss. Stage determines enemy difficulty.
namespace Core
{
    public class StageManager : MonoBehaviour
    {
        public int Stage = 1;
        public int LevelsPerStage = 10;
        public int MaxStage = 3;

        private void Awake()
        {
            Locator.ProvideStageManager(this);
        }

        public void AdvanceStage() => Stage += 1;
    }
}
