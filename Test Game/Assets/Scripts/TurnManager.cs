using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TurnManager : MonoBehaviour
{
	//The turn order of the characters.
	List<int> turns = new List<int>();

	//A reference to the scene manager.
	SceneManager sceneManager = null;
	//A reference to the character manager.
	CharacterManager characterManager = null;
	//A prefab for the turn order visual.
	[SerializeField]
	GameObject character_prefab_ = null;

	[SerializeField]
	Vector2 character_info_initial_Y_anchors_ = new Vector2(0.0f, 0.0f);

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

		Destroy(characterManager.character[turns[0]].characterInfo);
		for (int i = 1; i < turns.Count; i++) {
			RectTransform rect_transform = characterManager.character[turns[i]].characterInfo.GetComponent<RectTransform>();
			rect_transform.anchorMin = 
				new Vector2(rect_transform.anchorMin.x, 
				character_info_initial_Y_anchors_.x + 0.2f * (i - 1));
			rect_transform.anchorMax =
				new Vector2(rect_transform.anchorMax.x,
				character_info_initial_Y_anchors_.y + 0.2f * (i - 1));
		}
	}

	//Change the turn order based on the speed of the characters.
	public void ChangeTurnOrder(int index) {
		GameObject character_info = Instantiate(character_prefab_, gameObject.transform);
		character_info.GetComponentInChildren<TMP_Text>().text =
			characterManager.character[index].characterName;
		characterManager.character[index].characterInfo = character_info;
		int num = -1;
		for (int i = 0; i < turns.Count; i++) {
			if (characterManager.character[turns[i]].speed < 
				characterManager.character[index].speed) {
				turns.Insert(i, index);
				num = i;
				break;
			}
		}
		if (num == -1) {
			turns.Add(index);
			num = 0;
		}
		for (int i = num; i < turns.Count; i++) {
			RectTransform rect_transform = characterManager.character[turns[i]].characterInfo.GetComponent<RectTransform>();
			rect_transform.anchorMin =
				new Vector2(rect_transform.anchorMin.x,
				character_info_initial_Y_anchors_.x + 0.2f * i);
			rect_transform.anchorMax =
				new Vector2(rect_transform.anchorMax.x,
				character_info_initial_Y_anchors_.y + 0.2f * i);
		}
	}
}
