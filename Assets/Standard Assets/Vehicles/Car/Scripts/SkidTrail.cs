using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class SkidTrail : MonoBehaviour
    {
        [SerializeField] private float m_PersistTime;
        Transform m_SkidTrail;
        TrailRenderer TR;
        Rigidbody Parent_rigidbody;


        private void Start()
        {
            TR = GetComponent<TrailRenderer>();
            

        }
        

        //public IEnumerator SkidStart()
        //{
        //    while (m_SkidTrail == null)
        //    {
        //        yield return null;
        //    }
        //    m_SkidTrail.parent = transform;
        //    m_SkidTrail.localPosition = -Vector3.up * m_WheelCollider.radius;
        //}

    }
}
