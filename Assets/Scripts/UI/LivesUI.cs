using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LivesUI : MonoBehaviour
{
    public PlayerStats playerStats; 

    private int maxLives;

    public Sprite fullLive;
    public Sprite emptyLive;

    public List<Image> playerLives  = new List<Image>();

    private IEnumerator Start()
    {
        yield return null;

        maxLives = playerStats.maxLives; 
        playerStats.OnLivesChanged += UpdatePlayerLives;

        UpdatePlayerLives();

    }

    private void OnDestroy()
    {
        playerStats.OnLivesChanged -= UpdatePlayerLives;
    }

    void UpdatePlayerLives() 
    {

        int currentLives = playerStats.currentLives; 

        Debug.Log("Current lives: " + currentLives + " max lives: " + maxLives);

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
