using System.Linq;
using Creature;
using Spells;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements.Experimental;

namespace Core
{
    public static class Locator
    {
        public static Player Player { get; private set; }
        public static Inventory Inventory { get; private set; }
        public static GameplaySettingsManager GameplaySettingsManager { get; private set; }
        public static CreatureManager CreatureManager { get; private set; }
        public static LevelManager LevelManager { get; private set; }
        public static StageManager StageManager { get; private set; }

        public static UnityEvent<Player> OnPlayerChanged = new();
        public static UnityEvent<GameplaySettingsManager> OnGameplaySettingsManagerChanged = new();
        public static UnityEvent<CreatureManager> OnCreatureManagerChanged = new();
        public static UnityEvent<LevelManager> OnLevelManagerChanged = new();
        public static UnityEvent<StageManager> OnStageManagerChanged = new();

        public static void ProvidePlayer(Player player)
        {
            Player = player;
            OnPlayerChanged.Invoke(player);
        }

        public static void ProvideInventory(Inventory inventory)
        {
            Inventory = inventory;
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

        internal static void ProvideLevelManager(LevelManager levelManager)
        {
            LevelManager = levelManager;
            OnLevelManagerChanged.Invoke(levelManager);
        }

        public static void ProvideStageManager(StageManager stageManager)
        {
            StageManager = stageManager;
            OnStageManagerChanged.Invoke(stageManager);
        }
    }
}