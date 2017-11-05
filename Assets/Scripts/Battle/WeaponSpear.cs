using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WeaponSpear : WeaponDeluxe {

	[SerializeField]
	float lifetimeAfterDisappearing = 2f;
	[SerializeField]
	SpearCaptureArea spearCapture;

	bool stopped = false;
	float original_drag;
	[SerializeField]
	[Range(0f, 1f)]
	float range = 0.5f;

	void Start () {
		Initialize();

		StartCoroutine (Start_Stopping ());
	}

	void Update() {
		if (!sr.isVisible && !is_destroying) {
			StartCoroutine (Destroy ());
		}

		Handle_Direction();
	}

	void Initialize() {
		rb = GetComponentInChildren<Rigidbody2D>();

		original_drag = rb.drag;
		rb.drag = 0.5f;
	}

	IEnumerator Start_Stopping() {
		yield return new WaitForSeconds (range * 0.4f);

		Stop ();

		yield return new WaitForSeconds (0.75f);

		spearCapture.gameObject.SetActive(true);
		spearCapture.Initialize();
	}

	public void Stop() {
		rb.drag = original_drag;
	}

	public override void Handle_Direction() {
		if (rb.velocity.sqrMagnitude < 0.35f) {
			if (!stopped) {
				stopped = true;
				rb.velocity = Vector2.zero;
				transform.DOScale (this.transform.localScale - Vector3.one * 0.15f, 0.25f);
			}
			return;
		}

		float rotation = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
		this.transform.rotation = Quaternion.Euler(0f, 0f, rotation - 90);
	}

	public override void Register_Hit(GameObject target) {
		if(already_damaged){
			return;
		}
		if (rb.velocity.sqrMagnitude < 0.1f) {
			return;
		}
		already_damaged = true;

		target.GetComponentInChildren<Dragon>().Take_Damage_Spear(this);

		if (destroy_on_contact) {
			Destroy(this.gameObject);
		}
	}

	bool is_destroying = false;
	IEnumerator Destroy() {
		is_destroying = true;

		yield return new WaitForSeconds(lifetimeAfterDisappearing);

		Destroy_Now ();
	}

	public void Destroy_Now() {
		GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren <Player>().current_weapons++;
		Destroy(this.gameObject);
	}

	public void Fix_Spear_To(GameObject target) {
		sr.transform.parent = target.transform;
		sr.transform.SetAsLastSibling();
		sr.sortingOrder = 0;
		sr.gameObject.tag = gameObject.tag;
		sr.gameObject.name = "FIXED_SPEAR";
		

		spearCapture.gameObject.SetActive(true);
		spearCapture.Initialize();
		Destroy(this.gameObject);
	}
}
