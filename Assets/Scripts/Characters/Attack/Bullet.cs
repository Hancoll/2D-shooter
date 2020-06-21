using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    int damage;
    int knockback;
    BaseStats sender;

    public void Initialize(int damage, int knockback,  BaseStats sender)
    {
        this.damage = damage;
        this.knockback = knockback;
        this.sender = sender;
    }

    public void DestroyBullet()
    {
        GetComponent<GradualLoadingObject>().OnDestroyObject();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<BaseStats>() != null)
        {
            BaseStats baseStats = collider.GetComponent<BaseStats>();

            if ((sender as CharacterStats && baseStats as MonsterStats)
                || (sender as MonsterStats && baseStats as CharacterStats))
            {
                Vector2 direction = (transform.position - baseStats.transform.position).normalized;
                baseStats.GetDamage(damage, sender, -direction * knockback);

                //Сделать анимацию взрыва у пули
                DestroyBullet();
            }

            else
                return;
        }

        else
            DestroyBullet();
    }
}
