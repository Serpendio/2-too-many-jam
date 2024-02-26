using Creature;
using UnityEngine;

namespace Core
{
    public class Locator
    {
        public static Player Player { get; private set; }
        public static CurrencyManager CurrencyManager { get; private set; }
        public static GameplaySettingsManager GameplaySettingsManager { get; private set; }
        public static CreatureManager CreatureManager { get; private set; }

        public static void ProvidePlayer(Player player) =>
            Player = player;
        
        public static void ProvideCurrencyManager(CurrencyManager currencyManager) => 
            CurrencyManager = currencyManager;

        public static void ProvideGameplaySettingsManager(GameplaySettingsManager gameplaySettingsManager) =>
            GameplaySettingsManager = gameplaySettingsManager;

        public static void ProvideCreatureManager(CreatureManager creatureManager) =>
            CreatureManager = creatureManager;
    }
}