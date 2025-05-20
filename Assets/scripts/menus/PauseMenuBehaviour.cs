using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
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
    
    public UnityEvent onCinematicSkip;
    public UnityEvent onPause;
    public UnityEvent onResume;

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
        
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Space))
        {
            keyPressed = true;
        }
    }
    
    IEnumerator StopVideoPlayer()
    {

        float videoLength = 0;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            videoLength = 57f;
        }
        else
        {
            videoLength = 7f;
        }
            
        
        
        RawImage videoPlayerImage = videoPlayer.GetComponent<RawImage>();
        float elapsedTime = 0;
        while (!keyPressed && elapsedTime < videoLength)
        {
            elapsedTime += Time.fixedDeltaTime;
            if (elapsedTime >= videoLength-1)
            {
                float newAlpha = Mathf.Lerp(1f, 0f, (elapsedTime - (videoLength-1)));
                videoPlayerImage.color = new Color(videoPlayerImage.color.r, videoPlayerImage.color.g, videoPlayerImage.color.b, newAlpha);
            }
            yield return new WaitForFixedUpdate();
        }

        if (keyPressed)
        {
            onCinematicSkip.Invoke();
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
        onPause.Invoke();
        yield return new WaitForSecondsRealtime(0.5f);
    }
    
    IEnumerator ClosePauseMenu()
    {
        animator.SetInteger("menuAnimationState", 2);
        Time.timeScale = 1f;
        isPaused = false;
        onResume.Invoke();
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
        
        activePanel = panel;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(panelsFistButtons[panel]);
        
        /*
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
        
        */
        Image panelImage = activePanel.GetComponent<Image>();
        Color originalColor = panelImage.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        panelImage.color = transparentColor;
        panel.SetActive(true);
        float elapsedTime = 0;
        while (elapsedTime < scaleAnimationDuration)
        {
            panelImage.color = Color.Lerp(transparentColor, originalColor, elapsedTime / scaleAnimationDuration);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        pauseMenuPanel.SetActive(false);
    }
    
    IEnumerator ClosePanelScaleAnimation(GameObject panel)
    {
        activePanel = pauseMenuPanel;
        
        /*
        Vector3 originalScale = panel.transform.localScale;
        float elapsedTime = 0f;
        pauseMenuPanel.SetActive(true);
        while (elapsedTime < scaleAnimationDuration)
        {
            panel.transform.localScale = Vector3.Lerp(panel.transform.localScale, Vector3.zero, elapsedTime / scaleAnimationDuration);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        */
        Image panelImage = panel.GetComponent<Image>();
        Color originalColor = panelImage.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        pauseMenuPanel.SetActive(true);
        float elapsedTime = 0;
        while (elapsedTime < scaleAnimationDuration)
        {
            panelImage.color = Color.Lerp(originalColor, transparentColor, elapsedTime / scaleAnimationDuration);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        
        
        panel.SetActive(false);
        //panel.transform.localScale = originalScale;
        panelImage.color = originalColor;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(panelsFistButtons[pauseMenuPanel]);
    }

    public void RetryLevel()
    {
        OpenClosePauseMenu(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        SceneLoader.LoadMenu();
    }
}
