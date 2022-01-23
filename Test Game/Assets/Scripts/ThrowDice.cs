using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDice : MonoBehaviour {
	/*Struct to hold the number of extra dice, the position the dice are to be 
	 * released and the params to be sent to the future scripts.*/
	struct DiceThrow {
		public int extraDice;
		public Vector3 dicePosition;
		public List<object> param;
	}

	//The prefab of the dice.
	[SerializeField]
	GameObject dicePrefab = null;
	//The initial number of dice before a character/enemies dice are added.
	[SerializeField]
	int initNumDice = 1;
	//The maxiumum force that can be applied to the dice.
	[SerializeField, Range(100, 5000)]
	float maxForce = 200;
	//The furthest along the force can be applied to the dice.
	[SerializeField, Range(0, 100)]
	float maxPosition = 4;

	public List<GameObject> dice = new List<GameObject>();
	List<DiceThrow> diceThrowQueue = new List<DiceThrow>();

	//If the dice throw queue isnt empty then throw some dice.
	public bool CheckQueue() {
		if (diceThrowQueue.Count > 0) {
			ThrowTheDice();
			return true;
		}
		return false;
	}

	//Add a dice throw to the queue.
	public void QueueDiceThrow(int extraDice, Vector3 throwDicePosition, List<object> functionNameAndParameters_) {
		int index = diceThrowQueue.Count;
		DiceThrow diceThrow = new DiceThrow();
		diceThrow.extraDice = extraDice;
		diceThrow.dicePosition = throwDicePosition;
		diceThrow.param = functionNameAndParameters_;
		diceThrowQueue.Add(diceThrow);
	}

	public void ThrowTheDice() {
		finishedDice = 0;
		addedDiceNums = 0;

		int numDiceOnBoard = initNumDice;
		numDiceOnBoard += diceThrowQueue[0].extraDice;

		//Destroy any dice that might be on the board.
		for (int i = 0; i < dice.Count; i++) {
			Destroy(dice[i]);
			dice.RemoveAt(0);
			i--;
		}

		for (int i = 0; i < numDiceOnBoard; i++) {
			//Create a dice.
			dice.Add(Instantiate(dicePrefab, transform));
			Rigidbody dPhysics = dice[dice.Count - 1].GetComponent<Rigidbody>();
			dPhysics.transform.position = diceThrowQueue[0].dicePosition;

			//Add a random force to the dice.
			Vector3 force = new Vector3();
			force.x = Random.Range(-maxForce, maxForce);
			force.y = Random.Range(-maxForce / 2f, maxForce / 4f);
			force.z = Random.Range(-maxForce, maxForce);
			
			//Add the force at a random position.
			Vector3 position = new Vector3();
			position.x = Random.Range(-maxPosition, maxPosition);
			position.y = Random.Range(-maxPosition, maxPosition);
			position.z = Random.Range(-maxPosition, maxPosition);

			//Apply the force to the dice.
			dPhysics.AddForceAtPosition(force, position);
		}
	}

	int finishedDice = 0;
	int addedDiceNums = 0;

	void AllDiceFinished(int num) {
		//Increase the int for number of stationary dice.
		finishedDice++;
		//Add the sum on the upwards face of the dice to the total number.
		addedDiceNums += num;
		//If all the dice are stationary.
		if (finishedDice >= dice.Count) {
			//Add the sum of the upwards face of the dice to the params to send over.
			diceThrowQueue[0].param.Add(addedDiceNums);
			//Have the scene manager broadcast down to which ever script asked for the dice number.
			SendMessageUpwards("HandleBroadcast", diceThrowQueue[0].param);
			//Remove the current throw from the throw queue.
			diceThrowQueue.RemoveAt(0);

			if (!CheckQueue()) {
				SendMessageUpwards("DoStage");
			}
		}
	}
}
