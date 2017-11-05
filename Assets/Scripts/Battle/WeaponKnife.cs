using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKnife : WeaponDeluxe {
	[SerializeField]
	float lifetimeAfterDisappearing = 2f;

	void Update () {
		if (!sr.isVisible && !is_destroying) {
			StartCoroutine (Destroy ());
		}
	}

	public override void Register_Hit(GameObject target) {
		if(already_damaged){
			return;
		}
		target.GetComponentInChildren<Dragon>().Take_Damage_Knife(this);
		already_damaged = true;
		
		if (destroy_on_contact) {
			Destroy_Now();
			return;
		}
	}

	public void Fix_Knife_To(GameObject target) {
		transform.parent = target.transform;
		transform.SetAsLastSibling();
		sr.sortingOrder = 0;
		rb.velocity = Vector2.zero;
		rb.Sleep();

		if (target.tag == "DragonAttackFireBall") {
			StartCoroutine(Destroy());
		}
		else {
			GetComponentInChildren<Collider2D>().enabled = false;
			this.enabled = false;
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
}
