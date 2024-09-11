using Spells;
using Spells.Modifiers;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class ShardManager : MonoBehaviour
    {
        [SerializeField] private int _shardsPerTier = 10;
        [SerializeField] private Image _flaskFillImage;

        [SerializeField] private Button _makeSpellButton;

        private int _tiers = 4;

        private void Awake()
        {
            Locator.Inventory.Currency.OnSpellShardChanged.AddListener(OnShardChanged);
            _flaskFillImage.fillAmount = 0;
            _makeSpellButton.interactable = false;

            _makeSpellButton.onClick.AddListener(() =>
            {
                var maxTier = Mathf.Min(Locator.Inventory.Currency.ShardAmount / _shardsPerTier, _tiers) - 1;
                var tier = (ShardSpellGenerator.ShardTiers)maxTier;
                ShardSpellGenerator.GenerateSpellFromShard(tier);
            });
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.P))
            {
                Locator.Inventory.Currency.AddSpellShards(4);
            }
#endif

            // check if inventory is full, if so, make button not interactable
            _makeSpellButton.interactable = Locator.Inventory.HasSpaceForItem
                                            && Locator.Inventory.Currency.ShardAmount >= _shardsPerTier;
        }

        // private void OnEnable()
        // {
        //     _flaskFillImage.gameObject.AddTween(new ImageFillAmountTween
        //     {
        //         from = _flaskFillImage.fillAmount * 0.99f,
        //         to = _flaskFillImage.fillAmount,
        //         duration = 2f,
        //         easeType = EaseType.SineInOut,
        //         isInfinite = true,
        //         usePingPong = true,
        //     });
        // }
        //
        // private void OnDisable()
        // {
        //     _flaskFillImage.gameObject.CancelTweens();
        // }

        private void OnShardChanged(int shardAmount)
        {
            var fillAmount = (float)shardAmount / (_shardsPerTier * (_tiers + 1));
            _flaskFillImage.fillAmount = fillAmount;
        }
    }
}