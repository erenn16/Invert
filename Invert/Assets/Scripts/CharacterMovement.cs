﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour {
	public Camera camera;

	private float speed = 7.0f;
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
	private bool movingLeft = false;
	private bool movingRight = false;
	private int shiftTriggers = 0;
	private int jumpTriggers = 0;

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
				Application.LoadLevel (Application.loadedLevel);
				Physics2D.gravity = new Vector2 (0, -9.81f);
			}
		}
			
		if (!isDead) {
			if (jumpTriggers > 0) {
				jumpTriggers--;
				rb.AddForce (cameraUpsideDown ? new Vector2 (0, -500) : new Vector2 (0, 500));
				isOnGround = false;
			} else if (shiftTriggers > 0) {
				// flip the character to the default layer so that it won't collide with any of the
				// black or white tiles
				shiftTriggers--;
				gameObject.layer = 0;
				gravityY = Physics2D.gravity.y;
				Physics2D.gravity = new Vector2 (0, 0);
				this.rotateCamera ();
			}

			if (movingRight) {
				gameObject.GetComponent<SpriteRenderer> ().flipX = false;
			} else if (movingLeft) {
				gameObject.GetComponent<SpriteRenderer> ().flipX = true;
			}
			float move = movingLeft ? -1.0f : (movingRight ? 1.0f : 0.0f);
			transform.Translate (Vector2.right * move * Time.deltaTime * speed);
		}
	}

	// trigger moving left from the buttons script
	public void isMovingLeft(bool left) {
		movingLeft = left;
	}

	// trigger moving right from the buttons script
	public void isMovingRight(bool right) {
		movingRight = right;
	}

	// trigger a jump from the buttons script
	public void triggerJump() {
		if (isOnGround) {
			jumpTriggers = 1;
		}
	}

	// trigger a shift from the buttons script
	public void triggerShift() {
		if (isOnGround && canShift) {
			shiftTriggers = 1;
		}
	}
	
	void OnCollisionStay2D(Collision2D other) {
		if(other.gameObject.tag == "Ground") {
			isOnGround = true;
		}

		if ((other.gameObject.layer == 8 || other.gameObject.layer == 9) && other.gameObject.layer != 11) {
			canShift = true;
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		if(other.gameObject.tag == "Deadly") {
			isDead = true;
			gameObject.GetComponent<SpriteRenderer> ().enabled = false;
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

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log ("Triggered: " + other.gameObject.tag);
		if (other.gameObject.tag == "Door" && isOnGround) {
			SceneManager.LoadScene (other.GetComponent<NextRoom> ().getNextRoom ());
			Physics2D.gravity = new Vector2 (0, -9.81f);
		} else if (other.gameObject.tag == "Key") {
			Debug.Log ("hit a key");
			other.GetComponent<MoveBarrier> ().rotateBarrier ();
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
			particles.transform.eulerAngles = gameObject.transform.eulerAngles;
			cameraUpsideDown = !cameraUpsideDown;
			gameObject.layer = cameraUpsideDown ? 8 : 9;
			particles.gravityModifier = -1.0f * particles.gravityModifier;
			Physics2D.gravity = new Vector2 (0, -1.0f * gravityY);
			if (gameObject.layer == 8) {
				gameObject.GetComponent<SpriteRenderer> ().color = Color.white; 
			} else if (gameObject.layer == 9) {
				gameObject.GetComponent<SpriteRenderer> ().color = Color.black; 
			}
		}
	}
}
