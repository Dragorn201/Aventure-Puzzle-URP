using UnityEngine;

public class CheatCodeManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] steps;

    private bool xPressed = false;
    private int currentStep = 0;

    private bool dpadInUse = false;

    void Update()
    {
        // X sur Xbox = JoystickButton2
        if (Input.GetKeyDown(KeyCode.Joystick1Button2)) xPressed = true;
        if (Input.GetKeyUp(KeyCode.Joystick1Button2)) xPressed = false;

        float dpadX = Input.GetAxisRaw("DPad_Horizontal");

        if (xPressed && !dpadInUse)
        {
            if (dpadX > 0.5f)
            {
                GoNextStep();
                dpadInUse = true;
            }
            else if (dpadX < -0.5f)
            {
                GoCurrentStep();
                dpadInUse = true;
            }
        }

        // Reset dès que la croix revient au neutre
        if (Mathf.Abs(dpadX) < 0.1f)
        {
            dpadInUse = false;
        }
    }

    void GoNextStep()
    {
        if (currentStep < steps.Length - 1)
        {
            currentStep++;
        }
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
        if (currentStep > 0)
        {
            currentStep--;
        }
        player.transform.position = steps[currentStep].transform.position;
    }

    public void ChekPointReached(int cpId)
    {
        currentStep = cpId;
    }
}