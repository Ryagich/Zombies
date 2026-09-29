using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zombies.NPC
{
    public sealed class HumansController
    {
        private readonly List<ManLifetimeScope> people = new();
        private readonly HashSet<ManLifetimeScope> deadPeople = new();

        public event Action<int, int> PeopleProgressChanged;

        public IReadOnlyList<ManLifetimeScope> People => people;
        public int CurrentPeopleCount { get; private set; }
        public int DeadPeopleCount => deadPeople.Count;
        public int TotalPeopleCount => people.Count;
        public int AlivePeopleCount { get; private set; }
        public event Action AllPeopleDied;

        public void SetLevelHumans(IReadOnlyList<ManLifetimeScope> levelPeople)
        {
            people.Clear();
            deadPeople.Clear();
            if (levelPeople != null)
            {
                foreach (var person in levelPeople)
                {
                    if (person != null && !people.Contains(person))
                        people.Add(person);
                }
            }

            AlivePeopleCount = people.Count;
            CurrentPeopleCount = DeadPeopleCount;
            PublishProgress();
        }

        public void Register(ManLifetimeScope person)
        {
            if (person == null || people.Contains(person))
                return;

            people.Add(person);
            AlivePeopleCount++;
            PublishProgress();
        }

        public void Unregister(ManLifetimeScope person)
        {
            // The level counter is historical: its denominator is the initial
            // population, even after the corresponding GameObject is deleted.
        }

        public void SetCurrentPeopleCount(int value)
        {
            var nextValue = Math.Clamp(value, 0, TotalPeopleCount);
            if (nextValue == CurrentPeopleCount)
                return;

            CurrentPeopleCount = nextValue;
            PublishProgress();
        }

        public void Clear() => SetLevelHumans(null);

        public ManLifetimeScope FindClosestAlive(Vector3 position)
        {
            ManLifetimeScope closest = null;
            var closestDistance = float.PositiveInfinity;
            foreach (var person in people)
            {
                if (person == null || person.Hp == null || person.Hp.IsDead)
                    continue;
                var distance = (person.transform.position - position).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closest = person;
                    closestDistance = distance;
                }
            }
            return closest;
        }

        public void MarkDead(ManLifetimeScope person)
        {
            if (person == null || !people.Contains(person) || !deadPeople.Add(person))
                return;
            AlivePeopleCount = Math.Max(0, TotalPeopleCount - DeadPeopleCount);
            CurrentPeopleCount = DeadPeopleCount;
            PublishProgress();
            if (AlivePeopleCount == 0)
                AllPeopleDied?.Invoke();
        }

        private void PublishProgress() => PeopleProgressChanged?.Invoke(CurrentPeopleCount, TotalPeopleCount);
    }
}
