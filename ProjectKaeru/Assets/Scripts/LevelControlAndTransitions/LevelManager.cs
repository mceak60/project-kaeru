
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Transform resetPoint;
    public GameObject resetPoints;
    public GameObject playerPrefab;
    public GameObject playerCamera;

    private GameObject player;
    private Transform playerTransform;
    private Health playerHealth;

    static public string entryPoint = "spawn"; // The default entry point when the player first loads in is coded as the one called "spawn" currently
    static public string respawnScene = "default"; // The default respawn point is called default currently
    private LevelLoader levelLoader; // Yeah you need a level loader in the scene for this code to work now

    //When the game starts, create a new player and have the camera follow it
    private void Awake()
    {
        instance = this;
        resetPoint = instance.resetPoints.transform.Find(LevelManager.entryPoint);

        player = Instantiate(playerPrefab, resetPoint.position, Quaternion.identity);
        playerTransform = player.GetComponent<Transform>();
        playerHealth = player.GetComponent<Health>();

        CinemachineVirtualCamera vcam = playerCamera.GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = playerTransform;

        levelLoader = Object.FindObjectOfType<LevelLoader>();
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
