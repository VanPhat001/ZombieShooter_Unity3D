using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    // private void OnCollisionEnter(Collision other) {
    //     Debug.Log(other.transform.tag);
    // }

    private void OnTriggerEnter(Collider other) {
        tag = other.tag;
        if (tag.Equals("ZombieRightHand"))
        {
            PlayerController.Instance.ReceiveDamage(10);
        }
    }
}
