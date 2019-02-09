using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFallHelper : MonoBehaviour
{
    public bool Stop;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!Stop)
        {
            transform.Translate(new Vector3(0, 0, 0.5f * Time.deltaTime));
        }

        if (transform.localPosition.y < -150f)
        {
            Destroy(gameObject);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.name == "bg_1")
        {
            Stop = true;
        }
    }
}
