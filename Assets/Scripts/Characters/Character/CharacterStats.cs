using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : BaseStats
{
    CharacterController characterController;

    [SerializeField] CharacterUI characterUI;

    protected override void Awake()
    {
        base.Awake();

        characterController = GetComponent<CharacterController>();
    }

    protected override void Start()
    {
        base.Start();       
    }

    public override void GetHealth(int health)
    {
        Health += health;

        if (Health >= MaxHealth)
            Health = MaxHealth;
    }

    public override void GetDamage(int damage, BaseStats attacker, Vector2 knockbackDirection)
    {
        Health -= damage;

        if (Health <= 0)
        {
            //Анимация смерти и последующая логика

            IsDied = true;
        }
    }

    public override void EndDeathAnimation()
    {
        //Высвечивание окна возраждения
        Debug.Log("Died");
    }

    protected override void UpdateHealthBar()
    {
        characterUI.UpdateHealthBar(Health, MaxHealth);
    }

    public float CalculateDamage(ItemGun currentItemGun)
    {
        //Добавить доп урон от модификаторов
        float damage = currentItemGun.AttackDamage;
        return damage;
    }
}
