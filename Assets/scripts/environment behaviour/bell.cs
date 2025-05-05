using UnityEngine;

public class bell : MonoBehaviour
{
    private Camera currentCamera;
    public Animation cameraMovement;

    public void StartEvent()
    {
        currentCamera.transform.GetComponent<CameraFollow>().enabled=false;
        cameraMovement.Play();
        currentCamera.transform.gameObject.SetActive(false);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
