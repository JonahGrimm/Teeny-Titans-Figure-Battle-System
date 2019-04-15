using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tester class that was used to detect touches
public class UIDetection : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Touched via visualizer!");
    }
}
