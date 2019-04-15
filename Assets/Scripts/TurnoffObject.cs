using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A small class that responds to animation events from the tappable buttons
public class TurnoffObject : MonoBehaviour
{
    ///<summary>Turns off the button gameobject after the disappear animation played</summary>
    void Turnoff()
    {
        gameObject.SetActive(false);
    }

    ///<summary>Reset the "Done" bool var in the animator controller 
    ///"Done" is dedicated to when the button has been exhausted and is disappearing</summary>
    void ResetDone()
    {
        gameObject.GetComponent<Animator>().SetBool("Done", false);
    }
}
