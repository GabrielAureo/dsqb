using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class MagnetCaptureArea : MonoBehaviour {

	[SerializeField]
	GameObject spearCapture_prefab;
	[SerializeField]
	WeaponSpear spear;
	[SerializeField]
	GameObject blood_prefab;

	GameObject worldCanvas;
	Image spearCapture;
	bool player_on_capture = false;
	float capture_modifier = 0.5f;
	Player player;

	public void Initialize() {
		worldCanvas = HushPuppy.safeFind("WorldCanvas");
		player = HushPuppy.safeFindComponent("Player", "Player") as Player;

		GameObject aux = Instantiate(spearCapture_prefab, worldCanvas.transform, false);
		aux.transform.position = this.transform.position;
		spearCapture = aux.GetComponentInChildren<Image>();
	}

	void Update() {
		Handle_Fill();
	}

	void Handle_Fill() {
		if (spearCapture == null) {
			return;
		}

		spearCapture.transform.position = this.transform.position;

		if (player_on_capture && Input.GetButton("Fire2")) {
			Update_Fill(1f);
			player.is_capturing_spear = true;
		}
		else {
			Update_Fill(-0.5f);
			player.is_capturing_spear = false;
		}		
	}

	void OnTriggerEnter2D(Collider2D coll) {
		GameObject target = coll.gameObject;
		if (target.tag == "Player") {
			player_on_capture = true;
		}
	}

	void OnTriggerExit2D(Collider2D coll) {
		GameObject target = coll.gameObject;
		if (target.tag == "Player") {
			player_on_capture = false;
		}
	}

	void Update_Fill(float modifier) {
		if (spearCapture == null) {
			return;
		}
		
		spearCapture.fillAmount += modifier * Time.deltaTime * capture_modifier;
		spearCapture.fillAmount = Mathf.Clamp(spearCapture.fillAmount, 0f, 1f);

		if (spearCapture.fillAmount == 1f) {
			Collect_Spear();
		}
	}

	bool collected = false;

	void Collect_Spear() {
		if (collected) {
			return;
		}

		collected = true;
		player.is_capturing_spear = false;
		
		StartCoroutine(Destroy_Capture());
		player.Gain_Spear();
	}

	IEnumerator Destroy_Capture() {
		float wait_time = 0.2f;

		spearCapture.transform.DOScale(
			spearCapture.transform.localScale * 1.2f,
			wait_time
		);

		spearCapture.DOColor(
			HushPuppy.getColorWithOpacity(spearCapture.color, 0f),
			wait_time
		);

		yield return new WaitForSeconds(wait_time);

		Destroy(spearCapture.gameObject);
		player.is_capturing_spear = false;
		if (spear != null) {
			Destroy(spear.gameObject);
		}
		else {
			//is fixed in dragon
			var dragon = GameObject.FindGameObjectWithTag("Dragon");
			Vector3 blood_pos = (this.transform.position + dragon.transform.position) /2;
			var blood = Instantiate(blood_prefab, blood_pos, Quaternion.identity);
			blood.GetComponentInChildren<Blood>().Initialize(3f);
			Destroy(this.transform.parent.gameObject);
		}
	}
}