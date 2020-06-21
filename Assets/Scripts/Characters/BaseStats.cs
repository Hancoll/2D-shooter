using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStats : MonoBehaviour
{
    public int MaxHealth { get { return maxHealth; } }
    [SerializeField] int maxHealth;
    public int Health { get { return health; } protected set { health = value; UpdateHealthBar(); } }
    [SerializeField] int health;

    public System.Action OnDied;
    public bool IsDied { get { return isDied; }
        set
        {
            if (value)
            {
                OnDied?.Invoke();
                gameObject.layer = GameManager.DiedLayerMask;
            }

            isDied = value;
        }
    }
    bool isDied = false;

    protected virtual void Awake() { InitializeHealth(); }

    protected virtual void Start() { }

    public virtual void GetHealth(int health) { }

    public virtual void GetDamage(int damage, BaseStats attacker, Vector2 knockbackDirection) { }

    public virtual void EndDeathAnimation() { }

    protected virtual void UpdateHealthBar() { }

    void InitializeHealth() => Health = MaxHealth;

    void InitializeHealth(int health) => this.Health = health;
}
