
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public bool hasDashPowerup = false;
    public bool hasWallClingPowerup = false;
    public bool hasGrapplingHookPowerup = false;

    private void Start()
    {
        if (PlayerPrefs.GetInt("dash") == 1)
        {
            hasDashPowerup = true;
        }
        if (PlayerPrefs.GetInt("wallcling") == 1)
        {
            hasWallClingPowerup = true;
        }
        if (PlayerPrefs.GetInt("grapple") == 1)
        {
            hasGrapplingHookPowerup = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Handles collection of dash powerup
        if (col.gameObject.CompareTag("DashPowerup"))
        {
            Destroy(col.gameObject);
            hasDashPowerup = true;
            PlayerPrefs.SetInt("dash", 1);
        }
        // Handles collection of wall cling powerup
        else if (col.gameObject.CompareTag("WallClingPowerup"))
        {
            Destroy(col.gameObject);
            hasWallClingPowerup = true;
            PlayerPrefs.SetInt("wallcling", 1);
        }
        // Handles collection of grappling hook powerup
        else if (col.gameObject.CompareTag("GrapplingHookPowerup"))
        {
            Destroy(col.gameObject);
            hasGrapplingHookPowerup = true;
            PlayerPrefs.SetInt("grapple", 1);
        }
    }
}
