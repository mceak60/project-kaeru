using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Transform respawnPoint;
    public GameObject playerPrefab;

    //When the game starts, create a new player and have the camera follow it
    private void Awake()
    {
        instance = this;
        GameObject player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        Transform playerTransform = player.GetComponent<Transform>();

        CinemachineVirtualCamera vcam = playerCamera.GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = playerTransform;
        vcam.Follow = playerTransform;
    }

    //Creates a new player at the respawn point and has the camera follow them
    public void Respawn()
    {
        Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
    }
}
