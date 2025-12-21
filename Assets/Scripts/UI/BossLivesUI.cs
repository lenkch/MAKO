using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class BossLivesUI : MonoBehaviour
{
    public BossHealth bossHealth;
    public List<Image> lifeIcons; 

    void Start()
    {
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged.AddListener(UpdateBossLives);
        }

        UpdateBossLives();
    }

    void OnDisable()
    {
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged.RemoveListener(UpdateBossLives);
        }
    }

    void UpdateBossLives()
    {

        for (int i = 0; i < bossHealth.maxBossHealth; i++)
        {
            if (i < bossHealth.currentBossHealth)
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
