using System;
using UnityEngine;

public class CheckPointStep : MonoBehaviour
{
    public CheatCodeManager cheatCodeManager;
    public int iD;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cheatCodeManager.ChekPointReached(iD);
        }
        
    }
}
