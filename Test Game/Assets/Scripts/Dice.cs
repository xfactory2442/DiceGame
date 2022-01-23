using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
	Rigidbody rBody = null;

	private void Awake() {
		rBody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate() {
		if (rBody.IsSleeping()) {

			float[] angle = { 0f, 0f, 0f, 0f, 0f, 0f };
			angle[0] = Vector3.Angle(-transform.forward, Vector3.up); // 1
			angle[1] = Vector3.Angle(transform.up, Vector3.up); // 2
			angle[2] = Vector3.Angle(-transform.right, Vector3.up); // 3
			angle[3] = Vector3.Angle(transform.right, Vector3.up); // 4
			angle[4] = Vector3.Angle(-transform.up, Vector3.up); // 5
			angle[5] = Vector3.Angle(transform.forward, Vector3.up); // 6

			int bestAngle = 0;
			for (int i = 1; i < 6; i++) {
				if (angle[bestAngle] > angle[i]) {
					bestAngle = i;
				} 
			}

			SendMessageUpwards("AllDiceFinished", bestAngle + 1);
			GetComponent<Dice>().enabled = false;
		}
	}
}
