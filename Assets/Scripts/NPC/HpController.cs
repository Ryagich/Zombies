using System;

namespace Zombies.NPC
{
    public sealed class NpcHealth
    {
        public float MaxHp { get; }
        public float CurrentHp { get; private set; }
        public bool IsDead => CurrentHp <= 0f;

        public event Action<float, float> HpChanged;
        public event Action Died;

        public NpcHealth(HealthConfig config)
        {
            MaxHp = config != null ? config.MaxHp : 100f;
            RestoreFullHp();
        }

        public void TakeDamage(float damage)
        {
            if (damage <= 0f || IsDead)
                return;

            SetCurrentHp(CurrentHp - damage);
        }

        public void Heal(float amount)
        {
            if (amount <= 0f || IsDead)
                return;

            SetCurrentHp(CurrentHp + amount);
        }

        public void RestoreFullHp() => SetCurrentHp(MaxHp);

        private void SetCurrentHp(float value)
        {
            var wasAlive = !IsDead;
            CurrentHp = System.Math.Clamp(value, 0f, MaxHp);
            HpChanged?.Invoke(CurrentHp, MaxHp);

            if (wasAlive && IsDead)
                Died?.Invoke();
        }
    }
}
