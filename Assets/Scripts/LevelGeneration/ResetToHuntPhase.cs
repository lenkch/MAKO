using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetToHuntPhase : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entered ResetToHuntPhase trigger.");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Loading next scene: Hunt Scene");
            SceneManager.LoadScene("Hunt Scene");
        }
    }
}