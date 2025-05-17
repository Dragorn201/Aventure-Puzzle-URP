using UnityEngine;

public class CheatCodeManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] steps;

    private bool xPressed = false;
    
    private int currentStep = 0;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button18))xPressed = true;
        if (Input.GetKeyUp(KeyCode.Joystick1Button18))xPressed = false;
        
        if(Input.GetKeyDown(KeyCode.Joystick1Button8) && xPressed)GoNextStep();
        if(Input.GetKeyDown(KeyCode.Joystick1Button7) && xPressed)GoCurrentStep();
    }

    void GoNextStep()
    {
        currentStep++;
        player.transform.position = steps[currentStep].transform.position;
    }

    void GoCurrentStep()
    {
        if (player.transform.position != steps[currentStep].transform.position)
        {
            player.transform.position = steps[currentStep].transform.position;
        }
        else
        {
            GoPreviousStep();
        }
    }

    void GoPreviousStep()
    {
        currentStep--;
        player.transform.position = steps[currentStep].transform.position;
    }

    public void ChekPointReached(int cpId)
    {
        currentStep = cpId;
    }
}

//pour utiliser les cheatcodes , il faut maintenir X et utiliser la croix directionnelle de la manette
