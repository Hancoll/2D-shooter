using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] HealtBar healtBar;
    
    public void UpdateHealthBar(int health, int maxHealth)
    {
        healtBar.UpdateHealthBar(health, maxHealth);
    }
}
