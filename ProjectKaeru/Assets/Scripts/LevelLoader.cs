using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    private LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = Object.FindObjectOfType<LevelManager>();
    }


    public void LoadNextLevel(string toScene, string entrance)
    {
        levelManager.SetEntrance(entrance);
        StartCoroutine(LoadLevel(toScene));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}
