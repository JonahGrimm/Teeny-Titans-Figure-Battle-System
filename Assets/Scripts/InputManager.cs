using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the touches and detection
public class InputManager : MonoBehaviour
{
    public float touchRadius;
    private GameManager manager;

    private void Start()
    {
        manager = FindObjectOfType<GameManager>() as GameManager;
    }

    void Update()
    {
        //For each touch
        foreach (Touch touch in Input.touches)
        {
            //If lifting up finger
            if (touch.phase == TouchPhase.Ended)
            {
                RaycastHit2D hit = Physics2D.CircleCast(touch.position, touchRadius, Vector2.down, 0.01f);

                //If the finger touched a button
                if (hit.collider != null)
                {
                    GameObject touchedObject = hit.transform.gameObject;

                    //Tell the manager that it hit something, and it should take action
                    manager.respondToInput(touchedObject);
                }
            }
        }
    }
}
