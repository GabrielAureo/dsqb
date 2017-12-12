using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelection : MonoBehaviour {
	[SerializeField]
	Player player;

	Weapon weapon;

	[SerializeField]
	Button StartButton;
	[SerializeField]
	GameObject objs;



	void Start () {
		StartButton.interactable = false;
		objs.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SelectWeapon(string w){
		weapon = (Weapon)System.Enum.Parse(typeof(Weapon), w);

		StartButton.interactable = true;
	}

	public void StartGame(){
		player.current_weapon = weapon;
		objs.SetActive(true);
		gameObject.SetActive(false);
	}


}
