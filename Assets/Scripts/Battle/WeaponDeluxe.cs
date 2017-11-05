using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDeluxe : MonoBehaviour {
	public int damage = 30;
	public bool destroy_on_contact = false;

	[SerializeField]
	protected SpriteRenderer sr;
	[SerializeField]
	protected Rigidbody2D rb;

	public bool already_damaged = false;

	void OnTriggerEnter2D(Collider2D coll) {
		GameObject target = coll.gameObject;
		if (target.tag == "Dragon") {
			Register_Hit(target);
		}
	}

	public virtual void Register_Hit(GameObject target) {
		if(already_damaged){
			return;
		}

		target.GetComponentInChildren<Dragon>().Take_Damage(damage);
		already_damaged = true;

		if (destroy_on_contact) {
			Destroy(this.gameObject);
		}
		
	}

	public virtual void Handle_Direction() {
		if (rb.velocity == Vector2.zero) {
			sr.enabled = false;
			return;
		} else if (!sr.enabled) {
			sr.enabled = true;
		}

		float rotation = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
		this.transform.rotation = Quaternion.Euler(0f, 0f, rotation - 90);
	}
}
