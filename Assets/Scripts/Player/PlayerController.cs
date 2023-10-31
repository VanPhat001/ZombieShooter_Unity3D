using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static public PlayerController Instance { get; private set; }

    public GameObject fpsCamera;
    public GameObject weapons;
    public GameObject gunWrapper;
    public GameObject grenadeWrapper;

    public float currentHP { get; private set; } = 100;
    public float maxHP { get; private set; } = 100;

    public float speed = 5f;
    public float speedUpRate = 1.8f;
    public bool useGun { get; private set; } = true;

    Rigidbody rigid;

    private void Start()
    {
        Instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        CanvasController.Instance.SetHealth(1);
        this.rigid = this.GetComponent<Rigidbody>();
    }

    public void ReceiveDamage(float damage)
    {
        this.currentHP = Mathf.Clamp(this.currentHP - damage, 0, maxHP);
        CanvasController.Instance.SetHealth(percent: this.currentHP / this.maxHP);
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

    void RotateHead()
    {
        float vertical = Input.GetAxis("Mouse Y");
        if (vertical != 0)
        {
            this.fpsCamera.transform.Rotate(-vertical * 0.7f, 0, 0);

            Vector3 euler = this.fpsCamera.transform.rotation.eulerAngles;
            float maxBottomRotation = 30;
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
    }

    bool onGround()
    {
        return Physics.Raycast(this.transform.position, Vector3.down, 0.750001f);
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && onGround())
        {
            this.rigid.AddForce(Vector3.up * 200);
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
            this.useGun = true;
            this.gunWrapper.SetActive(true);
            this.grenadeWrapper.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            this.useGun = false;
            this.gunWrapper.SetActive(false);
            this.grenadeWrapper.SetActive(true);
        }
    }

    void PickWeapon()
    {

    }

    private void Update()
    {
        Move();
        RotateBody();
        RotateHead();
        Jump();
        Zoom();
        ChangeWeapon();
        PickWeapon();
    }
}
