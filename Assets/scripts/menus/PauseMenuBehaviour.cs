using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuBehaviour : MonoBehaviour
{
    public static bool isPaused = false;
    public float scaleAnimationDuration = 0.5f;
    private Animator animator;
    
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    private GameObject activePanel;
    
    public GameObject pauseMenuFirstButton;
    public GameObject settingsMenuFirstButton;
    
    public Dictionary<GameObject, GameObject> panelsFistButtons = new Dictionary<GameObject, GameObject>();
    
    public GameObject videoPlayer;
    private bool keyPressed = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        panelsFistButtons.Add(pauseMenuPanel, pauseMenuFirstButton);
        panelsFistButtons.Add(settingsPanel, settingsMenuFirstButton);
    }
    
    void Start()
    {
        activePanel = null;
        pauseMenuPanel.SetActive(false);
        StartCoroutine(LoadVolume());
        StartCoroutine(StopVideoPlayer());
        videoPlayer.SetActive(true);
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
        
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            keyPressed = true;
        }
    }
    
    IEnumerator StopVideoPlayer()
    {

        RawImage videoPlayerImage = videoPlayer.GetComponent<RawImage>();
        float elapsedTime = 0;
        while (!keyPressed && elapsedTime < 7f)
        {
            elapsedTime += Time.fixedDeltaTime;
            if (elapsedTime >= 6f)
            {
                float newAlpha = Mathf.Lerp(1f, 0f, (elapsedTime - 6f));
                videoPlayerImage.color = new Color(1f, 1f, 1f, newAlpha * 255);
            }
            yield return new WaitForFixedUpdate();
        }
        pauseMenuPanel.SetActive(false);
        videoPlayer.SetActive(false);
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
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(panelsFistButtons[pauseMenuPanel]);
        
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
        StartCoroutine(OpenPanelScaleAnimation(panel));
    }

    public void ClosePanel(GameObject panel)
    {
        if (activePanel == pauseMenuPanel)
        {
            OpenClosePauseMenu(false);
        }
        else
        {
            StartCoroutine(ClosePanelScaleAnimation(panel));
        }
    }

    IEnumerator OpenPanelScaleAnimation(GameObject panel)
    {
        pauseMenuPanel.SetActive(false);
        activePanel = panel;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(panelsFistButtons[panel]);
        Vector3 originalScale = panel.transform.localScale;
        panel.transform.localScale = Vector3.zero;
        panel.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < scaleAnimationDuration)
        {
            panel.transform.localScale = Vector3.Lerp(panel.transform.localScale,originalScale , elapsedTime / scaleAnimationDuration);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
    }
    
    IEnumerator ClosePanelScaleAnimation(GameObject panel)
    {
        activePanel = pauseMenuPanel;
        
        Vector3 originalScale = panel.transform.localScale;
        float elapsedTime = 0f;
        pauseMenuPanel.SetActive(true);
        while (elapsedTime < scaleAnimationDuration)
        {
            panel.transform.localScale = Vector3.Lerp(panel.transform.localScale, Vector3.zero, elapsedTime / scaleAnimationDuration);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        panel.SetActive(false);
        panel.transform.localScale = originalScale;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(panelsFistButtons[pauseMenuPanel]);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
