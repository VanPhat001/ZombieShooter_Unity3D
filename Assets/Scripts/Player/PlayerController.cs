using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static public PlayerController Instance { get; private set; }

    public GameObject fpsCamera;
    public GameObject weapons;
    public GameObject gunWrapper;
    public GameObject grenadeWrapper;
    public GameObject bulletImpact;

    public int grenadeRemains = 5;
    public float jumpForce = 100f;
    public float currentHP { get; private set; } = 100;
    public float maxHP { get; private set; } = 100;
    public bool reloading { get; private set; } = false;

    public float speed = 3.5f;
    public bool isDead { get; private set; } = false;
    public float speedUpRate = 1.8f;
    public bool useGun { get; private set; } = true;
    public float grenadeThrowingForce = 280f;
    public Vector3 grenadeThrowingVector => (this.fpsCamera.transform.forward + this.fpsCamera.transform.up) * this.grenadeThrowingForce;
    public GunController gunController { get; private set; }
    public LineRenderer line { get; private set; }
    public AudioClip hurtSound;


    float totalRecoilUp = 0;
    Rigidbody rigid;
    AudioSource audioSource;
    GameObject detectGun = null;

    public GameObject DetectGun
    {
        get => detectGun;
        set
        {
            if (detectGun == value)
            {
                return;
            }

            if (detectGun != null)
            {
                Renderer gunRenderer = detectGun.GetComponent<Renderer>();
                Color customColor = new Color(1, 1, 1, 1);
                gunRenderer.material.SetColor("_Color", customColor);
            }

            detectGun = value;
            if (detectGun != null)
            {
                Renderer gunRenderer = detectGun.GetComponent<Renderer>();
                Color customColor = new Color(0, 0, 1, 1);
                gunRenderer.material.SetColor("_Color", customColor);
            }
        }
    }

    private void Start()
    {
        // this.currentHP = 10f;
        Instance = this;

        GameController.Instance.Continue();
        Cursor.lockState = CursorLockMode.Locked;
        CanvasController.Instance.UpdateBestScoreText();
        CanvasController.Instance.SetHealth(this.currentHP / this.maxHP);

        this.rigid = this.GetComponent<Rigidbody>();
        this.audioSource = this.GetComponent<AudioSource>();
        this.line = this.GetComponent<LineRenderer>();

        GameObject gun = this.gunWrapper.transform.GetChild(0).gameObject;
        this.gunController = gun.GetComponent<GunController>();

        UpdateBulletOnScreen();
    }

    public void UpdateBulletOnScreen()
    {
        string text = this.useGun ?
                        $"{this.gunController.currentBulletsInMagazine}\\{this.gunController.currentTotalBullets}"
                        : this.grenadeRemains.ToString();
        CanvasController.Instance.SetBulletRemainText(text);
    }

    public void ReceiveDamage(float damage)
    {
        this.currentHP = Mathf.Clamp(this.currentHP - damage, 0, maxHP);
        CanvasController.Instance.SetHealth(percent: this.currentHP / this.maxHP);
    }

    public void HealAmount(float value)
    {
        this.currentHP = Mathf.Clamp(this.currentHP + value, 0, maxHP);
        CanvasController.Instance.SetHealth(percent: this.currentHP / this.maxHP);
    }

    public void HealPercent(float percent)
    {
        this.currentHP = Mathf.Clamp(this.currentHP + this.maxHP * percent, 0, maxHP);
        CanvasController.Instance.SetHealth(percent: this.currentHP / this.maxHP);
    }

    public void PlayFireSound()
    {
        this.audioSource.PlayOneShot(this.gunController.fireSound);
    }

    public void PlayHurtSound()
    {
        this.audioSource.PlayOneShot(this.hurtSound);
    }

    public void AddBulletImpact(Vector3 pos)
    {
        GameObject bulletImpactPS = Instantiate(this.bulletImpact, pos, this.fpsCamera.transform.rotation);
        bulletImpactPS.transform.Rotate(0, 0, 180);
        bulletImpactPS.GetComponent<ParticleSystem>().Play();

        Destroy(bulletImpactPS, 1f);
    }

    void Move()
    {
        float speed = Time.deltaTime * this.speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= this.speedUpRate;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.Translate(Vector3.forward * speed);
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.Translate(Vector3.back * speed);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Translate(Vector3.left * speed);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Translate(Vector3.right * speed);
        }
    }

    void RotateBody()
    {
        float horizontal = Input.GetAxis("Mouse X");
        if (horizontal != 0)
        {
            this.transform.Rotate(0, horizontal, 0);
        }
    }

    void LimitRotateHeadInRange()
    {
        Vector3 euler = this.fpsCamera.transform.rotation.eulerAngles;
        float maxBottomRotation = 44;
        float maxTopRotation = -30;
        float newX = 0;

        if (maxBottomRotation < euler.x && euler.x < 360 + maxTopRotation)
        {
            float dBot = euler.x - maxBottomRotation;
            float dTop = 360 + maxTopRotation - euler.x;
            newX = dBot < dTop ? maxBottomRotation : 360 + maxTopRotation;
        }

        if (newX != 0)
        {
            this.fpsCamera.transform.rotation = Quaternion.Euler(newX, euler.y, euler.z);
        }
        this.weapons.transform.rotation = this.fpsCamera.transform.rotation;
    }

    void RotateHead()
    {
        float vertical = Input.GetAxis("Mouse Y");
        if (vertical != 0)
        {
            this.fpsCamera.transform.Rotate(-vertical * 0.7f, 0, 0);
            LimitRotateHeadInRange();
            totalRecoilUp = 0;
            return;
        }

        if (this.totalRecoilUp != 0)
        {
            float restoreValue = this.totalRecoilUp < 0.05f ? this.totalRecoilUp : 0.05f;
            AddRecoil(-restoreValue, true);
            this.totalRecoilUp -= restoreValue;
        }
    }

    bool OnGround()
    {
        // return Physics.Raycast(this.transform.position, Vector3.down, 0.750001f);
        return this.rigid.velocity.y == 0;
    }

    void Jump()
    {
        // Debug.Log(this.rigid.velocity);
        if (Input.GetKey(KeyCode.Space) && OnGround())
        {
            this.rigid.AddForce(Vector3.up * this.jumpForce);
        }
    }

    void Zoom()
    {
        Camera cam = this.fpsCamera.GetComponent<Camera>();
        if (Input.GetKey(KeyCode.F))
        {
            cam.fieldOfView -= 0.8f;
        }
        else
        {
            cam.fieldOfView += 0.8f;
        }
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 10, 60);
    }

    void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // use gun
            Transform gun = this.gunWrapper.transform.GetChild(0);
            GunController gunController = gun.GetComponent<GunController>();
            string bulletRemainsText = gunController.currentBulletsInMagazine + "/" + gunController.currentTotalBullets;

            this.useGun = true;
            this.gunWrapper.SetActive(true);
            this.grenadeWrapper.SetActive(false);
            CanvasController.Instance.SetVisibleSight(true);
            CanvasController.Instance.SetBulletRemainText(bulletRemainsText);
            PlayerShooter.Instance.HideGrenadeTrajectory();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // use grenade
            this.useGun = false;
            this.gunWrapper.SetActive(false);
            this.grenadeWrapper.SetActive(true);
            CanvasController.Instance.SetVisibleSight(false);
            CanvasController.Instance.SetBulletRemainText(this.grenadeRemains.ToString());

            CancelReload();
        }
    }

    void CancelReload()
    {
        if (this.reloading)
        {
            this.reloading = false;
            CanvasController.Instance.ForceStopReload();
        }
    }

    void PickWeapon()
    {
        if (Input.GetMouseButtonDown(1) && this.DetectGun != null)
        {
            CancelReload();

            Transform oddGun = this.gunWrapper.transform.GetChild(0);
            Transform newGun = this.DetectGun.transform;

            newGun.transform.position = oddGun.transform.position;
            newGun.transform.rotation = oddGun.transform.rotation;

            // drop gun
            oddGun.SetParent(null);
            oddGun.GetComponent<BoxCollider>().isTrigger = false;
            Rigidbody rigid = oddGun.GetComponent<Rigidbody>();
            rigid.isKinematic = false;
            rigid.AddForce((this.transform.forward + Vector3.up) * 250);

            // pick gun
            newGun.SetParent(this.gunWrapper.transform);
            newGun.GetComponent<BoxCollider>().isTrigger = true;
            rigid = newGun.GetComponent<Rigidbody>();
            rigid.isKinematic = true;

            this.gunController = newGun.GetComponent<GunController>();
            UpdateBulletOnScreen();
        }
    }

    void DetectWeapon()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.fpsCamera.transform.position, this.fpsCamera.transform.forward, out hit, 3f))
        {
            if (hit.transform.tag.Equals("Gun"))
            {
                this.DetectGun = hit.transform.gameObject;
                return;
            }
        }

        this.DetectGun = null;
    }

    public IEnumerator CoroutineLoadBulletsIntoMagazine()
    {
        this.reloading = true;
        yield return CanvasController.Instance.CoroutineStartReload(this.gunController.timeReload);
        this.gunController.Reload();
        UpdateBulletOnScreen();
        this.reloading = false;
    }

    public void AddRecoil(float recoilUp, bool restoreToBalanceRotation = false)
    {
        if (!restoreToBalanceRotation)
        {
            totalRecoilUp += recoilUp;
        }
        this.fpsCamera.transform.Rotate(-recoilUp, 0, 0);
        LimitRotateHeadInRange();
    }

    private void Update()
    {
        if (this.isDead)
        {
            return;
        }

        if (this.currentHP <= 0)
        {
            this.isDead = true;
            Cursor.lockState = CursorLockMode.None;
            GameController.Instance.UpdateBestScore(CanvasController.Instance.score);
            GameController.Instance.Pause();
            CanvasController.Instance.UpdateBestScoreText();
            CanvasController.Instance.SetVisibleRestartMenuBard(true);
            return;
        }

        if (CanvasController.Instance.suggestReloadTextVisible
            && Input.GetKeyDown(KeyCode.C))
        {
            this.gunController.RestoreToMaxBullets();
            UpdateBulletOnScreen();
        }

        if (!this.reloading && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(CoroutineLoadBulletsIntoMagazine());
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CanvasController.Instance.restartMenuBoard.activeInHierarchy)
            {
                GameController.Instance.Continue();
                CanvasController.Instance.SetVisibleRestartMenuBard(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                GameController.Instance.Pause();
                CanvasController.Instance.SetVisibleRestartMenuBard(true);
                Cursor.lockState = CursorLockMode.None;
            }
        }

        Move();
        RotateBody();
        RotateHead();
        Jump();
        Zoom();
        ChangeWeapon();
        DetectWeapon();
        PickWeapon();
    }
}
