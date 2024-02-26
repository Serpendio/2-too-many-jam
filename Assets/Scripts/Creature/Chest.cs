using Currency;
using System.Collections;
using System.Collections.Generic;
using Tweens;
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

        public override void TakeDamage(float damage)
        {
            spriteRenderer.sprite = openChestSprite;
            if (Random.value < Core.Locator.GameplaySettingsManager.CoinDropChance)
            {
                var coinDrop = Instantiate(_coinDropPrefab, transform.position, Quaternion.identity);
                coinDrop.coinValue = Mathf.RoundToInt(Core.Locator.GameplaySettingsManager.CoinDropValue.GetValue());
            }
            
            // would make it white, but that's difficult so maybe not (would need a custom shader, or just replace the sprite for a frame?)
        }
    }
}
