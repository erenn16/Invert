﻿using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {
	public Camera camera;

	private float speed = 5.0f;
	private Rigidbody2D rb;
	private bool isOnGround = false;

	private Vector3 targetCameraRotation = new Vector3(0,0,0);
	private Vector3 currentCameraRotation;
	private Vector3 targetRotation = new Vector3(0,0,0);
	private Vector3 currentRotation;
	private float gravityY;

	public float rotationStep = 10f;

	public bool cameraUpsideDown = false;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space) && isOnGround) {
			rb.AddForce (new Vector2 (0, 400));
			isOnGround = false;
		} else if ((Input.GetKeyDown (KeyCode.LeftShift) ||
					Input.GetKeyDown (KeyCode.RightShift)) && isOnGround) {
			gameObject.layer = 0;
			gravityY = Physics2D.gravity.y;
			Physics2D.gravity = new Vector2 (0, 0);
			this.rotateCamera ();
		}

		transform.Translate (Vector2.right * Input.GetAxis ("Horizontal") * Time.deltaTime * speed);
	}
	
	void OnCollisionStay2D(Collision2D other) {
		if(other.gameObject.tag == "Ground") {
			isOnGround = true;
		}
	}

	void rotateCamera() {
		currentCameraRotation = camera.transform.eulerAngles;
		currentRotation = gameObject.transform.eulerAngles;
		if (cameraUpsideDown) {
			// 10.0f instead of 0.0f because of inaccuricies
			targetCameraRotation.z = 10.0f;
			targetRotation.x = 10.0f;
		} else {
			targetCameraRotation.z = 180f;
			targetRotation.x = 180f;
		}
		StartCoroutine (shiftRotationAnimation ());
	}

	IEnumerator shiftRotationAnimation() {
		// add rotation step to current rotation.
		currentCameraRotation.z += cameraUpsideDown ? (-1.0f * rotationStep) : rotationStep;
		currentRotation.x += cameraUpsideDown ? (-1.0f * rotationStep) : rotationStep;
		currentRotation.y += cameraUpsideDown ? (-1.0f * rotationStep) : rotationStep;
		Debug.Log ("Current rotation is: " + currentCameraRotation.z);
		camera.transform.eulerAngles = currentCameraRotation;
		gameObject.transform.eulerAngles = currentRotation;
		yield return new WaitForSeconds (0);
		if ((!cameraUpsideDown && currentCameraRotation.z < targetCameraRotation.z) ||
			(cameraUpsideDown && currentCameraRotation.z > targetCameraRotation.z)) {
			StartCoroutine (shiftRotationAnimation ());
		} else {
			cameraUpsideDown = !cameraUpsideDown;
			gameObject.layer = cameraUpsideDown ? 8 : 9;	
			Physics2D.gravity = new Vector2 (0, -1.0f * gravityY);
		}
	}
}