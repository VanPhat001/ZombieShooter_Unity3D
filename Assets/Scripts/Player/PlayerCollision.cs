using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    // private void OnCollisionEnter(Collision other) {
    //     Debug.Log(other.transform.tag);
    // }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        if (tag.Equals("ZombieRightHand"))
        {
            // ZombieController zombie = other.GetComponent<ZombieController>();
            PlayerController.Instance.ReceiveDamage(10);
        }
        else if (tag.Equals("ZombieCyberSword"))
        {
            // CyberZombieController cyber = other.GetComponent<CyberZombieController>();
            PlayerController.Instance.ReceiveDamage(damage: 14);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        string tag = other.tag;
        if (tag.Equals("WeaponsPack"))
        {
            CanvasController.Instance.SetVisibleSuggestReloadText(true);
        }
        else if (tag.Equals("HealthPack"))
        {
            PlayerController.Instance.HealPercent(0.3f);
            Destroy( other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string tag = other.tag;
        if (tag.Equals("WeaponsPack"))
        {
            CanvasController.Instance.SetVisibleSuggestReloadText(false);
        }
    }
}
