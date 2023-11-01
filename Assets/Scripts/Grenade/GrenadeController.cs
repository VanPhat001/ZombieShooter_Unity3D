using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    public float grenadeDamage = 40;
    public AudioClip explosionSound;
    AudioSource audioSource;

    private void Start()
    {
        this.audioSource = this.AddComponent<AudioSource>();
    }

    IEnumerator CoroutineExplosion(float timeDuration)
    {
        yield return new WaitForSeconds(timeDuration);

        this.audioSource.PlayOneShot(this.explosionSound);
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 5f);
        foreach (Collider collider in colliders)
        {
            if (collider.tag.Equals("Zombie"))
            {
                collider.GetComponent<ZombieController>().ReceiveDamage(grenadeDamage);
            }
        }

        Destroy(this.gameObject, 1.2f);
    }

    public void Explosion()
    {
        StartCoroutine("CoroutineExplosion", 3f);
    }
}
