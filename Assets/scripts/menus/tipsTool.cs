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

    void Start()
    {
        tipsCanvas.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        isActive = true;
        StartCoroutine(ShowTips());
    }

    private void OnTriggerExit(Collider other)
    {
        isActive = false;
    }

    public IEnumerator ShowTips()
    {
        
        float elapsedTime = -1;
        
        switch (tipType)
        {
            case TipType.Deplacements:
                tipImage.sprite = tips[0];
                yield return new WaitForSeconds(10f);
                break;
            case TipType.Grab:
                tipImage.sprite = tips[1];
                break;
            case TipType.DestructionMurs:
                yield return new WaitForSeconds(5f);
                tipImage.sprite = tips[2];
                break;
        }

        tipsCanvas.SetActive(true);
        while (isActive)
        {
            elapsedTime += Time.fixedDeltaTime;
            Color newColor = new Color(tipImage.color.r, tipImage.color.g, tipImage.color.b, Mathf.Sin(elapsedTime * 2 )+1);
            tipImage.color = newColor;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        tipsCanvas.SetActive(false);
        
    }
}