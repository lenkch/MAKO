using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private UIInputActions inputActions;
    private InputAction pauseInput;

    public GameObject pauseMenuPanel;

    public bool isPaused = false;
    public GameObject optionsPanel;
    public bool isOptionsOpen = false;

    void Awake()
    {
        inputActions = new UIInputActions();
        pauseInput = inputActions.UI.PauseGame; 
        optionsPanel.SetActive(false);
        
    } 

    void Update()
    {
        if(isPaused)
        {
            pauseMenuPanel.SetActive(true);
        }
        else
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    void OnEnable()
    {
        pauseInput.performed += OnPauseGamePress;
        pauseInput.Enable();
    }

    void OnDisable()
    {
        pauseInput.Disable();
        pauseInput.performed -= OnPauseGamePress;
    }

    void OnPauseGamePress(InputAction.CallbackContext context)
    {
        pauseMenuPanel.SetActive(true);
        PauseGame();
    }
    void PauseGame()
    {
        pauseMenuPanel.SetActive(true);  
        Time.timeScale = 0f;            
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false); 
        Time.timeScale = 1f;             
        isPaused = false;
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
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
    public void QuitGame()
    {
        Time.timeScale = 1f;  
        Application.Quit();
    } 
}
