using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Zombies.NPC
{
    public sealed class ZombieSelectionController : IStartable, ITickable
    {
        private readonly ZombieStorage storage;
        private readonly BrainController brains;
        private readonly Dictionary<ZombieConfig, float> cooldowns = new();

        public ZombieConfig SelectedConfig { get; private set; }
        public event Action Changed;

        public ZombieSelectionController(ZombieStorage storage, BrainController brains)
        {
            this.storage = storage;
            this.brains = brains;
        }

        public void Start()
        {
            if (storage != null && storage.SortedConfigs.Count > 0)
                Select(storage.SortedConfigs[0]);
        }

        public void Select(ZombieConfig config)
        {
            if (config == null || SelectedConfig == config)
                return;

            SelectedConfig = config;
            Changed?.Invoke();
        }

        public bool CanSpawn(ZombieConfig config) => config != null
            && GetCooldownProgress(config) >= 1f
            && brains.CurrentBrains >= config.BrainCost
            && config.ZombiePrefab != null;

        public float GetCooldownProgress(ZombieConfig config)
        {
            if (config == null || config.SpawnCooldown <= 0f)
                return 1f;

            return Mathf.Clamp01(cooldowns.TryGetValue(config, out var elapsed) ? elapsed / config.SpawnCooldown : 1f);
        }

        public bool TryConsumeSelected()
        {
            if (!CanSpawn(SelectedConfig) || !brains.TrySpend(SelectedConfig.BrainCost))
                return false;

            cooldowns[SelectedConfig] = 0f;
            Changed?.Invoke();
            return true;
        }

        public void Tick()
        {
            var changed = false;
            if (storage != null)
            {
                foreach (var config in storage.SortedConfigs)
                {
                    if (config == null || config.SpawnCooldown <= 0f || GetCooldownProgress(config) >= 1f)
                        continue;

                    cooldowns[config] = Mathf.Min(config.SpawnCooldown, cooldowns[config] + Time.deltaTime);
                    changed = true;
                }
            }

            if (changed)
                Changed?.Invoke();
        }
    }
}
