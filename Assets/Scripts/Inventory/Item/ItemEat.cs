using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Eat", menuName = "Create ItemEat")]
public class ItemEat : Item
{
    public int HealthCount { get { return healthCount; } }
    [SerializeField] int healthCount;

    public override bool Execute()
    {
        if (GameManager.CharacterStats.Health < GameManager.CharacterStats.MaxHealth)
        {
            GameManager.CharacterStats.GetHealth(healthCount);
            return true;
        }

        else
            return false;
    }
}
