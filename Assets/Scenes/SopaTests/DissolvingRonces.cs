using UnityEngine;
using System.Collections;

public class DissolvingRonces : MonoBehaviour
{
    public Material material;
#if UNITY_EDITOR
    Material mat2;
#endif

    public float dissolveRate = 0.0125f;
    public float refreshRate = 0.025f;
    public string parameter = "";

    void Awake()
    {
        material.SetFloat(parameter, 0);
#if UNITY_EDITOR
        mat2 = new Material(material);
#endif
    }

    IEnumerator DissolvingCo()
    {
        float counter = 0;

        while (material.GetFloat(parameter) < 100)
        {
            counter += dissolveRate;
            material.SetFloat(parameter, counter);

            yield return new WaitForSeconds(refreshRate);
        }
    }

    public void BeginDissolving()
    {
        StartCoroutine(DissolvingCo());
    }
    
#if UNITY_EDITOR
    void OnApplicationQuit()
    {
        material.CopyPropertiesFromMaterial(mat2);
    }
#endif
}