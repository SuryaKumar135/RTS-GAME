using System;
using UnityEngine;

namespace RTSGame
{
    [Serializable]
    public class CommonData
    {
        public Sprite Icon;
        public string Name;
        public string Description;

        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float health = 100f;

        public float MaxHealth
        {
            get => maxHealth;
            set
            {
                maxHealth = Mathf.Max(1f, value); // Ensure maxHealth is always at least 1
                Health = Mathf.Min(Health, maxHealth); // Clamp current health to new max
            }
        }

        public float Health
        {
            get => health;
            set => health = Mathf.Clamp(value, 0f, maxHealth);
        }

        // Optional: A method to apply damage or healing
        public void ModifyHealth(float amount)
        {
            Health += amount; // Will auto-clamp due to property setter
        }
    }
}
