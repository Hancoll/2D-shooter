using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Single,//При стрельбе вылетает 1 пуля
    Burst,//При стрельбе пули вылетают друг за другом
    Shot,//При стрельбе вылетает дробь
}


[CreateAssetMenu(fileName = "Gun", menuName = "Create ItemGun")]
[System.Serializable]
public class ItemGun : Item
{
    public int AttackDamage { get { return attackDamage; } }
    [SerializeField] protected int attackDamage = 1;
    public float AttackRate { get { return attackRate; } }
    [SerializeField] protected float attackRate = 1;
    public float BulletSpeed { get { return bulletSpeed; } }
    [SerializeField] protected float bulletSpeed = 30;
    public int Knockback { get { return knockback; } }
    [SerializeField] protected int knockback = 1;
    public AttackType AttackType { get { return attackType; } }
    [SerializeField] protected AttackType attackType = AttackType.Single;

    public override bool Execute(ItemInventory bullets)
    {
        if (bullets != null && bullets.itemCount > 0 && bullets.item is ItemBullet)
            return true;
        else
            return false;
    }
}
