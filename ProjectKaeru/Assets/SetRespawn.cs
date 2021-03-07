﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetRespawn : MonoBehaviour
{
    private bool interactable = false;
    private LevelManager levelManager;
    private Scene scene;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = Object.FindObjectOfType<LevelManager>();
        scene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable && Input.GetButtonDown("Enter"))
        {
            LevelManager.respawnScene = scene.name;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        interactable = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        interactable = false;
    }
}
