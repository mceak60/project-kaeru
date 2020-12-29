using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Transform respawnPoint;
    public GameObject playerPrefab;
    public GameObject playerCamera;

    private void Awake()
    {
        instance = this;
        GameObject player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        Transform playerTransform = player.GetComponent<Transform>();

        CinemachineVirtualCamera vcam = playerCamera.GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = playerTransform;
        vcam.Follow = playerTransform;
    }

    public void Respawn()
    {
        GameObject player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        Transform playerTransform = player.GetComponent<Transform>();

        CinemachineVirtualCamera vcam = playerCamera.GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = playerTransform;
        vcam.Follow = playerTransform;
    }
}
