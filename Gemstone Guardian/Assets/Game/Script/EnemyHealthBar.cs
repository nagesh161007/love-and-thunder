using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image HealthBarImage;
    public Canvas HealthBarCanvas;

    public void SetHealth(float health, float maxHealth)
    {
        HealthBarImage.fillAmount = health / maxHealth;
    }


    public void HideHealthBar()
    {
        if (HealthBarCanvas != null)
        {
            HealthBarCanvas.gameObject.SetActive(false); // Disable the canvas
        }
    }
}
