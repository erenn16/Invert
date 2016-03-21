using UnityEngine;
using System.Collections;

public class MoveBarrier : MonoBehaviour {

	public GameObject barrier;
	private float rotationStep = 5f;

	private Vector3 targetRotation;
	private Vector3 currentRotation;

	public void rotateBarrier() {
		Debug.Log ("started rotating");
		gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		currentRotation = barrier.transform.eulerAngles;
		targetRotation = barrier.transform.eulerAngles;
		targetRotation.z += 180f;
		barrier.layer = 0;
		StartCoroutine (barrierRotationAnimation ());
	}
		
	IEnumerator barrierRotationAnimation() {
		// add rotation step to current rotation.
		currentRotation.z += rotationStep;

		barrier.transform.eulerAngles = currentRotation;
		yield return new WaitForSeconds (0);

		// If we aren't done rotating, start the coroutine again
		// Otherwise, flip the respective variables to show the new state the game is in
		if (currentRotation.z < targetRotation.z) {
			StartCoroutine (barrierRotationAnimation ());
		} else {
			barrier.layer = 11;
			Destroy (gameObject);
		}
	}
}
