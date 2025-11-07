using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Play() 
    {
        Debug.Log("Main Menu -  Play Button pressed");
    }

    public void Options() 
    {
        Debug.Log("Main Menu - Options Button pressed");
    }

    public void Quit()
    {
        Debug.Log("Main Menu - Quit Button pressed");
        Application.Quit(); // toto funguje len v builde 
    }
}
