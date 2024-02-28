using Core;
using UnityEngine;
using UnityEngine.Events;

namespace Currency
{
    public class CoinDrop : MonoBehaviour
    {
        public int coinValue = 1;

        [SerializeField] private float _baseMagnetSpeed = 2;
        [SerializeField] private float _magnetDistance = 3;
        [SerializeField] private float _pickupDistance = 1;

        public UnityEvent OnPickup = new();

        private void Update()
        {
            var distanceFromPlayer = Vector3.Distance(transform.position, Locator.Player.transform.position);
            
            if (distanceFromPlayer < _pickupDistance)
            {
                Locator.Inventory.AddGold(coinValue);
                OnPickup.Invoke();
                Destroy(gameObject);
            }
            else if (distanceFromPlayer < _magnetDistance)
            {
                var speedMultiplier = (Mathf.InverseLerp(_magnetDistance, _pickupDistance, distanceFromPlayer) * 2) + 1;

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    Locator.Player.transform.position,
                    _baseMagnetSpeed * speedMultiplier * Time.deltaTime
                );
            }
        }
    }
}