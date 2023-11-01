using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    float timeBetweenShoots = 0.1f;
    float tick = 0;

    public void ShootObject()
    {

    }

    public void ShootZombie(RaycastHit hit)
    {
        var zombie = hit.transform.GetComponent<ZombieController>();
        zombie.ReceiveDamage(damage: 20);
        if (zombie.currentHP <= 0)
        {
            CanvasController.Instance.AddScore(10);
        }
    }

    void Shoot()
    {
        GameObject cam = PlayerController.Instance.fpsCamera;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            string tag = hit.transform.tag;
            Debug.Log(tag);
            
            if (tag.StartsWith("Object"))
            {
                ShootObject();
                PlayerController.Instance.PlayFireSound();
            }
            else if (tag.Equals("Zombie"))
            {
                ShootZombie(hit);
                PlayerController.Instance.PlayFireSound();
            }
        }
    }

    void ThrowGrenade()
    {
        GameObject cam = PlayerController.Instance.fpsCamera;
        Transform grenade = PlayerController.Instance.grenadeWrapper.transform.GetChild(0);
        GameObject newGrenade = Instantiate(grenade.gameObject, grenade.position, grenade.rotation);
        Rigidbody rigid = newGrenade.GetComponent<Rigidbody>();

        rigid.isKinematic = false;
        rigid.AddForce((cam.transform.forward + cam.transform.up) * 220);

        newGrenade.GetComponent<GrenadeController>().Explosion();
    }

    private void Update()
    {
        this.tick += Time.deltaTime;
        if (Input.GetMouseButton(0) && this.tick >= this.timeBetweenShoots)
        {
            if (PlayerController.Instance.useGun)
            {
                this.tick = 0;
                Shoot();
            }
            else
            {
                this.tick = 0;
                ThrowGrenade();
            }
        }
    }
}
