using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
	private PlayerHealth playerHealth;
	public List<GameObject> enemyHitboxes = new List<GameObject>(); // Initialize the list to prevent null errors
	public GameObject weaponHitbox;
	public GameObject weaponTrail;
	public GameObject shieldHitbox;

	public float speed = 1.0f; // Normal movement speed
	public float sprintSpeed = 5.0f; // Sprint speed
	public float rollSpeed = 5f;
	public float rollDuration = 0.5f;
	private Rigidbody rb;
	private Animator anim;

	private bool dodge = false;
	private float rollTime = 0f;
	private Vector3 rollDirection;
	private bool isAttacking = false;
	private bool isWalking = false; // Flag to track walking state

	private Vector3 rootMotionDelta = Vector3.zero; // To store root motion adjustments

	private StaminaManager staminaManager;
	public float staminaCostSprint = 5.0f;  // Stamina cost per second for sprinting
	public float staminaCostDodge = 20.0f;  // Stamina cost for dodging
	public float staminaCostAttack = 0f; // Stamina cost for attacking
	
	// Healing properties
	public int maxHealUses = 3;
	public int remainingHealUses = 3;
	public int healAmount = 30;

	// Audio
	AudioManager audioManager;

	private void Awake()
	{
		audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
	}

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		playerHealth = GetComponent<PlayerHealth>();
		staminaManager = GetComponent<StaminaManager>();
	
		if (playerHealth == null)
		{
			Debug.LogError("PlayerHealth component not found on this GameObject.");
		}
	
		if (staminaManager == null)
		{
			Debug.LogError("StaminaManager component not found on this GameObject.");
		}
	
		AssignHitboxes();
	
		if (weaponHitbox != null)
		{
			weaponHitbox.SetActive(false); // Disable weapon hitbox by default
		}
		else
		{
			Debug.LogWarning("Weapon hitbox is not assigned. Please set it in the inspector.");
		}
	
		if (weaponTrail != null)
		{
			weaponTrail.SetActive(false); // Ensure the weapon trail is initially inactive
		}
		else
		{
			Debug.LogWarning("Weapon trail is not assigned. Please set it in the inspector.");
		}
	
		if (shieldHitbox != null)
		{
			shieldHitbox.SetActive(false); // Ensure the shield hitbox is initially inactive
		}
		else
		{
			Debug.LogWarning("Shield hitbox is not assigned. Please set it in the inspector.");
		}
	}
	
	void Update()
	{
		// Clean up enemyHitboxes list by removing inactive or null hitboxes
		RemoveInactiveHitboxes();
		if (playerHealth != null && playerHealth.IsDead() && anim != null)
		{
			anim.SetBool("dead", true); // Trigger death animation
			if (SceneManager.GetActiveScene().name == "MainBoss")
			{
				SceneManager.LoadScene("CSC_Ending");
			}
			else
			{
				SceneManager.LoadScene("Main Menu");
			}
		}
	
		// Blocking logic
		bool isBlocking = Input.GetMouseButton(1); // Right mouse button for blocking
		anim.SetBool("block", isBlocking);
	
		if (shieldHitbox != null)
		{
			shieldHitbox.SetActive(isBlocking); // Activate shield hitbox when blocking
		}
	
		if (!isAttacking && !isBlocking) // Only allow movement if not attacking or blocking
		{
			if (!dodge)
			{
				HandleMovement();
				if (Input.GetMouseButtonDown(0) && staminaManager.HasEnoughStamina(staminaCostAttack)) // Attack on left mouse button
				{
					staminaManager.UseStamina(staminaCostAttack);
					StartCoroutine(Attack());
				}
				if (Input.GetKeyDown(KeyCode.Space) && staminaManager.HasEnoughStamina(staminaCostDodge)) // Dodge on spacebar
				{
					staminaManager.UseStamina(staminaCostDodge);
					StartDodge();
				}
			}
			else
			{
				Dodge();
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) // Warcry skill on right mouse button
		{
			StartCoroutine(WarcrySkill());
		}
	
		if (Input.GetKeyDown(KeyCode.R)) // Heal when pressing R
		{
			Heal();
		}
	
		if (Time.frameCount % 60 == 0) // Example: Reassign every 60 frames
		{
			AssignHitboxes();
		}
	}
	private IEnumerator WarcrySkill()
	{
		anim.SetBool("warcry", true); // Trigger warcry animation
	
		if (weaponHitbox != null)
		{
			weaponHitbox.SetActive(true); // Activate weapon hitbox
		}
		else
		{
			Debug.LogWarning("Weapon hitbox is not assigned!");
		}
	
		PlayerWHitbox playerWHitbox = weaponHitbox.GetComponent<PlayerWHitbox>();
		if (playerWHitbox != null)
		{
			StartCoroutine(playerWHitbox.ApplyDamageBuff()); // Apply damage buff
		}
		else
		{
			Debug.LogWarning("PlayerWHitbox component not found on weaponHitbox!");
		}
	
		yield return new WaitForSeconds(1.0f); // Adjust the delay as needed
	
		anim.SetBool("warcry", false); // Reset warcry animation
		if (weaponHitbox != null)
		{
			weaponHitbox.SetActive(false); // Deactivate weapon hitbox
		}
	}
	private void Heal()
	{
		if (remainingHealUses > 0 && playerHealth != null)
		{
			
			if (playerHealth.health >= 100)
			{
				Debug.Log("Player health is already full. Heal not applied.");
				return;
			}
		
				playerHealth.RestoreHealth(healAmount); // Restore 30 health
				remainingHealUses--; // Decrease remaining heal uses
				Debug.Log($"Healed for {healAmount}. Remaining heals: {remainingHealUses}");
		}
		else
		{
			Debug.Log("No heals remaining or PlayerHealth is not assigned.");
		}
	}
	
	public void OnBlockSuccess(Vector3 enemyPosition)
	{
		// Calculate the direction to the enemy
		Vector3 directionToEnemy = (enemyPosition - transform.position).normalized;
		directionToEnemy.y = 0; // Ignore vertical direction

		// Rotate the player to face the enemy
		Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

		audioManager.PlaySFX(audioManager.shieldHit);
	}

	private void RemoveInactiveHitboxes()
	{
		for (int i = enemyHitboxes.Count - 1; i >= 0; i--)
		{
			if (enemyHitboxes[i] == null || !enemyHitboxes[i].activeSelf)
			{
				Debug.Log("Removed inactive or null hitbox: " + (enemyHitboxes[i] != null ? enemyHitboxes[i].name : "null"));
				enemyHitboxes.RemoveAt(i); // Remove inactive or null hitbox from the list
			}
		}
	}

	private void AssignHitboxes()
	{
		// Find all objects with the "Hitbox" tag in the entire scene
		GameObject[] allHitboxes = GameObject.FindGameObjectsWithTag("Hitbox");

		foreach (GameObject hitbox in allHitboxes)
		{
			// Only add hitboxes that are active and not already in the list
			if (hitbox.activeSelf && !enemyHitboxes.Contains(hitbox))
			{
				enemyHitboxes.Add(hitbox);
				Debug.Log("Assigned hitbox: " + hitbox.name);
			}
		}
	}

	private void HandleMovement()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		// Get camera-forward and camera-right vectors
		Vector3 cameraForward = Camera.main.transform.forward;
		Vector3 cameraRight = Camera.main.transform.right;

		// Flatten camera vectors to ignore vertical axis
		cameraForward.y = 0;
		cameraRight.y = 0;
		cameraForward.Normalize();
		cameraRight.Normalize();

		// Create movement vector relative to the camera
		Vector3 movement = (cameraForward * moveVertical + cameraRight * moveHorizontal).normalized;
		Debug.DrawRay(transform.position, movement * 1f, Color.red);

		// Determine current speed (normal or sprint)
		float currentSpeed = Input.GetKey(KeyCode.LeftShift) && staminaManager.HasEnoughStamina(staminaCostSprint * Time.deltaTime) ? sprintSpeed : speed;

		if (currentSpeed == sprintSpeed)
		{
			staminaManager.UseStamina(staminaCostSprint * Time.deltaTime);
		}

		LayerMask wallLayer = LayerMask.GetMask("Wall"); // Ensure "Wall" layer is assigned

		// If there's movement, check for obstacles and move
		if (movement.magnitude > 0)
		{
			if (!isWalking)
			{
				audioManager.PlaySFX(audioManager.walk);
				isWalking = true;
			}

			// Calculate desired movement direction
			Vector3 desiredMovement = movement * currentSpeed * Time.deltaTime;

			// Perform a raycast to check for collisions
			if (!Physics.Raycast(transform.position, movement, out RaycastHit hit, 0.5f))
			{
				// Apply movement using Rigidbody
				rb.transform.Translate(desiredMovement, Space.World);

				// Smoothly rotate the character towards movement direction
				Quaternion targetRotation = Quaternion.LookRotation(movement);
				rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 10f);
			}
			Vector3 forwardDirection = transform.forward;  // Use player’s facing direction
			if (Physics.Raycast(transform.position, forwardDirection, out RaycastHit kenak, 1f, wallLayer))
			{
				Debug.Log("Hit a wall, stopping movement.");
			}
		}
		else
		{
			if (isWalking)
			{
				isWalking = false;
				audioManager.StopSFX();
			}
		}

		// Update animator parameters
		anim.SetFloat("x", moveHorizontal);
		anim.SetFloat("y", moveVertical);
		anim.SetFloat("speed", movement.magnitude * currentSpeed / sprintSpeed); // Normalize speed for sprinting
	}

	private void StartDodge()
	{
		dodge = true;
		rollTime = rollDuration;

		// Deactivate enemy hitboxes when dodging
		foreach (GameObject hitbox in enemyHitboxes)
		{
			if (hitbox != null)
			{
				hitbox.GetComponent<EnemyHitbox>().DeactivateHitbox();
			}
		}

		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

        audioManager.PlaySFX(audioManager.dodge);

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
		rollDirection = movement.normalized;
		anim.SetBool("dodge", true);

		StartCoroutine(ReactivateHitbox(rollDuration));
	}

	private void Dodge()
	{
		if (rollTime > 0)
		{
			transform.Translate(rollDirection * rollSpeed * Time.deltaTime);
			rollTime -= Time.deltaTime;
			
		}
		else
		{
			dodge = false;
			anim.SetBool("dodge", false);
		}
	}

	private IEnumerator Attack()
	{
		isAttacking = true;
	
		// Stop movement and reset speed in Animator
		anim.SetFloat("speed", 0);
	
		int attackCombo = 0; // Current combo index
		int maxCombo = 3; // Maximum combo attacks
		float attackDuration = 1.0f; // Time for each attack animation
		float comboResetTime = 1.5f; // Time before combo resets
		float lastInputTime = Time.time;
	
		while (isAttacking)
		{
			// Check if there is enough stamina for the attack
			if (!staminaManager.HasEnoughStamina(staminaCostAttack))
			{
				Debug.Log("Not enough stamina for attack.");
				break;
			}
	
			// Deduct stamina for the attack
			staminaManager.UseStamina(staminaCostAttack);
	
			// Enable the weapon's hitbox and trail at the start of the attack
			if (weaponHitbox != null)
			{
				weaponHitbox.SetActive(true);
				audioManager.PlaySFX(audioManager.slash);
			}
			else
			{
				Debug.LogWarning("Weapon hitbox is not assigned!");
			}

			if (weaponTrail != null)
			{
				weaponTrail.SetActive(true);
				Debug.Log("Weapon trail activated");
			}
			else
			{
				Debug.LogWarning("Weapon trail is not assigned!");
			}
	
			anim.SetFloat("attack", attackCombo + 1); // Trigger attack animation (1-based index)
			float animationTime = 0f;
	
			while (animationTime < attackDuration)
			{
				animationTime += Time.deltaTime;
	
				// Handle combo continuation
				if (Input.GetMouseButtonDown(0) && animationTime >= 0.5f)
				{
					lastInputTime = Time.time;
					if (attackCombo < maxCombo - 1)
					{
						attackCombo++;
					}
					else
					{
						attackCombo = 0; // Reset combo
					}
					break;
				}
	
				// Reset combo if no input within the reset time
				if (Time.time - lastInputTime > comboResetTime)
				{
					attackCombo = 0;
					anim.SetFloat("attack", 0); // Reset attack animation
					isAttacking = false;
	
					// Disable the weapon's hitbox and trail after the attack finishes
					if (weaponHitbox != null)
					{
						weaponHitbox.SetActive(false);
					}

					if (weaponTrail != null)
					{
						weaponTrail.SetActive(false);
						Debug.Log("Weapon trail deactivated");
					}
					yield break;
				}
	
				yield return null;
			}
	
			yield return new WaitForSeconds(0.1f); // Small delay before next combo
	
			if (attackCombo == 0) break; // Exit loop if combo is reset
		}
	
		anim.SetFloat("attack", 0); // Reset animation to idle
		isAttacking = false;
	
		// Disable the weapon's hitbox and trail after the attack ends
		if (weaponHitbox != null)
		{
			weaponHitbox.SetActive(false);
		}

		if (weaponTrail != null)
		{
			weaponTrail.SetActive(false);
			Debug.Log("Weapon trail deactivated");
		}
	}
	
	IEnumerator ResetWarcryAnimation()
	{
		yield return new WaitForSeconds(1.0f); // Adjust the delay as needed
		if (anim != null)
		{
			anim.SetBool("warcry", false);
			Debug.Log("Warcry animation reset");
		}
	}

	// Reactivate hitboxes after a delay
	private IEnumerator ReactivateHitbox(float duration)
	{
		yield return new WaitForSeconds(duration);
		if (enemyHitboxes != null)
		{
			foreach (GameObject hitbox in enemyHitboxes)
			{
				if (hitbox != null)
				{
					hitbox.SetActive(true);
					Debug.Log("Enemy hitbox reactivated: " + hitbox.name);
				}
				else
				{
					Debug.LogWarning("A hitbox in enemyHitboxes is null during reactivation!");
				}
			}
		}
		else
		{
			Debug.LogError("enemyHitboxes is null when trying to reactivate hitboxes.");
		}
	}

	void OnAnimatorMove()
	{
		// Apply root motion only when the character is attacking
		if (anim && isAttacking)
		{
			Vector3 deltaPosition = anim.deltaPosition + rootMotionDelta;
			Quaternion deltaRotation = anim.deltaRotation;

			// Move and rotate the character
			rb.MovePosition(rb.position + deltaPosition);
			rb.MoveRotation(rb.rotation * deltaRotation);

			// Reset accumulated manual root motion after applying
			rootMotionDelta = Vector3.zero;
		}
	}
}