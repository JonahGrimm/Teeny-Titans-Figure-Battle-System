using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Updates various continuous mechanics
public class GameManager : MonoBehaviour
{
    public Image[] fillbars;
    public float fillSpeed = .20f;
    public float firstAttack = .40f;
    public float secondAttack = 1f;
    public GameObject[] leftButtons;
    public GameObject[] rightButtons;

    private GameObject[,] buttons = new GameObject[2,2];
    private bool[,] buttonStates = new bool[2,2];
    private TransitionCameras trans;
    private bool canGrowMeter;

    //Initialize
    void Start()
    {
        trans = FindObjectOfType<TransitionCameras>() as TransitionCameras;
        buttons[0, 0] = leftButtons[0];
        buttons[0, 1] = leftButtons[1];
        buttons[1, 0] = rightButtons[0];
        buttons[1, 1] = rightButtons[1];
        canGrowMeter = true;

        for (int i = 0; i < buttonStates.GetLength(0); i++)
        {
            for (int j = 0; j < buttonStates.GetLength(1); j++)
            {
                buttonStates[i, j] = false;
            }
        }
    }

    void Update()
    {
        //If the meter can grow
        if (canGrowMeter)
        {
            //For every fillbar that will be grown
            for (int i = 0; i < fillbars.Length; i++)
            {
                //Grow the fillbar
                fillbars[i].fillAmount += fillSpeed * Time.fixedDeltaTime;

                //If the fillbar has surpassed the first attack button threshold and HASN'T turned on yet
                if (fillbars[i].fillAmount >= firstAttack && !buttonStates[i, 0])
                {
                    //On
                    buttons[i, 0].SetActive(true);
                    buttonStates[i, 0] = true;
                }
                //If the fillbar has fell below the first attack button threshold and IS turned on
                else if (fillbars[i].fillAmount < firstAttack && buttonStates[i, 0])
                {
                    //Off
                    var animator = buttons[i, 0].GetComponent<Animator>();
                    animator.SetBool("Done", true);
                    buttonStates[i, 0] = false;
                }

                //If the fillbar has surpassed the second attack button threshold and HASN'T turned on yet
                if (fillbars[i].fillAmount >= secondAttack && !buttonStates[i, 1])
                {
                    //On
                    buttons[i, 1].SetActive(true);
                    buttonStates[i, 1] = true;
                }
                //If the fillbar has fell below the second attack button threshold and IS turned on
                else if (fillbars[i].fillAmount < secondAttack && buttonStates[i, 1])
                {
                    //Off
                    var animator = buttons[i, 1].GetComponent<Animator>();
                    animator.SetBool("Done", true);
                    buttonStates[i, 1] = false;
                }
            }
        }       
    }

    //When InputManager finds something, it calls this method for GameManager to decide what to do with it
    ///<summary>Makes a decision on what to do with the UI element that was just pressed</summary>
    public void respondToInput(GameObject target)
    {
        //Uses a switch statement for all possible cases
        //If statements would work fine here, but a switch statement would be better in the long term
        switch (target.name)
        {
            //If it's the projectile button
            case "Projectile":
                //Was the button on the left or right side of the screen?
                if (target.tag == "Left")
                {
                    //Attempt to transition camera to fullscreen
                    if (trans.Transition(Direction.Left, CinematicType.Projectile))
                    {
                        //Transition was successful. Now it will subtract the attack cost
                        //and create a slight delay before locking meter growth
                        fillbars[0].fillAmount -= firstAttack;
                        StartCoroutine(SlightDelayForMeter());
                    }
                }
                else
                {
                    //Attempt to transition camera to fullscreen
                    if (trans.Transition(Direction.Right, CinematicType.Projectile))
                    {
                        //Transition was successful. Now it will subtract the attack cost
                        //and create a slight delay before locking meter growth
                        fillbars[1].fillAmount -= firstAttack;
                        StartCoroutine(SlightDelayForMeter());
                    }
                }
                break;

            //If it's the sword button
            case "Sword":
                //Was the button on the left or right side of the screen?
                if (target.tag == "Left")
                {
                    //Attempt to transition camera to fullscreen
                    if (trans.Transition(Direction.Left, CinematicType.Sword))
                    {
                        //Transition was successful. Now it will subtract the attack cost
                        //and create a slight delay before locking meter growth
                        fillbars[0].fillAmount -= secondAttack;
                        StartCoroutine(SlightDelayForMeter());
                    }
                }
                else
                {
                    //Attempt to transition camera to fullscreen
                    if (trans.Transition(Direction.Right, CinematicType.Sword))
                    {
                        //Transition was successful. Now it will subtract the attack cost
                        //and create a slight delay before locking meter growth
                        fillbars[1].fillAmount -= secondAttack;
                        StartCoroutine(SlightDelayForMeter());
                    }
                }
                break;
        }
    }

    //Called by TransitionCameras to continue the meter growth
    ///<summary>Resumes meter growth</summary>
    public void ResumeMeter()
    {
        canGrowMeter = true;
    }

    ///<summary>Waits one frame to lock meter so that it can trigger the respective UI elements to turn off</summary>
    IEnumerator SlightDelayForMeter()
    {
        yield return null;
        canGrowMeter = false;
    }
}
