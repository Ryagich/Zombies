using System;
using UnityEngine;
using VContainer.Unity;

namespace Zombies.NPC
{
    /// <summary>Owns the current brain balance and its timed regeneration.</summary>
    public sealed class BrainController : IStartable, ITickable
    {
        private readonly BrainConfig config;
        private float elapsedSinceLastBrain;

        public int CurrentBrains { get; private set; }
        public int MaxBrains => config != null ? config.MaxBrains : 0;
        public float RestoreProgress => CurrentBrains >= MaxBrains || config == null
            ? 0f
            : Mathf.Clamp01(elapsedSinceLastBrain / config.SecondsPerBrain);

        public event Action Changed;

        public BrainController(BrainConfig config)
        {
            this.config = config;
            CurrentBrains = MaxBrains;
        }

        public void Start() { }

        public bool TrySpend(int amount)
        {
            if (amount < 0 || amount > CurrentBrains)
                return false;

            CurrentBrains -= amount;
            Changed?.Invoke();
            return true;
        }

        public void Tick()
        {
            if (config == null || CurrentBrains >= MaxBrains)
                return;

            elapsedSinceLastBrain += Time.deltaTime;
            while (elapsedSinceLastBrain >= config.SecondsPerBrain && CurrentBrains < MaxBrains)
            {
                elapsedSinceLastBrain -= config.SecondsPerBrain;
                CurrentBrains++;
            }

            if (CurrentBrains >= MaxBrains)
                elapsedSinceLastBrain = 0f;

            Changed?.Invoke();
        }
    }
}
