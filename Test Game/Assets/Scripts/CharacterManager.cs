using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CharacterManager : MonoBehaviour
{
	enum ThrowType {
		Speed
	}

	[SerializeField]
	float actionButtonRotation = 70.0f;
	//A reference to the throw dice script.
	[SerializeField]
	ThrowDice throwDice = null;
	//An array of the transforms for the action buttons.
	[SerializeField]
	RectTransform[] buttonTransform;
	//An array for the move up and down actions buttons.
	[SerializeField]
	GameObject[] moveButtonsButton; //0 = up, 1 = down;
	//A reference to the end turn button.
	[SerializeField]
	GameObject endTurnButton;

	//Stores which button is going to be moved to the top if the action buttons get moved down.
	int buttonNum;
	//A reference to who's turn it is.
	CharacterStats currentChar;
	//How many actions have been used in the characters turn.
	public int actionsUsed = 0;

	//An array of references to the character stats script.
	[System.NonSerialized]
	public CharacterStats[] character;
	TurnManager turnManager = null;

	private void Start() {
		turnManager = FindObjectOfType<TurnManager>();
		//Find the scripts and assign them to the array.
		character = new CharacterStats[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {
			character[i] = transform.GetChild(i).GetComponent<CharacterStats>();
		}
	}

	//Initialise the battle.
	public void StartBattle() {
		RerollSpeed();
	}

	public void RerollSpeed() {
		for (int i = 0; i < character.Length; i++) {
			ThrowDiceSpeed(i);
		}

		throwDice.CheckQueue();
	}

	public void StartTurn(int turn) {
		currentChar = character[turn];

		List<Class.Move> moves = currentChar.GetMoves();
		for (int i = 0; i < Mathf.Min(buttonTransform.Length, moves.Count); i++) {
			SetActionButton(i, moves[i]);
		}
	}

	int fullyMoved = 0;

	//Raise the Action Buttons
	void RaiseActionButtons() {
		fullyMoved = 0;
		endTurnButton.SetActive(false);
		for (int i = 0; i < moveButtonsButton.Length; i++) {
			moveButtonsButton[i].SetActive(false);
		}

		for (int i = 0; i < buttonTransform.Length; i++) {
			if (i == 0 || buttonTransform[i - 1].eulerAngles.z > 30) {
				Vector3 rotation = buttonTransform[i].eulerAngles;

				float rot = rotation.z + actionButtonRotation * Time.deltaTime;
				rotation.z = Mathf.Clamp(rot, 0, 90);

				buttonTransform[i].eulerAngles = rotation;

				if (rotation.z >= 90.0f) {
					fullyMoved++;
				}
			}
		}

		if (fullyMoved == buttonTransform.Length) {
			CancelInvoke("RaiseActionButtons");
			fullyMoved = 0;
			SendMessageUpwards("DoStage");
		}
	}

	//Lower the Action Buttons
	void LowerActionButtons() {
		List<Class.Move> moves = currentChar.GetMoves();
		//If the number of moves is smaller than four use the number of moves.
		int min = Mathf.Min(buttonTransform.Length - 1, moves.Count - 1);
		//For the minumum number of buttons sub the ones already completed, 
		for (int i = min - fullyMoved; i > -1; i--) {
			if (i == min || buttonTransform[i + 1].eulerAngles.z < 60) {
				Vector3 rotation = buttonTransform[i].eulerAngles;

				float rot = rotation.z - actionButtonRotation * Time.deltaTime;
				rotation.z = Mathf.Clamp(rot, 0, 90);

				buttonTransform[i].eulerAngles = rotation;

				if (rotation.z <= 0.0f) {
					fullyMoved++;
				}
			}
		}

		if (fullyMoved == min + 1) {
			CancelInvoke("LowerActionButtons");
			fullyMoved = 0;

			endTurnButton.SetActive(true);
			for (int i = 0; i < moveButtonsButton.Length; i++) {
				moveButtonsButton[i].SetActive(true);
			}
			buttonNum = 3;
		}
	}

	//Move the action buttons up or down and change their contents accordingly.
	public void MoveButtons(int index) {
		int moved = 0;
		List<Class.Move> moves = currentChar.GetMoves();
		//If there isnt more than 4 moves then dont bother moving them around.
		if (moves.Count < 4) {
			return;
		}
		for (int i = 0; i < buttonTransform.Length; i++) {
			Vector2 position = buttonTransform[i].localPosition;
			position.y += 42.0f * index;
			if (Mathf.Abs(position.y) > 64.0f) {
				position.y = Mathf.Clamp(position.y, -63.0f, 63.0f);
				position.y *= -1;
				moved = i;
			}

			buttonTransform[i].localPosition = position;
		}

		buttonNum += index;

		buttonNum = buttonNum >= moves.Count ? 0 : buttonNum;
		buttonNum = buttonNum < 0 ? moves.Count - 1 : buttonNum;
		SetActionButton(buttonNum, moves[moved]);
	}

	//Set the text and move of an action button.
	void SetActionButton(int buttonIndex, Class.Move move) {
		buttonTransform[buttonIndex].GetComponentInChildren<TMP_Text>().text =
			move.name;
		buttonTransform[buttonIndex].GetComponent<ActionButton>().move = move;
	}

	public void ActionButtonClicked(int i) {
		print(currentChar.GetMoves()[i].name);
		actionsUsed++;
		GetComponentInParent<SceneManager>().stage = SceneManager.Stage.TakeActions;

		InvokeRepeating("RaiseActionButtons", 0.0f, SceneManager.buttonMoveInterval);
	}

	//Throw the speed dice for a character.
	public void ThrowDiceSpeed(int index) {
		List<object> param = new List<object>();
		param.Add("CharacterDice");
		param.Add(ThrowType.Speed);
		param.Add(index);
		ThrowDice(character[index].speedDice, character[index].throwDicePosition, param);
	}

	//Throw a set of dice.
	void ThrowDice(int numDice, Vector3 dicePosition, List<object> param) {
		throwDice.QueueDiceThrow(numDice,
				dicePosition, param);
	}

	//Recieve the results of a dice role and then do something with them.
	void CharacterDice(List<object> param) {
		int index = (int)param[1];
		switch((ThrowType)param[0]) {
		case ThrowType.Speed:
			print(character[index].name + " Speed: " + param[2]);
			character[index].speed = (int)param[2];

			turnManager.ChangeTurnOrder(index);
			break;
		}
	}
}
