using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
    

    void Start()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        
        _menuFirstButtons.Add(mainMenuPanel, mainMenuFirstButton);
        _menuFirstButtons.Add(settingsPanel, settingsFirstButton);
        
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuFirstButton);
    }

    void FixedUpdate()
    {
        lineLighter.position =  Vector3.SmoothDamp(lineLighter.position, EventSystem.current.currentSelectedGameObject.transform.position,ref velocity, smoothTime);
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene(1);
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
}
