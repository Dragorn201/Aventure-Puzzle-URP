using UnityEngine;
using UnityEngine.VFX;

public class MagicHook : MonoBehaviour
{
    public VisualEffect hook;
    public GameObject hookManager;
    public GameObject killBox;

    void Awake()
    {
        hook.SetBool("isKilling", false);
    }
    void Start()
    {
        hook.SendEvent("OnPlay");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            RemoveChains();
        }
    }

    void RemoveChains()
    {
        hook.SetBool("isKilling", true);
        //killBox.transform.position = Vector3.Lerp(killBox.transform.position,hookManager.hit.point, elapsedTime / timeBeforePlayerMove);
    }
}
