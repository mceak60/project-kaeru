
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
        else 
        {
            hasDashPowerup = false;
        }

        if (PlayerPrefs.GetInt("wallcling") == 1)
        {
            hasWallClingPowerup = true;
        }
        else
        {
            hasWallClingPowerup = false;
        }

        if (PlayerPrefs.GetInt("grapple") == 1)
        {
            hasGrapplingHookPowerup = true;
        }
        else
        {
            hasGrapplingHookPowerup = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Handles collection of dash powerup
        if (col.gameObject.CompareTag("DashPowerup"))
        {
            Destroy(col.gameObject);
            hasDashPowerup = true;
            PlayerPrefs.SetInt("dash", 0);
        }
        // Handles collection of wall cling powerup
        else if (col.gameObject.CompareTag("WallClingPowerup"))
        {
            Destroy(col.gameObject);
            hasWallClingPowerup = true;
            PlayerPrefs.SetInt("wallcling", 0);
        }
        // Handles collection of grappling hook powerup
        else if (col.gameObject.CompareTag("GrapplingHookPowerup"))
        {
            Destroy(col.gameObject);
            hasGrapplingHookPowerup = true;
            PlayerPrefs.SetInt("grapple", 0);
        }
        else if (col.gameObject.CompareTag("ResetPowerUp"))
        {
            Destroy(col.gameObject);
            hasDashPowerup = false;
            hasWallClingPowerup = false;
            hasGrapplingHookPowerup = false;

            PlayerPrefs.SetInt("dash", 0);
            PlayerPrefs.SetInt("wallcling", 0);
            PlayerPrefs.SetInt("grapple", 0);
        }
    }
}
