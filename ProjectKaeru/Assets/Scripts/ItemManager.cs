
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public bool hasDashPowerup = false;
    public bool hasWallClingPowerup = false;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Handles collection of dash powerup
        if (col.gameObject.CompareTag("DashPowerup"))
        {
            Destroy(col.gameObject);
            hasDashPowerup = true;
        }
        // Handles collection of wall cling powerup
        else if (col.gameObject.CompareTag("WallClingPowerup"))
        {
            Destroy(col.gameObject);
            hasWallClingPowerup = true;
        }
    }
}
