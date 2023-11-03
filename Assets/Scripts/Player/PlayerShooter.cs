using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    float timeBetweenShoots => PlayerController.Instance.gunController.timeBetweenShoots;
    float tick = 0;

    public void ShootObject()
    {

    }

    public void ShootZombie(RaycastHit hit)
    {
        var zombie = hit.transform.GetComponent<ZombieController>();
        zombie.ReceiveDamage(damage: PlayerController.Instance.gunController.damage);
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

            bool isShoot = false;
            if (tag.StartsWith("Object") || tag.Equals("House"))
            {
                isShoot = true;
                ShootObject();
            }
            else if (tag.Equals("Zombie"))
            {
                isShoot = true;
                ShootZombie(hit);
            }

            if (isShoot)
            {
                PlayerController player = PlayerController.Instance;
                player.AddRecoil(player.gunController.recoil);
                player.PlayFireSound();
                player.AddBulletImpact(hit.point);
                player.gunController.Shoot();
                player.UpdateBulletOnScreen();
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
        PlayerController player = PlayerController.Instance;
        GunController gunController = player.gunController;

        if (player.useGun
            && !player.reloading
            && gunController.currentBulletsInMagazine == 0
            && gunController.currentTotalBullets > 0)
        {
            StartCoroutine(PlayerController.Instance.CoroutineLoadBulletsIntoMagazine());
        }

        if (Input.GetMouseButton(0))
        {
            if (player.useGun)
            {
                if (player.reloading
                    || this.tick < this.timeBetweenShoots
                    || gunController.currentBulletsInMagazine <= 0)
                {
                    return;
                }

                this.tick = 0;
                Shoot();
            }
            else // use grenade
            {
                if (this.tick < 0.4f)
                {
                    return;
                }

                this.tick = 0;
                ThrowGrenade();
            }
        }
    }
}
