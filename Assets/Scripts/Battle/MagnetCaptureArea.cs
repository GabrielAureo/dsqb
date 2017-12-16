using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class MagnetCaptureArea : MonoBehaviour {

	[SerializeField]
	GameObject magnetCapture_prefab;
	[SerializeField]
	KnifeMagnetArea magnet;

	GameObject worldCanvas;
	Image magnetCapture;
	bool player_on_capture = false;
	float capture_modifier = 0.5f;
	Player player;

	public void Initialize() {
		worldCanvas = HushPuppy.safeFind("WorldCanvas");
		player = HushPuppy.safeFindComponent("Player", "Player") as Player;

		GameObject aux = Instantiate(magnetCapture_prefab, worldCanvas.transform, false);
		aux.transform.position = this.transform.position;
		magnetCapture = aux.GetComponentInChildren<Image>();
	}

	void  Start(){
		Initialize();
	}
	void Update() {
		Handle_Fill();
	}

	void Handle_Fill() {
		if (magnetCapture == null) {
			return;
		}

		magnetCapture.transform.position = this.transform.position;

		if (player_on_capture && Input.GetButton("Fire2")) {
			Update_Fill(1f);
			player.is_capturing_magnet = true;
		}
		else {
			Update_Fill(-0.5f);
			player.is_capturing_magnet = false;
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
		if (magnetCapture == null) {
			return;
		}
		
		magnetCapture.fillAmount += modifier * Time.deltaTime * capture_modifier;
		magnetCapture.fillAmount = Mathf.Clamp(magnetCapture.fillAmount, 0f, 1f);

		if (magnetCapture.fillAmount == 1f) {
			Collect_Magnet();
		}
	}

	bool collected = false;

	void Collect_Magnet() {
		if (collected) {
			return;
		}

		collected = true;
		player.is_capturing_magnet = false;
		
		StartCoroutine(Destroy_Capture());
		//player.Gain_Spear();
	}

	IEnumerator Destroy_Capture() {
		float wait_time = 0.2f;

		magnetCapture.transform.DOScale(
			magnetCapture.transform.localScale * 1.2f,
			wait_time
		);

		magnetCapture.DOColor(
			HushPuppy.getColorWithOpacity(magnetCapture.color, 0f),
			wait_time
		);

		yield return new WaitForSeconds(wait_time);

		Destroy(magnetCapture.gameObject);
		player.is_capturing_magnet = false;
		if (magnet != null) {
			magnet.Collect_Magnet();
		}
		/*else {
			//is fixed in dragon
			var dragon = GameObject.FindGameObjectWithTag("Dragon");
			Vector3 blood_pos = (this.transform.position + dragon.transform.position) /2;
			var blood = Instantiate(blood_prefab, blood_pos, Quaternion.identity);
			blood.GetComponentInChildren<Blood>().Initialize(3f);
			Destroy(this.transform.parent.gameObject);
		}*/
	}
}