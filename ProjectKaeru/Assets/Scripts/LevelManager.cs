
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Transform respawnPoint;
    public GameObject respawnPoints;
    public GameObject playerPrefab;
    public GameObject playerCamera;

    private GameObject player;
    private Transform playerTransform;

    static public string entryPoint = "spawn";

    //When the game starts, create a new player and have the camera follow it
    private void Awake()
    {
        instance = this;
        respawnPoint = instance.respawnPoints.transform.Find(LevelManager.entryPoint);

        player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        playerTransform = player.GetComponent<Transform>();

        CinemachineVirtualCamera vcam = playerCamera.GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = playerTransform;
    }


    //Creates teleports the player to the respawn position
    public void Respawn()
    {
        playerTransform.position = respawnPoint.position;
    }

    public void SetEntrance(string entrance)
    {
        entryPoint = entrance;
    }

}
