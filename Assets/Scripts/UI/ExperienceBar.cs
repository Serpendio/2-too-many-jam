using Core;
using TMPro;

namespace UI
{
    public class ExperienceBar : StatBar
    {

        public override void SetFill(float value, float max)
        {
            base.SetFill(value, max);
            UpdateBar((int)Core.Locator.LevelManager.getCurrentLevel());
        }

        private void Start()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            UpdateBar(Core.Locator.LevelManager.getCurrentLevel());
            Core.Locator.LevelManager.OnPlayerLevelUp.AddListener(UpdateBar);
        }

        private void Awake()
        {
            Locator.OnLevelManagerChanged.AddListener(xp => xp.OnExperienceChanged.AddListener(SetFill));
        }

        private void UpdateBar(int val)
        {
            _text.text = val.ToString();
        }
    }
}