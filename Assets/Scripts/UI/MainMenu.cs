using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject optionsPanel;

    public bool isOptionsOpen = false;
 

    void Start()
    {
        optionsPanel.SetActive(false);
        MusicManager.Instance.PlayMusic("MainMenu");
    }

    public void Play() 
    {
        SceneManager.LoadScene("Community Scene");
    }

    public void Options() 
    {

        if (isOptionsOpen) 
        {
            optionsPanel.SetActive(false);
            isOptionsOpen = false; 
        } 
        else 
        {
            optionsPanel.SetActive(true);
            isOptionsOpen = true;
        }
    }

    public void Quit()
    {
        Application.Quit(); // toto funguje len v builde 
    }
}
