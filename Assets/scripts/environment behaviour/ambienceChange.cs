using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using System.Collections;

public class ambienceChange : MonoBehaviour
{
    public DirectionalLight directionalLight;
    public Volume prePurifiedVolume;
    public Volume postPurifiedVolume;

    private bool _switched;
    public float changingRate = 0.025f;

    void Awake()
    {
        prePurifiedVolume.weight = 1;
        postPurifiedVolume.weight = 0;
        _switched = false;
    }

    // void Update()
    // {
    //     if (Input.GetKey(KeyCode.P))
    //     {
    //         SwitchMood();
    //     }
    // }

    public void SwitchMood()
    {
        if (_switched == false)
        {
            StartCoroutine(ChangingMoodCo());
        }
    }

    IEnumerator ChangingMoodCo()
    {
        while (postPurifiedVolume.weight < 1)
        {
            postPurifiedVolume.weight += Time.deltaTime * changingRate;
            prePurifiedVolume.weight -= Time.deltaTime * changingRate;
            yield return null;
        }

        postPurifiedVolume.weight = 1;
        prePurifiedVolume.weight = 0;

        _switched = true;
    }
}
