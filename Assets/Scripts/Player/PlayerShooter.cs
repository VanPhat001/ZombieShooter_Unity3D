using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public static PlayerShooter Instance { get; private set; }

    float timeBetweenShoots => PlayerController.Instance.gunController.timeBetweenShoots;
    float tick = 0;

    private void Start()
    {
        Instance = this;
    }

    public void ShootObject()
    {

    }

    public void ShootZombie(RaycastHit hit)
    {
        var zombie = hit.transform.GetComponent<ZombieController>();
        zombie.ReceiveDamage(damage: PlayerController.Instance.gunController.damage);
        if (zombie.currentHP <= 0)
        {
            CanvasController.Instance.AddScore(zombie.zombieScore);
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
        rigid.AddForce(PlayerController.Instance.grenadeThrowingVector);

        newGrenade.GetComponent<GrenadeController>().Explosion();
    }

    public void HideGrenadeTrajectory()
    {
        PlayerController.Instance.line.positionCount = 0;
    }

    public void ShowGrenadeTrajectory(Vector3 forceVector, Rigidbody rigidbody, Vector3 startingPoint)
    {
        int _lineSementCount = 10;
        var linePoints = new List<Vector3>();

        Vector3 velocity = (forceVector / rigidbody.mass) * Time.fixedDeltaTime;
        float flightDuration = (2 * velocity.y) / Physics.gravity.y;
        float stepTime = flightDuration / _lineSementCount;

        linePoints.Clear();

        for (int i = 0; i < _lineSementCount; i++)
        {
            float stepTimePassed = stepTime * i;

            Vector3 movementVector = new Vector3(
                velocity.x * stepTimePassed,
                velocity.y * stepTimePassed - 0.5f * Physics.gravity.y * stepTimePassed * stepTimePassed,
                velocity.z * stepTimePassed
            );

            RaycastHit hit;
            if (Physics.Raycast(startingPoint, -movementVector, out hit, movementVector.magnitude))
            {
                break;
            }

            linePoints.Add(-movementVector + startingPoint);
        }

        PlayerController.Instance.line.positionCount = linePoints.Count;
        PlayerController.Instance.line.SetPositions(linePoints.ToArray());
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

        if (!player.useGun)
        {
            Transform grenade = player.grenadeWrapper.transform.GetChild(0);
            GameObject cam = player.fpsCamera;
            Rigidbody rigid = grenade.GetComponent<Rigidbody>();
            ShowGrenadeTrajectory(player.grenadeThrowingVector, rigid, grenade.position);
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
