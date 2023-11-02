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
            PlayerController.Instance.ReceiveDamage(10);
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
