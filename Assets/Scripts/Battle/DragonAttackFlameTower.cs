using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DragonAttackFlameTower : DragonAttackDeluxe {
	[SerializeField]
	ParticleSystem explosion;
	[SerializeField]
	CircleCollider2D trigger_coll;

	public float base_buildup_time = 2f;
	float buildup_health_multiplier = 1f;

	void Start() {
		StartCoroutine(Behaviour());
	}

	public void Initialize(float health) {
		buildup_health_multiplier = health * 1f;
		//if dragon has 50% HP, flametowers explode 50% quicker.
	}

	IEnumerator Behaviour() {
		trigger_coll.enabled = false;
		Vector3 original_scale = this.transform.localScale;

		this.transform.localScale = Vector3.zero;
		this.transform.DOScale(
			original_scale,
			0.4f
		);

		yield return new WaitForSeconds(0.5f);

		float wait_time = Mathf.Clamp(
			buildup_health_multiplier * base_buildup_time,
			0.5f,
			base_buildup_time);
			
		yield return new WaitForSeconds(wait_time);
		
		explosion.Play();
		trigger_coll.enabled = true;

		yield return new WaitForSeconds(
			explosion.main.duration +
			explosion.main.startLifetime.constant);
		
		trigger_coll.enabled = false;
		
		var tween = this.transform.DOScale(
			Vector3.zero,
			0.5f
		);
		tween.SetEase(Ease.InCirc);
		yield return new WaitForSeconds(0.5f);

		Destroy(this.transform.parent.gameObject);
	}
}
