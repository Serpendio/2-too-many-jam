using Currency;
using Core;
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
            var totalValue = Locator.GameplaySettingsManager.ChestDropValue.GetValue();
            var droppedValue = 0f;

            spriteRenderer.sprite = openChestSprite;
            while (droppedValue < totalValue)
            {
                var offset = transform.up * Random.Range(-0.25f, -0.75f) + transform.right * Random.Range(-0.5f, 0.5f);
                Debug.DrawRay(transform.position, offset, Color.red, 60f);

                var coinDrop = Instantiate(_coinDropPrefab, transform.position + offset, Quaternion.identity);
                coinDrop.coinValue = Mathf.RoundToInt(Locator.GameplaySettingsManager.CoinDropValue.GetValue());

                droppedValue += coinDrop.coinValue;
            }
            
            Locator.CreatureManager.RemoveCreature(this);
            Destroy(this); // remove chest component so we dont drop coins again

            // would make it white, but that's difficult so maybe not (would need a custom shader, or just replace the sprite for a frame?)
            
            // ^ note for todo: you can apparently do this by setting the material to something like "GUI Text Material"
            // for a few frames then setting it back
        }
    }
}