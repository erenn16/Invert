using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileMovement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public CharacterMovement character;
	public string buttonFunction;

	public void OnPointerDown(PointerEventData eventData) {

		if (buttonFunction == "quit") {
			Application.Quit ();
		}

		if (buttonFunction == "moveLeft") {
			character.isMovingLeft (true);
			character.isMovingRight (false);
		} else if (buttonFunction == "moveRight") {
			character.isMovingLeft (false);
			character.isMovingRight (true);
		}

		if (buttonFunction == "jump") {
			character.triggerJump ();
		} else if (buttonFunction == "shift") {
			character.triggerShift ();
		}
	}

	public void OnPointerUp(PointerEventData eventData) {
		if (buttonFunction == "moveLeft") {
			character.isMovingLeft (false);
		} else if (buttonFunction == "moveRight") {
			character.isMovingRight (false);
		}
	}

}
