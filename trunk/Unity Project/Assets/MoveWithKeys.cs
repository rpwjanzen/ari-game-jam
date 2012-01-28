using UnityEngine;
using System.Collections;

public class MoveWithKeys : MonoBehaviour {
	
	float forwardSpeed = 5f;
	float turnSpeed = 10f;
	
	void Start ()
	{
	
	}
	
	void Update ()
	{
		var horizontalAxis = Input.GetAxis("Horizontal");
		var turnAmount = horizontalAxis * Time.deltaTime * turnSpeed;
		transform.Rotate(0f, 0f, turnAmount);

		var verticalAxis = Input.GetAxis("Vertical");
		transform.Translate(transform.forward * (-verticalAxis) * Time.deltaTime * forwardSpeed);
	}
}
