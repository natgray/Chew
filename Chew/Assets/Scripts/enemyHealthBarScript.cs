using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHealthBarScript : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        healthBarScript.health -= 10f;
    }
}
