using UnityEngine;
using System.Collections;

public class RotateObjectAlongY : MonoBehaviour {

	public float Degrees = 0.1f;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0, Degrees, 0);
	}
}
