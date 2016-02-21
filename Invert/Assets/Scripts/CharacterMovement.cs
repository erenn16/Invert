﻿using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {
	public Camera camera;

	private float speed = 5.0f;
	private Rigidbody2D rb;
	private bool isOnGround = false;
	private bool canShift = false;

	private Vector3 targetCameraRotation = new Vector3(0,0,0);
	private Vector3 currentCameraRotation;
	private Vector3 targetRotation = new Vector3(0,0,0);
	private Vector3 currentRotation;
	private float gravityY;

	private float rotationStep = 10f;

	private bool cameraUpsideDown = false;
	private ParticleSystem particles;
	private bool isDead = false;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		particles = GetComponentInChildren<ParticleSystem> ();
	}

	// Update is called once per frame
	void Update () {
		if (particles) {
			if (!particles.IsAlive () && isDead) {
				particles.Stop ();
				particles.Clear ();
				Destroy (gameObject);
			}
		}
			
		if (!isDead) {
			if (Input.GetKeyDown (KeyCode.Space) && isOnGround) {
				rb.AddForce (cameraUpsideDown ? new Vector2 (0, -400) : new Vector2 (0, 400));
				isOnGround = false;
			} else if ((Input.GetKeyDown (KeyCode.LeftShift) ||
			           Input.GetKeyDown (KeyCode.RightShift)) && isOnGround && canShift) {
				// flip the character to the default layer so that it won't collide with any of the
				// black or white tiles
				gameObject.layer = 0;
				gravityY = Physics2D.gravity.y;
				Physics2D.gravity = new Vector2 (0, 0);
				this.rotateCamera ();
			}

			if (Input.GetAxis ("Horizontal") > 0) {
				gameObject.GetComponent<SpriteRenderer> ().flipX = false;
			} else if (Input.GetAxis("Horizontal") < 0) {
				gameObject.GetComponent<SpriteRenderer> ().flipX = true;
			}
			transform.Translate (Vector2.right * Input.GetAxis ("Horizontal") * Time.deltaTime * speed);
		}
	}
	
	void OnCollisionStay2D(Collision2D other) {
		if(other.gameObject.tag == "Ground") {
			isOnGround = true;
		}

		if (other.gameObject.layer == 8 || other.gameObject.layer == 9) {
			canShift = true;
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		if(other.gameObject.tag == "Deadly") {
			isDead = true;
			gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			particles.enableEmission = true;
			particles.Play ();
		}
	}

	void OnCollisionExit2D(Collision2D other) {
		if(other.gameObject.tag == "Ground") {
			isOnGround = false;
		}

		if (other.gameObject.layer == 8 || other.gameObject.layer == 9) {
			canShift = false;
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

		camera.transform.eulerAngles = currentCameraRotation;
		gameObject.transform.eulerAngles = currentRotation;
		yield return new WaitForSeconds (0);

		// If we aren't done rotating, start the coroutine again
		// Otherwise, flip the respective variables to show the new state the game is in
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
