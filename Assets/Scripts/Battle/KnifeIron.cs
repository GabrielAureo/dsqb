using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeIron : MonoBehaviour {
	[SerializeField]
	KnifeMagnetArea magnetArea;
	[SerializeField]
	Rigidbody2D rb;
	[SerializeField]
	SpriteRenderer sr;
	[SerializeField]
	GameObject mainKnife;

	Collider2D knifeCol;

	void Start () {
		Set_Magnet(GameObject.FindObjectOfType<KnifeMagnetArea>());
	}
	
	void Update () {
		Handle_Magnet();
		Handle_Direction();
	}

	void Handle_Direction() {
		if (rb.velocity == Vector2.zero) {
			return;
		} else if (!sr.enabled) {
			sr.enabled = true;
		}

		float rotation = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
		this.transform.rotation = Quaternion.Euler(0f, 0f, rotation - 90);
	}

	public void Set_Magnet(KnifeMagnetArea magnet) {
		mainKnife.transform.SetParent(this.transform.root);
		this.magnetArea = magnet;
		knifeCol = GetComponentInParent<Collider2D>();
		rb.WakeUp();
	}

	void Handle_Magnet() {
		if (magnetArea == null) {
			return;
		}

		knifeCol.enabled = true;

		float magnitude = Mathf.Clamp(rb.velocity.magnitude, 0f, 10f);
		rb.velocity = rb.velocity.normalized * magnitude;

		Vector3 offset = magnetArea.transform.position - transform.position;
		offset.z = 0;
		
		float magsqr = offset.sqrMagnitude;
		float gravityForce = 500f;

		if (magsqr > 0.01f) {
			rb.AddForce(gravityForce * offset.normalized, ForceMode2D.Force);
		}

	}
}
