using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanFixArrow : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D coll) {
		GameObject target = coll.gameObject;
		//lookingAt(target, this.gameObject);
		if (target.tag == "Arrow") {
			target.GetComponentInChildren<WeaponArrow>().Fix_Arrow_To(this.gameObject);
		}
		if (target.tag == "Knife") {
			target.GetComponentInChildren<WeaponKnife>().Fix_Knife_To(this.gameObject);
		}
		if (target.tag == "Spear") {
			WeaponSpear spear = target.GetComponent<WeaponSpear>();
			Debug.Log(spear);
			spear.Fix_Spear_To(this.gameObject);
		}
	}

	void lookingAt(GameObject looker, GameObject target){
		Debug.Log(Vector2.Angle(looker.transform.up, looker.transform.position - target.transform.position));
	}
}