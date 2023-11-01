using System.Collections;
using System.Net.Http.Headers;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public AudioClip fireSound;
    public int maxBullets;
    public int maxBulletsInMagazine;
    public int currentTotalBullets;
    public int currentBulletsInMagazine;
    public float damage;
    public float recoil;
    public float timeBetweenShoots;
    public float timeReload;

    public bool Shoot()
    {
        if (currentBulletsInMagazine <= 0)
        {
            return false;
        }

        this.currentBulletsInMagazine--;
        return true;
    }

    IEnumerator CoroutineReload(int delta)
    {
        yield return new WaitForSeconds(this.timeReload);

        if (delta > this.currentTotalBullets)
        {
            delta = this.currentTotalBullets;
        }

        this.currentBulletsInMagazine += delta;
        this.currentTotalBullets -= delta;
    }

    public bool Reload()
    {
        int delta = this.maxBulletsInMagazine - this.currentBulletsInMagazine;

        if (delta == 0)
        {
            return false;
        }

        StartCoroutine("CoroutineReload", delta);
        return true;
    }
}
