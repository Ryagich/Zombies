using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zombies.NPC;

namespace Zombies.UI
{
    public sealed class ZombieCard : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private Image fill;
        [SerializeField] private Image selectImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private Color availableCostColor = Color.white;
        [SerializeField] private Color unavailableCostColor = Color.red;

        private ZombieConfig config;
        private Action<ZombieConfig> selected;
        public ZombieConfig Config => config;

        public void Bind(ZombieConfig zombieConfig, Action<ZombieConfig> onSelected)
        {
            config = zombieConfig;
            selected = onSelected;
            if (button != null)
                button.onClick.AddListener(OnClicked);

            if (timeText != null)
                timeText.text = $"{config.SpawnCooldown:0.#}s";
            if (costText != null)
                costText.text = config.BrainCost.ToString();
            if (iconImage != null)
            {
                iconImage.sprite = config.Icon;
                iconImage.gameObject.SetActive(config.Icon != null);
            }
        }

        public void Refresh(int brains, float cooldownProgress, bool isSelected)
        {
            if (costText != null)
                costText.color = brains >= config.BrainCost ? availableCostColor : unavailableCostColor;
            if (fill != null)
                fill.fillAmount = 1f - cooldownProgress;
            if (selectImage != null)
                selectImage.gameObject.SetActive(isSelected);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked() => selected?.Invoke(config);
    }
}
