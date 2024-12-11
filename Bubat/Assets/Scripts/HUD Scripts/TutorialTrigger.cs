using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Required for using TextMeshPro

public class TutorialTrigger : MonoBehaviour
{
    public GameObject tutorialCanvas; // The tutorial UI canvas
    public TMP_Text tutorialText; // TextMeshPro text component for messages
    public float displayDuration = 5f; // Time to display the canvas

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered tutorial trigger.");
            tutorialCanvas.SetActive(true); // Show the tutorial canvas
            CancelInvoke("HideCanvas"); // Cancel any previous HideCanvas invocations
            Invoke("HideCanvas", displayDuration); // Hide the canvas after displayDuration
        }
    }

    private void HideCanvas()
    {
        tutorialCanvas.SetActive(false); // Hide the tutorial canvas
    }
}
