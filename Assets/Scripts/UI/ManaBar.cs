using Core;

namespace UI
{
    public class ManaBar : StatBar
    {
        private void Awake()
        {
            Locator.OnPlayerChanged.AddListener(player => player.OnManaChanged.AddListener(SetFill));
        }
    }
}