using UnityEngine;
using System.Collections;

public class CharMove : MonoBehaviour {

	bool isLeft = false ;
	bool isRight = false;
	bool isFront = false;
	bool isBack = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			GetComponent<Animator> ().SetBool ("isRunning", true);
		}
		else if(Input.GetKeyUp (KeyCode.LeftShift))
		{
			GetComponent<Animator> ().SetBool ("isRunning", false);
		}

		if (Input.GetKeyDown (KeyCode.W)) {
			GetComponent<Animator> ().SetFloat ("speed", 1);
			GetComponent<Animator> ().SetBool ("isMoving", true);
			isFront = true;
		}

		else if (Input.GetKeyUp (KeyCode.W)) {
			isFront = false;
		}

		if (Input.GetKeyDown (KeyCode.A)) {
			GetComponent<Animator> ().SetBool ("isMoving", true);
			isLeft = true;
		}
		
		else if (Input.GetKeyUp (KeyCode.A)) {
			isLeft = false;
		}

		if (Input.GetKeyDown (KeyCode.S)) {
			GetComponent<Animator> ().SetFloat ("speed", -1);
			GetComponent<Animator> ().SetBool ("isMoving", true);
			isBack = true;
		}
		
		else if (Input.GetKeyUp (KeyCode.S)) {
			isBack = false;
		}

		if (Input.GetKeyDown (KeyCode.D)) {
			GetComponent<Animator> ().SetBool ("isMoving", true);
			isRight = true;
		}
		
		else if (Input.GetKeyUp (KeyCode.D)) {
			isRight = false;
		}

		if(!isRight && !isLeft && !isBack && !isFront)
			GetComponent<Animator> ().SetBool ("isMoving", false);

	
		if (Input.GetKey (KeyCode.W)) 
		{

			transform.Translate(transform.forward * Time.deltaTime * 2, Space.World);
			
		}

		if (Input.GetKey (KeyCode.A)) 
		{
			transform.Translate(transform.right * Time.deltaTime * -2, Space.World);
			
		}
		if (Input.GetKey (KeyCode.S)) 
		{
			transform.Translate(transform.forward * Time.deltaTime * -2, Space.World);
			
		}
		if (Input.GetKey (KeyCode.D)) 
		{
			transform.Translate(transform.right * Time.deltaTime * 2, Space.World);
			
		}
	}
}
