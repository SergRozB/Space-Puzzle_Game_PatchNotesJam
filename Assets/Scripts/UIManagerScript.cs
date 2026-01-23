using UnityEngine;

public class UIManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerInputManager playerInputManager;

    void Update()
    {
        if (playerInputManager.isPaused) 
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
        else 
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }

    public void Unpause() 
    {
        playerInputManager.isPaused = false;
    }
}
