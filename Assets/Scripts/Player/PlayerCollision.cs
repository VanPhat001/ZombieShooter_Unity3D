using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    // private void OnCollisionEnter(Collision other) {
    //     Debug.Log(other.transform.tag);
    // }

    Transform FindGameObject(Transform tf, string tag)
    {
        if (tf == null)
        {
            return null;
        }
        if (tf.tag == tag)
        {
            return tf;
        }
        return FindGameObject(tf.parent, tag);
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        if (tag.Equals("ZombieRightHand"))
        {
            var zombie = FindGameObject(other.transform, "Zombie");
            float attackDamage = 4f;
            if (zombie != null)
            {
                ZombieController zombieController = zombie.GetComponent<ZombieController>();
                attackDamage = zombieController.zombieAttackDamage;
            }

            PlayerController.Instance.ReceiveDamage(attackDamage);
            PlayerController.Instance.PlayHurtSound();
        }
        else if (tag.Equals("ZombieCyberSword"))
        {
            // CyberZombieController cyber = other.GetComponent<CyberZombieController>();
            PlayerController.Instance.ReceiveDamage(damage: 6);
            PlayerController.Instance.PlayHurtSound();
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
            HealthPackController healthPackController = other.gameObject.GetComponent<HealthPackController>();
            if (!healthPackController.Active)
            {
                return;
            }

            PlayerController.Instance.HealPercent(0.3f);
            healthPackController.SetAvailableAfter(10);
            // Destroy(other.gameObject);
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
