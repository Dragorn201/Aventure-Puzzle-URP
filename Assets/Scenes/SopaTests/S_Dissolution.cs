using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class S_Dissolution : MonoBehaviour
{
    [SerializeField] public MeshRenderer mesh;
    private Material[] materials;

    public float dissolveRate = 0.0125f;
    public float refreshRate = 0.025f;
    public string parameter = "";

    void Start()
    {
        if(mesh != null)
            materials = mesh.materials;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DissolvingCo());
        }
    }

    IEnumerator DissolvingCo()
    {
        if (materials.Length > 0)
        {
            float counter = 0;

            while (materials[0].GetFloat(parameter) < 100)
            {
                counter += dissolveRate;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat(parameter, counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
