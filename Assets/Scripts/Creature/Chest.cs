using Currency;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Creature
{
    public class Chest : CreatureBase
    {
        [SerializeField] private Sprite openChestSprite;
        private static CoinDrop _coinDropPrefab;

        protected override void Awake()
        {
            base.Awake();

            if (_coinDropPrefab == null) _coinDropPrefab = Resources.Load<CoinDrop>("Prefabs/CoinDrop");
        }

        //Replace with animator stuff?
        protected override void Die() {
            spriteRenderer.sprite = openChestSprite;
            if (Random.value < Core.Locator.GameplaySettingsManager.CoinDropChance)
            {
                var coinDrop = Instantiate(_coinDropPrefab, transform.position, Quaternion.identity);
                coinDrop.coinValue = Mathf.RoundToInt(Core.Locator.GameplaySettingsManager.CoinDropValue.GetValue());
            }
        }
    }
}
