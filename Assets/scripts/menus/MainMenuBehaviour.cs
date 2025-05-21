using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject mainMenuFirstButton;
    
    public GameObject settingsPanel;
    public GameObject settingsFirstButton;
    
    private Dictionary<GameObject, GameObject> _menuFirstButtons = new Dictionary<GameObject, GameObject>();

    public RectTransform lineLighter;
    [SerializeField]private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    
    
    
    
    public List<ButtonIdentity> buttonsIdentities = new List<ButtonIdentity>();
    
    public Dictionary<GameObject, ButtonIdentity> buttons = new Dictionary<GameObject, ButtonIdentity>();
    private Transform lightTarget;

    public GameObject videoPlayer;
    public bool keyPressed = false;

    public GameObject creditsVideo;
    public float creditLength;
    
    

    void Start()
    {
        Time.timeScale = 1;
        keyPressed = false;
        foreach (ButtonIdentity button in buttonsIdentities)
        {
            buttons.Add(button.button, button);
        }
        
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        lineLighter.gameObject.SetActive(false);
        creditsVideo.SetActive(false);
        
        _menuFirstButtons.Add(mainMenuPanel, mainMenuFirstButton);
        _menuFirstButtons.Add(settingsPanel, settingsFirstButton);
        

        StartCoroutine(StopVideoPlayer());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Space))
        {
            keyPressed = true;
        }
    }

    IEnumerator StopVideoPlayer()
    {
        RawImage videoPlayerImage = videoPlayer.GetComponent<RawImage>();
        float elapsedTime = 0f;
        while (!keyPressed && elapsedTime < 12f)
        {
            elapsedTime += Time.fixedDeltaTime;
            if (elapsedTime >= 11f)
            {
                float newAlpha = Mathf.Lerp(1f, 0f, (elapsedTime - 11f));
                videoPlayerImage.color = new Color(1f, 1f, 1f, newAlpha);
            }
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("coroutine finished");
        StartMenu();
    }

    void StartMenu()
    {
        mainMenuPanel.SetActive(true);
        videoPlayer.SetActive(false);
        lineLighter.gameObject.SetActive(true);
        creditsVideo.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);
        lightTarget = buttons[_menuFirstButtons[mainMenuPanel]].buttonTransform;
    }

    void FixedUpdate()
    {
        foreach (KeyValuePair<GameObject, ButtonIdentity> button in buttons)
        {
            button.Value.textSprite.SetActive(false);
        }
        
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            lightTarget = buttons[mainMenuFirstButton].buttonTransform;
            lineLighter.position =  Vector3.SmoothDamp(lineLighter.position, lightTarget.position ,ref velocity, smoothTime);
            buttons[mainMenuFirstButton].textSprite.SetActive(true);
            return;
        }
        
        lightTarget = buttons[EventSystem.current.currentSelectedGameObject].buttonTransform;
        buttons[EventSystem.current.currentSelectedGameObject].textSprite.SetActive(true);
        lineLighter.position =  Vector3.SmoothDamp(lineLighter.position, lightTarget.position ,ref velocity, smoothTime);
    }
    
    public void StartGame()
    {
        SceneLoader.LoadGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenPanel(GameObject panel)
    {
        mainMenuPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_menuFirstButtons[panel]);
        panel.SetActive(true);
    }

    public void ClosePanel(GameObject panel)
    {
        mainMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);
        panel.SetActive(false);
    }

    public void OpenCredits()
    {
        StartCoroutine(Credits());
    }

    IEnumerator Credits()
    {
        mainMenuPanel.SetActive(false);
        creditsVideo.SetActive(true);
        keyPressed = false;
        
        float elapsedTime = 0f;
        while (elapsedTime < creditLength && !keyPressed)
        {
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        mainMenuPanel.SetActive(true);
        creditsVideo.SetActive(false);
    }
}

[System.Serializable]
public class ButtonIdentity
{
    public GameObject button;
    public GameObject textSprite;
    public Transform buttonTransform;
}
