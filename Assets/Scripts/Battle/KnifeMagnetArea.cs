using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KnifeMagnetArea : MonoBehaviour {
	[SerializeField]
	GameObject main;
	[SerializeField]
	GameObject UI_prefab;
	[SerializeField]
	TextMeshProUGUI UI_text;

	int current_knifes;

	GameObject worldCanvas;
	MagnetCaptureArea magnetCapture;

	void Start() {
		// foreach (KnifeIron iron in GameObject.FindObjectsOfType<KnifeIron>()) {
		// 	iron.Set_Magnet(this);
		// }

		UI_text = Instantiate(UI_prefab, HushPuppy.safeFind("Canvas").transform, false).
			GetComponentInChildren<TextMeshProUGUI>();
		UI_text.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
		
	}

	void Update() {
		if (UI_text != null) {
			UI_text.text = "x" + current_knifes;
		}
	}

	/*void OnTriggerStay2D(Collider2D collision) {
		GameObject target = collision.gameObject;
		KnifeIron knife_iron = target.GetComponentInChildren<KnifeIron>();
		

		if (target.tag == "Player" && Input.GetButtonDown("Fire2")) {
			Collect_Magnet();
		}
		
	}*/

	void OnTriggerEnter2D(Collider2D collision)
	{

		GameObject target = collision.gameObject;
		KnifeIron knife_iron = target.GetComponentInChildren<KnifeIron>();
		
		if (target.tag == "Knife") {
			Debug.Log("sdasd");
			current_knifes++;
			Destroy(target);
		}
		else if (knife_iron != null) {
			current_knifes++;
			Destroy(knife_iron.transform.parent.gameObject);
		}
	}

	public void Collect_Magnet() {
		Player player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>();
		player.Collect_Magnet();
		player.current_weapons += current_knifes;
		Destroy(UI_text.transform.parent.gameObject);
		Destroy(main);
	}
}
