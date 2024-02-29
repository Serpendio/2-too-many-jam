using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//Pretty much the same as coin drop

namespace Currency
{
    public class ShardDrop : MonoBehaviour
    {
        public int shardValue = 1;

        [SerializeField] private float _baseMagnetSpeed = 2;
        [SerializeField] private float _magnetDistance = 3;
        [SerializeField] private float _pickupDistance = 1;

        public UnityEvent OnPickupShard = new();

        private void Update()
        {
            var distanceFromPlayer = Vector3.Distance(transform.position, Core.Locator.Player.transform.position);

            if (distanceFromPlayer < _pickupDistance)
            {
                Core.Locator.Inventory.Currency.AddSpellShards(shardValue);
                OnPickupShard.Invoke();
                Destroy(gameObject);
            }
            else if (distanceFromPlayer < _magnetDistance)
            {
                var speedMultiplier = (Mathf.InverseLerp(_magnetDistance, _pickupDistance, distanceFromPlayer) * 2) + 1;

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    Core.Locator.Player.transform.position,
                    _baseMagnetSpeed * speedMultiplier * Time.deltaTime
                );
            }
        }
    }
}
