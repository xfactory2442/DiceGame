using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxActions = 2;
    public int attackDice = 1;
    public int speedDice = 1;
    [HideInInspector]
    public int speed = 1;
    public Vector3 throwDicePosition = new Vector3(0, 15, 0);
    
    public string characterName = "";
    Class characterClass = null;

    public GameObject characterInfo = null;

    private void Awake() {
        characterName = transform.name;
        characterClass = GetComponent<Class>();
    }

    public List<Class.Move> GetMoves() {
        return characterClass.moves;
	}
}
