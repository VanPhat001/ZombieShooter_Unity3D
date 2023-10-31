using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    float timeBetweenShoots = 0.2f;
    float tick = 0;

    public void ShootObject()
    {

    }

    public void ShootZombie()
    {

    }

    void Shoot()
    {
        GameObject cam = PlayerController.Instance.fpsCamera;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            if (hit.transform.tag.StartsWith("Object"))
            {
                ShootObject();
            }
            else if (hit.transform.tag.Equals("Zombie"))
            {
                ShootZombie();
            }
        }
    }

    void ThrowGrenade()
    {
        
    }

    private void Update()
    {
        this.tick += Time.deltaTime;
        if (Input.GetMouseButton(0) && this.tick >= this.timeBetweenShoots)
        {
            if (PlayerController.Instance.useGun)
            {
                Shoot();
            }
            else
            {
                ThrowGrenade();
            }
        }
    }
}
