using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanFixArrow : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D coll) {
		GameObject target = coll.gameObject;
		if(gameObject.tag != "DragonAttackFireball" && !lookingAt(target, this.gameObject)) return;
		if (target.tag == "Arrow") {
			target.GetComponentInChildren<WeaponArrow>().Fix_Arrow_To(this.gameObject);
		}
		if (target.tag == "Knife") {
			target.GetComponentInChildren<WeaponKnife>().Fix_Knife_To(this.gameObject);
		}
		if (target.tag == "Spear" && gameObject.tag != "DragonAttackFireball") {
			WeaponSpear spear = target.GetComponentInChildren<WeaponSpear>();
			Debug.Log(spear);
			spear.Fix_Spear_To(this.gameObject);
		}
	}
	bool lookingAt(GameObject looker, GameObject target){
		var v = looker.transform.rotation * Vector3.up;
		v = v.normalized;
		return Vector3.Dot(v,target.transform.position.normalized) > 0;

	}
}