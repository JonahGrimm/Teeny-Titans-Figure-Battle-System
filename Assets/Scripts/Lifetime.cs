using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    public float seconds;

    // Update is called once per frame
    void Update()
    {
        seconds -= Time.deltaTime;

        if (seconds <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
