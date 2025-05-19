using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tipsTool : MonoBehaviour
{
    public GameObject tipsCanvas;
    public Sprite[] tips;
    public Image tipImage;

    public enum TipType
    {
        Deplacements,
        Grab,
        DestructionMurs
    }

    public TipType tipType;

    private bool isActive = false;
    private bool alreadyUsed = false;
    
    void Start()
    {
        tipsCanvas.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        isActive = true;
        if(alreadyUsed == false)StartCoroutine(ShowTips());
        
    }

    private void OnTriggerExit(Collider other)
    {
        isActive = false;
        alreadyUsed = true;
    }

    public IEnumerator ShowTips()
    {
        
        float elapsedTime = -1;
        
        switch (tipType)
        {
            case TipType.Deplacements:
                tipImage.sprite = tips[0];
                yield return new WaitForSeconds(57f);
                break;
            case TipType.Grab:
                tipImage.sprite = tips[1];
                break;
            case TipType.DestructionMurs:
                tipImage.sprite = tips[2];
                break;
        }

        if (alreadyUsed)
        {
            yield break;
        }
        
        tipsCanvas.SetActive(true);
        while (isActive)
        {
            if(elapsedTime < 1)
            {
                elapsedTime += Time.fixedDeltaTime;
                Color newColor = new Color(tipImage.color.r, tipImage.color.g, tipImage.color.b, Mathf.Sin(elapsedTime * 2 )+1);
                tipImage.color = newColor;
            }
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        StartCoroutine(FadeOut());
        
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.fixedDeltaTime;
            Color newColor = new Color(tipImage.color.r, tipImage.color.g, tipImage.color.b, Mathf.Lerp(1f, 0f, elapsedTime));
            tipImage.color = newColor;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        tipsCanvas.SetActive(false);
    }
}