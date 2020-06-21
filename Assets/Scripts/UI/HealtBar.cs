using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealtBar : MonoBehaviour
{
    [SerializeField] Text healthText;
    [SerializeField] RectTransform healthScale;

    public void UpdateHealthBar(int health, int maxHealth)
    {
        if(health >= 0)
        {
            healthText.text = health + "/" + maxHealth;
            float xScale = (float)health / (float)maxHealth;
            healthScale.localScale = new Vector3(xScale, 1, 1);
        }

        else
        {
            healthText.text = 0 + "/" + maxHealth;
            float xScale = 0;
            healthScale.localScale = new Vector3(xScale, 1, 1);
        }
    }
}
