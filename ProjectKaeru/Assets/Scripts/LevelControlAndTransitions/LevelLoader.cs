using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition; // The animation we play to represent a scene transition
    public float transitionTime = 1f; // Amount of time we play the transition animation
    private LevelManager levelManager; // The level manager script that should be attached to some gameObject in the scene. There should be one and only one or this script will not work!

    // Start is called before the first frame update
    void Start()
    {
        levelManager = Object.FindObjectOfType<LevelManager>(); // Finds levelManager script
    }


    public void LoadNextLevel(string toScene, string entrance)
    {
        //Sets the global static entrance value for the levelManager to determine where the player will start in the scene
        levelManager.SetEntrance(entrance);
        //Plays scene transition animation
        StartCoroutine(LoadLevel(toScene));
    }

    //This starts the scene transtition animation before loading into a new scene
    IEnumerator LoadLevel(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}
