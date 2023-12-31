using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    public float grenadeDamage = 40;
    public AudioClip explosionSound;
    public GameObject grenadeExplosion;
    AudioSource audioSource;

    private void Start()
    {
        this.audioSource = this.AddComponent<AudioSource>();
    }

    IEnumerator CoroutineExplosion(float timeDuration)
    {
        yield return new WaitForSeconds(timeDuration);

        this.audioSource.PlayOneShot(this.explosionSound);
        GameObject explosionObject = Instantiate(this.grenadeExplosion, this.transform.position, Quaternion.identity);
        explosionObject.GetComponent<ParticleSystem>().Play();

        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 5f);
        foreach (Collider collider in colliders)
        {
            if (collider.tag.Equals("Zombie"))
            {
                ZombieController zombie = collider.GetComponent<ZombieController>();
                zombie.ReceiveDamage(grenadeDamage);
                if (zombie.currentHP <= 0)
                {
                    CanvasController.Instance.AddScore(zombie.zombieScore);
                }
            }
            else if (collider.tag.Equals("ZombieCyber"))
            {
                CyberZombieController zombie = collider.GetComponent<CyberZombieController>();
                zombie.ReceiveDamage(grenadeDamage);
                if (zombie.currentHP <= 0)
                {
                    CanvasController.Instance.AddScore(zombie.zombieScore);
                }
            }
        }

        Destroy(this.gameObject, 1.2f);
    }

    public void Explosion()
    {
        StartCoroutine("CoroutineExplosion", 3f);
    }
}
