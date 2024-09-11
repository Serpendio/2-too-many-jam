using System.Collections.Generic;
using Core;
using Currency;
using UnityEngine;

namespace Creature
{
    public class Chest : CreatureBase
    {
        [SerializeField] private Sprite openSprite;
        [SerializeField] private Sprite emptySprite;
        private static CoinDrop _coinDropPrefab;
        private static ShardDrop _shardDropPrefab;

        private List<CoinDrop> _coinDrops = new();
        private List<ShardDrop> _shardDrops = new();

        protected override void Awake()
        {
            base.Awake();

            if (_coinDropPrefab == null) _coinDropPrefab = Resources.Load<CoinDrop>("Prefabs/CoinDrop");
            if (_shardDropPrefab == null) _shardDropPrefab = Resources.Load<ShardDrop>("Prefabs/Shard");
        }

        public override void TakeDamage(float damage)
        {
            spriteRenderer.sprite = openSprite;
            
            var totalValue = Locator.GameplaySettingsManager.ChestDropValue.GetValue();
            
            var droppedValue = 0f;
            while (droppedValue < totalValue)
            {
                var offset = transform.up * Random.Range(-0.25f, -0.75f) + transform.right * Random.Range(-0.5f, 0.5f);
                Debug.DrawRay(transform.position, offset, Color.red, 60f);

                var coinDrop = Instantiate(_coinDropPrefab, transform.position + offset, Quaternion.identity);
                coinDrop.transform.parent = transform;
                coinDrop.coinValue = Mathf.RoundToInt(Locator.GameplaySettingsManager.CoinDropValue.GetValue());
                _coinDrops.Add(coinDrop);
                droppedValue += coinDrop.coinValue;
                
                coinDrop.OnPickup.AddListener(() =>
                {
                    _coinDrops.Remove(coinDrop);
                    if (_coinDrops.Count == 0)
                    {
                        spriteRenderer.sprite = emptySprite;
                    }
                });

                if (Random.value > 0.8f)
                {
                    var shardDrop = Instantiate(_shardDropPrefab, transform.position + offset, Quaternion.identity);
                    shardDrop.transform.parent = transform;
                    shardDrop.shardValue = Random.Range(1, 4);
                    _shardDrops.Add(shardDrop);
                    droppedValue += shardDrop.shardValue;
                    shardDrop.OnPickupShard.AddListener(() =>
                    {
                        _shardDrops.Remove(shardDrop);
                        if (_shardDrops.Count == 0)
                        {
                            spriteRenderer.sprite = emptySprite;
                        }
                    });
                }
            }
            
            Locator.CreatureManager.RemoveCreature(this);
            Destroy(this); // remove chest component so we dont drop coins again

            // would make it white, but that's difficult so maybe not (would need a custom shader, or just replace the sprite for a frame?)
            
            // ^ note for todo: you can apparently do this by setting the material to something like "GUI Text Material"
            // for a few frames then setting it back
        }
    }
}