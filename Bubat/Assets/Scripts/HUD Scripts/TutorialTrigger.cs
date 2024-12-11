using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
	public GameObject tutorialCanvas;
	public float displayDuration = 5f;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log("Halo");
			tutorialCanvas.SetActive(true);
			Invoke("HideCanvas", displayDuration);
		}
	}

	private void HideCanvas()
	{
		tutorialCanvas.SetActive(false);
	}
}
