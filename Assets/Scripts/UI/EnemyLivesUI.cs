using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyLivesUI : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    public List<Image> lifeIcons; 

    void Start()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged.AddListener(UpdateEnemyLives);
        }

        UpdateEnemyLives();
    }

    void OnDisable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged.RemoveListener(UpdateEnemyLives);
        }
    }

    void UpdateEnemyLives()
    {
        Debug.Log("Update enemy lives, current: " +  enemyHealth.currentEnemyHealth);

        for (int i = 0; i < enemyHealth.maxEnemyHealth; i++)
        {
            if (i < enemyHealth.currentEnemyHealth)
            {
                lifeIcons[i].enabled = true; 
            }
            else
            {
                lifeIcons[i].enabled = false;
            }
        }
    }
}
