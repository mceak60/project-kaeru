using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public Rigidbody2D rb;
    // Start is called before the first frame update

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Here");
            col.transform.parent.GetComponent<Enemy>().TakeDamage(40, rb);
        }
    }
}
