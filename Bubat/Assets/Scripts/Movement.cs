using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
	private PlayerHealth playerHealth;
	public List<GameObject> enemyHitboxes;
	public float speed = 1.0f;
	public float rollSpeed = 5f;
	public float rollDuration = 0.5f;
	private Rigidbody rb;
	private Animator anim;

	private bool dodge = false;
	private float rollTime = 0f;
	private Vector3 rollDirection;
	private bool isAttacking = false;
	private bool isCooldown = false; // Cooldown for combo attacks

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		playerHealth = GetComponent<PlayerHealth>();

		if (playerHealth == null)
		{
			Debug.LogError("PlayerHealth component not found on this GameObject.");
		}

		if (enemyHitboxes == null)
		{
			Debug.LogError("enemyHitboxes list is not initialized.");
		}
	}

	void Update()
	{
		if (!isAttacking) // Only allow movement if not attacking
		{
			if (!dodge)
			{
				HandleMovement();
				if (Input.GetMouseButtonDown(0)) // Attack on left mouse button
				{
					StartCoroutine(Attack());
				}
				if (Input.GetKeyDown(KeyCode.Space)) // Dodge on spacebar
				{
					StartDodge();
				}
			}
			else
			{
				Dodge();
			}
		}
	}
	
	void OnAnimatorMove()
    {
        // Apply root motion automatically
        if (anim)
        {
            // Get the root motion delta position and rotation
            Vector3 deltaPosition = anim.deltaPosition;
            Quaternion deltaRotation = anim.deltaRotation;

            // Move the character based on the root motion
            rb.MovePosition(rb.position + deltaPosition);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }

	private void HandleMovement()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
		rb.transform.Translate(movement * speed * Time.deltaTime);

		if (movement.magnitude > 0)
		{
			Quaternion targetRotation = Quaternion.LookRotation(movement);
			rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 10f);
		}

		anim.SetFloat("x", moveHorizontal);
		anim.SetFloat("y", moveVertical);
		anim.SetFloat("speed", movement.magnitude);
	}

	private void StartDodge()
	{
		dodge = true;
		rollTime = rollDuration;

		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
		rollDirection = movement.normalized;
		anim.SetBool("dodge", true);
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
					yield break;
				}

				yield return null;
			}

			yield return new WaitForSeconds(0.1f); // Small delay before next combo

			if (attackCombo == 0) break; // Exit loop if combo is reset
		}

		anim.SetFloat("attack", 0); // Reset animation to idle
		isAttacking = false;
	}
}
