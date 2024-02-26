using Core;

namespace UI
{
    public class HealthBar : StatBar
    {
        private void Awake()
        {
            Locator.OnPlayerChanged.AddListener(player => player.OnHealthChanged.AddListener(SetFill));
        }
    }
}