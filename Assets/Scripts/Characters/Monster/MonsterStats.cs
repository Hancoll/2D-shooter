using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStats : BaseStats
{
    public string MonsterName { get { return monsterName; } }
    [SerializeField] string monsterName;
    public int Damage { get { return damage; } }
    [SerializeField] int damage;
    public float AttackDistance { get { return attackDistance; } }
    [Min(2)] [SerializeField] float attackDistance = 2;
    public float AttackRate { get { return attackRate; } }
    [SerializeField] float attackRate;
    public int Knockback { get { return knockback; } }
    [SerializeField] int knockback = 1;
    public int MoveSpeed { get { return moveSpeed; } }
    [Min(1)] [SerializeField] int moveSpeed = 3;

    MonsterBaseAI monsterAI;

    [SerializeField] protected TextMesh healthValueText;

    protected override void Awake()
    {
        base.Awake();
        monsterAI = GetComponent<MonsterBaseAI>();
    }

    protected override void UpdateHealthBar()
    {
        base.UpdateHealthBar();

        if(Health <= 0)
            healthValueText.text = $"{monsterName}: 0 / {MaxHealth}";
        else
            healthValueText.text = $"{monsterName}: {Health} / {MaxHealth}";
    }

    public override void GetDamage(int damage, BaseStats attacker, Vector2 knockbackDirection)
    {
        Health -= damage;

        if (Health <= 0)
            MonsterDie();

        if (monsterAI.CheckNewTarget(attacker))
            monsterAI.SetTarget(attacker);
    }

    void MonsterDie()
    {
        IsDied = true;
        monsterAI.Animator.SetBool("isDied", true);
    }

    public override void EndDeathAnimation()
    {
        //Выпадение лута
        GetComponent<LootContainer>().DropItems(transform.position);

        //Уничтожение
        {
            GetComponent<GradualLoadingObject>().OnDestroyObject();
            Destroy(monsterAI.marker.gameObject);
            Destroy(gameObject);
        }
    }
}
