using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LivesUI : MonoBehaviour
{
    public int maxLives = 9;
    public int currentLives = 9;

    public Sprite fullLive;
    public Sprite emptyLive;

    public List<Image> playerLives  = new List<Image>();

    void Start()
    {
        SetLives();
        UpdatePlayerLives();

        // pre testovanie, kazde 3s player dostane 1 damage/heal
        //StartCoroutine(TestRoutine());
    }

    private IEnumerator TestRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            RestoreLives(1);
            //akeDamage(1);
        }
    }

    public void TakeDamage(int damage) 
    {
        currentLives -= damage;

        // nemoze byt menej ako 0 zivotov
        if (currentLives < 0) 
        {
            currentLives = 0;
        }

        UpdatePlayerLives();
    }

    public void RestoreLives(int healing) 
    {
        currentLives += healing;

        if (currentLives > 9) 
        {
            currentLives = 9;
        }

        UpdatePlayerLives();
    }

    // inicialne nacitanie zivotov (bez animacii)
    void SetLives() 
    {
        for (int i = 0; i < maxLives; i++) {
            if (i < currentLives) 
            {
                playerLives[i].sprite = fullLive;
            }
            else 
            {
                playerLives[i].sprite = emptyLive;
            }
        }
    }

    void UpdatePlayerLives() 
    {
        for (int i = 0; i < maxLives; i++) 
        {
            if (i < currentLives) 
            {
                if (playerLives[i].sprite != fullLive)
                {
                    //Debug.Log("Heart: " + i + ", started heal animation");
                    StartCoroutine(HealHeartAnimation(playerLives[i].rectTransform));
                }
                playerLives[i].sprite = fullLive;
            }
            else 
            {
                // animacia sa vykona iba pre ten zivot ktory dostal damage
                if (playerLives[i].sprite != emptyLive)
                {
                    //Debug.Log("Heart: " + i + ", started damage animation");
                    StartCoroutine(ShakeHartAnimation(playerLives[i].rectTransform));
                }
                playerLives[i].sprite = emptyLive;

            }
        }
    }

    /* A N I M A T I O N S */

    // animacia zivotov ked player dostane damage
    private IEnumerator ShakeHartAnimation(RectTransform heart)
    {
        Vector2 originalPosition = heart.anchoredPosition;

        float animDuration = 0.2f;
        float animIntensity = 4f;
        float animElapsed = 0f;

        while (animElapsed < animDuration)
        {
            float xPos = Random.Range(-animIntensity, animIntensity);
            float yPos = Random.Range(-animIntensity, animIntensity);

            heart.anchoredPosition = originalPosition + new Vector2(xPos, yPos);

            animElapsed += Time.deltaTime;
            yield return null;
        }

        heart.anchoredPosition = originalPosition;
    }

    // animacia healingu
    private IEnumerator HealHeartAnimation(RectTransform heart)
    {
        Vector2 originalScale = heart.localScale;

        float animSmallScale = 0.5f;
        float animBigScale = 1.5f;

        float animDuration = 1f;
        float animElapsed = 0f;

        while (animElapsed < animDuration) 
        {
            float scale = Mathf.Lerp(animSmallScale, animBigScale, animElapsed);
            heart.localScale = originalScale * scale;

            animElapsed += Time.deltaTime * 5f; // ten koef. tu vplyva na rychlost animacie
            yield return null;
        }

        heart.localScale = originalScale;
    }
}
