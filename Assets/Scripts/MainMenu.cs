using UnityEngine;

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
        Debug.Log("Main Menu -  Play Button pressed");
    }

    public void Options() 
    {
        Debug.Log("Main Menu - Options Button pressed");

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
        Debug.Log("Main Menu - Quit Button pressed");
        Application.Quit(); // toto funguje len v builde 
    }
}
