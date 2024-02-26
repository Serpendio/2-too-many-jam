﻿using Creature;
using UnityEngine.Events;

namespace Core
{
    public static class Locator
    {
        public static Player Player { get; private set; }
        public static CurrencyManager CurrencyManager { get; private set; }
        public static GameplaySettingsManager GameplaySettingsManager { get; private set; }
        public static CreatureManager CreatureManager { get; private set; }

        public static UnityEvent<Player> OnPlayerChanged = new();
        public static UnityEvent<CurrencyManager> OnCurrencyManagerChanged = new();
        public static UnityEvent<GameplaySettingsManager> OnGameplaySettingsManagerChanged = new();
        public static UnityEvent<CreatureManager> OnCreatureManagerChanged = new();

        public static void ProvidePlayer(Player player)
        {
            Player = player;
            OnPlayerChanged.Invoke(player);
        }

        public static void ProvideCurrencyManager(CurrencyManager currencyManager)
        {
            CurrencyManager = currencyManager;
            OnCurrencyManagerChanged.Invoke(currencyManager);
        }

        public static void ProvideGameplaySettingsManager(GameplaySettingsManager gameplaySettingsManager)
        {
            GameplaySettingsManager = gameplaySettingsManager;
            OnGameplaySettingsManagerChanged.Invoke(gameplaySettingsManager);
        }

        public static void ProvideCreatureManager(CreatureManager creatureManager)
        {
            CreatureManager = creatureManager;
            OnCreatureManagerChanged.Invoke(creatureManager);
        }
    }
}