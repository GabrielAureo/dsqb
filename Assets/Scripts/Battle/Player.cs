using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon { BOW, SPEAR, KNIFE };

public class Player : MonoBehaviour {

	//references
	SpriteRenderer sr;
	Rigidbody2D rb;
	PlayerSpearCapture spearCapture;

	float charge_time = 1f; //time in seconds to complete a charge cycle
	bool is_charging_shot = false;

	//properties
	public Weapon current_weapon = Weapon.KNIFE;
	public float charge = 0f;
	public float health = 1f;
	public float stamina = 1f;
	public float invincibilityCooldown = 1f;

	[Header("Physics Properties")]
	[SerializeField]
	float speed;

	[Header("Prefab References")]
	[SerializeField]
	GameObject arrowPrefab;
	[SerializeField]
	GameObject spearPrefab;
	[SerializeField]
	GameObject knifePrefab;
	[SerializeField]
	GameObject knifeMagnetPrefab;

	[Header("Weapons")]
	[HideInInspector]
	public bool is_capturing_spear = false;
	[HideInInspector]
	public bool knife_magnet_deployed = false;
	public int max_spears = 3;
	public int max_knifes = 3;
	public int current_weapons;


	#region Start
		void Start() {
			Initialize_References();
			Initialize_Properties();
		}

		void Initialize_References() {
			rb = this.GetComponentInChildren<Rigidbody2D>();
			sr = this.GetComponentInChildren<SpriteRenderer>();
			spearCapture = this.GetComponentInChildren <PlayerSpearCapture> ();
		}

		void Initialize_Properties() {
			switch(current_weapon){
				case Weapon.SPEAR:
					current_weapons = max_spears;
					break;
				case Weapon.KNIFE:
					current_weapons = max_knifes;
					break;
				default:
					current_weapons = max_spears;
					break;	
			}
			
		}
	#endregion

	#region Update and Handlers
		void Update () {
			if (Input.GetKeyDown(KeyCode.Equals)) {
				current_weapon = (Weapon) (((int) (current_weapon) + 1) % System.Enum.GetValues(typeof(Weapon)).Length);
			}

			Handle_Aim();
			Handle_Movement();
			Handle_Stamina();
			Handle_Weapon();
		}

		void Handle_Aim() {
			Vector2 aim_analog = new Vector2(
				Input.GetAxis("Horizontal_Right"),
				Input.GetAxis("Vertical_Right")
			);

			if (aim_analog == Vector2.zero) {
				aim_analog = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				aim_analog -= (Vector2) this.transform.position;
				
				if (aim_analog == Vector2.zero) {
					return;
				}
			}

			float rotation = Mathf.Atan2(aim_analog.y, aim_analog.x) * Mathf.Rad2Deg;
			this.transform.rotation = Quaternion.Euler(0f, 0f, rotation - 90);
		}

		void Handle_Movement() {
			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");

			float speed = this.speed;

			if (is_charging_shot) {
				if (current_weapon == Weapon.BOW) {
					speed /= 4f; //if player is charging shot, he gets slower
				}
				// else if (current_weapon == Weapon.SPEAR) {
				// 	speed /= 6f;
				// }
			}

			rb.velocity = new Vector2(horizontal, vertical) * speed;
		}

		void Handle_Weapon() {
			switch (current_weapon) {
				case Weapon.BOW:
					Handle_Weapon_Bow();
					break;
				case Weapon.SPEAR:
					Handle_Weapon_Spear();
					// Handle_Weapon_Spear_Capture ();
					break;
				case Weapon.KNIFE:
					Handle_Weapon_Knife();
					break;
			}
		}
	#endregion

	#region Sprite
		void Set_Alpha(float alpha) {
			if (alpha < 0f || alpha > 1f) {
				print("Alpha not received correctly.");
			}
	
			sr.color = HushPuppy.getColorWithOpacity(sr.color, alpha);			
		}
	#endregion

	#region Weapons
		#region Charge
			void Add_Charge() {
				charge += Time.deltaTime / charge_time;
				charge = Mathf.Clamp(charge, 0f, 1f);
			}

			void Reset_Charge() {
				charge = 0f;
			}
		#endregion

		#region Bow
			void Handle_Weapon_Bow() {
				if (Input.GetButton("Fire1")) {
					Start_Bow();
				}

				if (Input.GetButtonUp("Fire1")) {
					Release_Bow(charge);
				}
			}

			void Start_Bow() {
				is_charging_shot = true;
				Stop_Stamina_Recovery();
				Add_Charge();
			}

			void Release_Bow(float charge) {
				is_charging_shot = false;
				Reset_Charge();
				Start_Stamina_Recovery();

				float minimumChargeForBow = 0.33f;
				if (charge < minimumChargeForBow) {
					return;	
				}
			
				GameObject arrow = Instantiate(
					arrowPrefab,
					this.transform.position,
					Quaternion.identity
				);

				float arrowSpeed = 25 * charge;

				Vector2 direction = Vector2.up;
				direction.x = Mathf.Cos((this.transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad);
				direction.y = Mathf.Sin((this.transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad);

				arrow.GetComponentInChildren<Rigidbody2D>().AddForce(
					direction.normalized * arrowSpeed,
					ForceMode2D.Impulse
				);
			}
		#endregion
		#region Spear
			void Handle_Weapon_Spear() {
				if (Input.GetButton("Fire1")) {
					Start_Spear();
				}

				if (Input.GetButtonUp("Fire1")) {
					Release_Spear(charge);
				}
			}

			void Start_Spear() {
				if (current_weapons <= 0) {
					return;
				}

				is_charging_shot = true;
				Stop_Stamina_Recovery();
				Add_Charge();
			}

			void Release_Spear(float charge) {
				if (current_weapons <= 0) {
					return;
				}

				is_charging_shot = false;
				Reset_Charge();
				Start_Stamina_Recovery();

				float minimumChargeForBow = 0.33f;
				if (charge < minimumChargeForBow) {
					return;
				}

				current_weapons--;
				GameObject spear = Instantiate(
					spearPrefab,
					this.transform.position,
					Quaternion.identity
				);

				float spearSpeed = 35 * charge;

				Vector2 direction = Vector2.up;
				direction.x = Mathf.Cos((this.transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad);
				direction.y = Mathf.Sin((this.transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad);
		
				var rot_aux = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
				spear.transform.rotation = Quaternion.Euler(0f, 0f, rot_aux - 90);

				spear.GetComponentInChildren<Rigidbody2D>().AddForce(
					direction.normalized * spearSpeed,
					ForceMode2D.Impulse
				);
			}

			public void Gain_Spear() {
				current_weapons = Mathf.Clamp(current_weapons + 1, 0, max_spears);
			}
		#endregion
		#region Knife
			void Handle_Weapon_Knife() {
				if (Input.GetButton("Fire1")) {
					Start_Knife();
				}

				if (Input.GetButtonUp("Fire1")) {
					Release_Knife(charge);
				}
				
				if (Input.GetButtonDown("Fire2")) {
					Put_Knife_Magnet();
				}
			}

			void Start_Knife() {
				if (current_weapons <= 0) {
					return;
				}

				is_charging_shot = true;
				Stop_Stamina_Recovery();
				Add_Charge();
			}

			void Release_Knife(float charge) {
				if (current_weapons <= 0) {
					return;
				}

				is_charging_shot = false;
				Reset_Charge();
				Start_Stamina_Recovery();

				float minimumChargeForBow = 0.33f;
				if (charge < minimumChargeForBow) {
					return;
				}

				current_weapons--;
				GameObject knife = Instantiate(
					knifePrefab,
					this.transform.position,
					Quaternion.identity
				);

				float knifeSpeed = 35 * charge;

				Vector2 direction = Vector2.up;
				direction.x = Mathf.Cos((this.transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad);
				direction.y = Mathf.Sin((this.transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad);
		
				var rot_aux = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
				knife.transform.rotation = Quaternion.Euler(0f, 0f, rot_aux - 90);

				knife.GetComponentInChildren<Rigidbody2D>().AddForce(
					direction.normalized * knifeSpeed,
					ForceMode2D.Impulse
				);
			}

			void Put_Knife_Magnet() {
				if (knife_magnet_deployed) {
					return;
				}

				knife_magnet_deployed = true;

				GameObject go = Instantiate(
					knifeMagnetPrefab,
					this.transform.position,
					Quaternion.identity
				);
				
				foreach (GameObject knife in GameObject.FindGameObjectsWithTag("Knife")) {
					knife.GetComponentInChildren<KnifeIron>().Set_Magnet(
						go.GetComponentInChildren<KnifeMagnetArea>()
					);
				}				
			}

			public void Collect_Magnet() {
				StartCoroutine(Collect_Magnet_Cooldown());
			}

			IEnumerator Collect_Magnet_Cooldown() {
				yield return HushPuppy.WaitForEndOfFrames(30);
				knife_magnet_deployed = false;
			}
		#endregion
	#endregion

	#region Stamina
	//stamina variables
		float bow_stamina_depletion = 2f;
		float spear_stamina_depletion = 1.5f;
		float stamina_recovery_time = 4f;
		Coroutine stamina_recovery = null;
		float capture_spear_depletion = 4f;

		void Handle_Stamina() {
			if (is_charging_shot) {
				if (current_weapon == Weapon.BOW) {
					stamina -= Time.deltaTime / bow_stamina_depletion; 
				}
				if (current_weapon == Weapon.SPEAR) {
					stamina -= Time.deltaTime / spear_stamina_depletion;
				}
			}

			stamina = Mathf.Clamp(stamina, 0f, 1f);

			if (stamina == 0f) {
				if (is_charging_shot) {
					if (current_weapon == Weapon.BOW) {
						Release_Bow (charge);
					}
					if (current_weapon == Weapon.SPEAR) {
						Release_Spear (charge);
					}
				}
			}
			
		}

		void Start_Stamina_Recovery() {
			Stop_Stamina_Recovery();
			stamina_recovery = StartCoroutine(Recover_Stamina());
		}

		void Stop_Stamina_Recovery() {
			if (stamina_recovery != null) {
				StopCoroutine(stamina_recovery);
				stamina_recovery = null;
			}
		}

		IEnumerator Recover_Stamina() {
			yield return new WaitForSeconds(0.5f);

			while (stamina < 1f) {
				stamina += Time.deltaTime / stamina_recovery_time;
				yield return new WaitForEndOfFrame();
			}

			stamina_recovery = null;
		}
	#endregion

	#region Health
		bool took_hit_invincible = false;

		public void Take_Damage(int amount) {
			if (took_hit_invincible) {
				return;
			}
			
			health -= (float) amount / 100f;
			if (health < 0) {
				((SceneLoader) HushPuppy.safeFindComponent("GameController", "SceneLoader")).Game_Over();
				Destroy(this.gameObject);
			}

			StartCoroutine(Take_Damage_Cooldown());
		}

		IEnumerator Take_Damage_Cooldown() {
			took_hit_invincible = true;
			Set_Alpha(0.5f);

			yield return new WaitForSeconds(invincibilityCooldown);	

			took_hit_invincible = false;
			Set_Alpha(1f);
		}
	#endregion
}
