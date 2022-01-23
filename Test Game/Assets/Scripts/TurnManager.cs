using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
	//The turn order of the characters.
	List<int> turns = new List<int>();

	//A reference to the scene manager.
	SceneManager sceneManager = null;
	CharacterManager characterManager = null;

	private void Start() {
		sceneManager = GetComponentInParent<SceneManager>();
		characterManager = FindObjectOfType<CharacterManager>();
	}

	public void StartBattle() {
		characterManager.StartBattle();
	}

	public void StartTurn() {
		characterManager.StartTurn(turns[0]);

		sceneManager.stage = SceneManager.Stage.TakeActions;
		sceneManager.DoStage();
	}

	public void EndTurn() {
		characterManager.actionsUsed = 0;

		int current_turn = turns[0];
		turns.RemoveAt(0);

		if (turns.Count == 0) {
			characterManager.RerollSpeed();
		}
		else {
			sceneManager.DoStage();
		}
	}

	public void EndTurnButtonClicked() {
		sceneManager.stage = SceneManager.Stage.EndTurn;
		characterManager.InvokeRepeating("RaiseActionButtons", 0.0f, 0.02f);
	}

	//Change the turn order based on the speed of the characters.
	public void ChangeTurnOrder(int index) {
		if (turns.Count == 0) {
			turns.Add(index);
			return;
		}
		for (int i = 0; i < turns.Count; i++) {
			if (characterManager.character[turns[i]].speed < 
				characterManager.character[index].speed) {
				turns.Insert(i, index);
				return;
			}
		}
	}
}
