using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyLivesUI : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    public List<Image> lifeIcons; 

    void OnEnable()
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
