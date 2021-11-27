
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Transform resetPoint;
    public GameObject resetPoints;
    public GameObject playerPrefab;
    public GameObject playerCamera;

    private GameObject player;
    private Transform playerTransform;
    private PlayerHealth playerHealth;

    static public string entryPoint = "devSpawn"; // The default entry point when the player first loads in is coded as the one called "spawn" currently
    static public string respawnScene; // The default respawn scene wiill be whichever scene is open when the game starts up
    private LevelLoader levelLoader; // The level loader is required to reload the scene when you die

    //When the game starts, create a new player and have the camera follow it
    private void Awake()
    {
        instance = this;
        resetPoints = GameObject.Find("RespawnPoints");
        resetPoint = instance.resetPoints.transform.Find(LevelManager.entryPoint);
        player = Instantiate(playerPrefab, resetPoint.position, Quaternion.identity);
        playerTransform = player.GetComponent<Transform>();
        playerHealth = player.GetComponent<PlayerHealth>();

        CinemachineVirtualCamera vcam = playerCamera.GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = playerTransform;

        levelLoader = Object.FindObjectOfType<LevelLoader>();
        LevelManager.respawnScene = SceneManager.GetActiveScene().name; // Sets default respawn scene to the whichever scene is open when the game is started
    }


    //Creates teleports the player to the respawn position
    public void Respawn()
    {
        levelLoader.LoadNextLevel(respawnScene, "spawn");
        //playerTransform.position = resetPoint.position;
        playerHealth.health = playerHealth.numHearts;
    }

    // Teleports the player to a safe space after falling into a pit
    public void Reset()
    {
        playerTransform.position = resetPoint.position;

    }

    // Sets the value the player will start in whenthe scene loads. Used for transitioning between scenes
    public void SetEntrance(string entrance)
    {
        entryPoint = entrance;
    }

}
