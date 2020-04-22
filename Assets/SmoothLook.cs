using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothLook : MonoBehaviour
{
    public Vector3 endRotation;
    public float lerpTime = 5;

    Vector3 startrotation;
    bool lerping = false;

    float t;
    float startTime;
    


    private void Start()
    {
        startrotation = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            lerping = true;
            startTime = Time.time;
        }

        if (lerping)
        {

            if (t < 1)
            {
                t = (Time.time - startTime) / lerpTime;
                Vector3 vector = new Vector3(Mathf.Lerp(startrotation.x, endRotation.x, t), startrotation.y);

                transform.eulerAngles = vector;
            }
        }
    }
}

