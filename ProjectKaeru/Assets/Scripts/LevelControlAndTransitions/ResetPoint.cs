using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPoint : MonoBehaviour
{
    private LevelManager levelManager;
    // Start is called before the first frame update
    void Start()
    {
        levelManager = Object.FindObjectOfType<LevelManager>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        levelManager.resetPoint = transform.GetChild(0).gameObject.transform;
    }
}
