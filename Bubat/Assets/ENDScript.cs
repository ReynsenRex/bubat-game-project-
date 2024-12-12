using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class ENDScript : MonoBehaviour
{
    public PlayableDirector playableDirector; // Reference to the PlayableDirector
    //public string nextSceneName; // Name of the next scene to load

    private void Start()
    {
        if (playableDirector != null)
        {
            // Subscribe to the stopped event
            playableDirector.stopped += OnTimelineStopped;
        }
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        if (director == playableDirector)
        {
            // Load the next scene
            SceneManager.LoadScene(0);
        }
    }

    private void OnDestroy()
    {
        if (playableDirector != null)
        {
            // Unsubscribe from the event to avoid memory leaks
            playableDirector.stopped -= OnTimelineStopped;
        }
    }
}
