using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObjectSlow : MonoBehaviour {

    Transform _tr;
    Vector3 _offset_v3;

    public float X_Speed = 1.0f;
    public float Y_Speed = 2.0f;

    void Awake()
    {
        _tr = gameObject.transform;
        _offset_v3 = new Vector3(0, 0, 0);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    void LateUpdate()
    {
        _offset_v3.x = Time.deltaTime * X_Speed;
        _offset_v3.y = Time.deltaTime * Y_Speed;
        _tr.Rotate(_offset_v3);
    }
}
