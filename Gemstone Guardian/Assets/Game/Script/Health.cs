using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;
    private EnemyHealthBar enemyHealthBar;

    public float CurrentHealthPercentage
    {
        get
        {
            return (float)CurrentHealth / (float)MaxHealth;
        }
    }

    private Character _cc;



    private void Awake()
    {
        CurrentHealth = MaxHealth;
        _cc = GetComponent<Character>();
        enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();

    }

    public void ApplyDamage(int damage)
    {
        CurrentHealth -= damage;
        Debug.Log(gameObject.name + "took damage: " + damage);
        Debug.Log(gameObject.name + " currentHealth: " + CurrentHealth);
        CheckHealth();
        if (enemyHealthBar != null)
        {
            enemyHealthBar.SetHealth(CurrentHealth, MaxHealth);
        }
    }

    private void CheckHealth()
    {
        if (CurrentHealth <= 0)
        {
            _cc.SwitchStateTo(Character.CharacterState.Dead);
            if (enemyHealthBar != null)
            {
                enemyHealthBar.HideHealthBar();
            }
        }
    }

    public void AddHealth(int health)
    {
        CurrentHealth += health;

        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
    }
}
