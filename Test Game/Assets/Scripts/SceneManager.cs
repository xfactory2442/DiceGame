using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
	public enum Stage {
		StartTurn,
		ChooseAction,
		TakeActions,
		EndTurn,
		Count
	}

	public static float buttonMoveInterval = 0.02f;
	public Stage stage = Stage.StartTurn;

	TurnManager turnManager = null;
	CharacterManager characterManager = null;

	private void Start() {
		turnManager = FindObjectOfType<TurnManager>();
		characterManager = FindObjectOfType<CharacterManager>();
		turnManager.StartBattle();
	}

	void HandleBroadcast(List<object> sentObjects) {
		string functionName = (string)sentObjects[0];
		sentObjects.RemoveAt(0);
		BroadcastMessage(functionName, sentObjects);
	}

	//Do stages actions.
	public void DoStage() {
		switch (stage) {
		case Stage.StartTurn:
			turnManager.StartTurn();
			break;
		case Stage.ChooseAction:
			characterManager.InvokeRepeating("LowerActionButtons", 0.0f, buttonMoveInterval);
			break;
		case Stage.TakeActions:
			//TODO: DO ACTION SHIT.

			stage = Stage.ChooseAction;
			DoStage();
			break;
		case Stage.EndTurn:
			stage = Stage.StartTurn;
			turnManager.EndTurn();
			break;
		}
	}

	public void ExitGame() {
		Application.Quit();
	}
}
