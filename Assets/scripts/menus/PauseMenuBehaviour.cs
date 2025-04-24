using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenuBehaviour : MonoBehaviour
{
    public static bool isPaused = false;
    private int menuAnimationState = 0;
    private Animator animator;
    
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    private GameObject activePanel;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    void Start()
    {
        activePanel = null;
        pauseMenuPanel.SetActive(false);
        StartCoroutine(LoadVolume());
    }

    IEnumerator LoadVolume()
    {
        settingsPanel.SetActive(true);
        yield return new WaitForEndOfFrame();
        settingsPanel.SetActive(false);
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)) && (activePanel == pauseMenuPanel || activePanel == null))
        {
            OpenClosePauseMenu(!pauseMenuPanel.activeSelf);
        }
        else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)) &&
                 (activePanel != pauseMenuPanel && activePanel != null))
        {
            OpenClosePauseMenu();
        }

        if ((Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.JoystickButton1)) && isPaused == true)
        {
            ClosePanel(activePanel);
        }
    }

    public void OpenClosePauseMenu(bool menuState = false)
    {
        
        settingsPanel.SetActive(false);
        if (menuState)
        {
            StartCoroutine(OpenPauseMenu());
        }

        else
        {
            StartCoroutine(ClosePauseMenu());
        }
    }

    IEnumerator OpenPauseMenu()
    {
        pauseMenuPanel.SetActive(true);
        animator.SetInteger("menuAnimationState", 1);
        isPaused = true;
        activePanel = pauseMenuPanel;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.5f);
    }
    
    IEnumerator ClosePauseMenu()
    {
        animator.SetInteger("menuAnimationState", 2);
        Time.timeScale = 1f;
        isPaused = false;
        yield return new WaitForSecondsRealtime(0.5f);
        pauseMenuPanel.SetActive(false);
        
        activePanel = null;
    }
    
    
    public void OpenPanel(GameObject panel)
    {
        pauseMenuPanel.SetActive(false);
        activePanel = panel;
        panel.SetActive(true);
    }

    public void ClosePanel(GameObject panel)
    {
        if (activePanel == pauseMenuPanel)
        {
            OpenClosePauseMenu(false);
        }
        else
        {
            pauseMenuPanel.SetActive(true);
            activePanel = pauseMenuPanel;
            panel.SetActive(false);
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
