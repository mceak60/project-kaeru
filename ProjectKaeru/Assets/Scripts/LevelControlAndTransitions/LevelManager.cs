﻿
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
    private Health playerHealth;

    static public string entryPoint = "spawn";

    //When the game starts, create a new player and have the camera follow it
    private void Awake()
    {
        instance = this;
        respawnPoint = instance.respawnPoints.transform.Find(LevelManager.entryPoint);

        player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        playerTransform = player.GetComponent<Transform>();
        playerHealth = player.GetComponent<Health>();

        CinemachineVirtualCamera vcam = playerCamera.GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = playerTransform;
    }


    //Creates teleports the player to the respawn position
    public void Respawn()
    {
        playerTransform.position = respawnPoint.position;
        playerHealth.health = playerHealth.numHearts;
    }

    // Teleports the player to a safe space after falling into a pit
    public void Reset()
    {
        playerTransform.position = respawnPoint.position;

    }

    // Sets the value the player will start in whenthe scene loads. Used for transitioning between scenes
    public void SetEntrance(string entrance)
    {
        entryPoint = entrance;
    }

}
