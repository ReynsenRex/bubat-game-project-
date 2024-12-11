using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerScript : MonoBehaviour
{
    /*Scene currScene;

    void Start()
    {
        currScene = GetComponent<Scene>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    public void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
