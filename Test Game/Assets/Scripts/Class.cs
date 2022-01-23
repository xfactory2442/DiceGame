using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class : MonoBehaviour
{
   public enum MoveType {
		Attack,
		Defence,
		Speed
	}

	[System.Serializable] 
	public struct Move {
		public string name;
		public List<MoveType> moveTypes;
		public List<int> moveAmount;
		public string description;
	}

	public List<Move> moves;
}
