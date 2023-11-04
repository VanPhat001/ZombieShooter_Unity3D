using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CyberZombieController : MonoBehaviour
{
    public GameObject headGun;
    public Slider healthBar;
    public GameObject goal;
    public float zombieScore = 25f;
    // public float zombieAttackDamage = 14f;
    public float zombieShootDamage = 18f;
    public float currentHP { get; private set; } = 100;
    public float maxHP { get; private set; } = 100;
    public bool isDead { get; private set; } = false;
    public bool shooting { get; private set; } = false;
    public AudioClip deathSound;
    public AudioClip fireSound;

    NavMeshAgent agent;
    Animator animator;
    AudioSource audioSource;
    LineRenderer line;

    private void Start()
    {
        this.agent = this.GetComponent<NavMeshAgent>();
        this.animator = this.GetComponent<Animator>();
        this.audioSource = this.GetComponent<AudioSource>();
        UpdateHealthBar();

        this.line = this.AddComponent<LineRenderer>();
        this.line.startWidth = 0.01f;
        this.line.endWidth = 0.01f;
        this.line.positionCount = 2;
        this.line.SetPositions(new Vector3[] {
            this.headGun.transform.position,
            this.headGun.transform.position
        });
    }

    public void ReceiveDamage(float damage)
    {
        this.currentHP = Mathf.Clamp(this.currentHP - damage, 0, this.maxHP);
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        this.healthBar.value = this.currentHP / this.maxHP;
    }


    void MoveToGoal()
    {
        this.agent.destination = this.goal.transform.position;
    }

    float GetSpeedAction()
    {
        return this.animator.GetFloat("speed");
    }

    void SetSpeedAction(float value)
    {
        if (GetSpeedAction() != value)
        {
            this.animator.SetFloat("speed", value);

            this.agent.speed = value > 2 ? 5.6f : 3.5f;
        }
    }

    void SetAttackAction(bool value)
    {
        if (this.animator.GetBool("attack") != value)
        {
            this.animator.SetBool("attack", value);
        }
    }

    void SetShootAction(bool value)
    {
        if (this.animator.GetBool("shoot") != value)
        {
            this.animator.SetBool("shoot", value);

            this.shooting = value;
            if (this.shooting)
            {
                StartCoroutine(CoroutineShoot());
            }
        }
    }

    void SetDeathAction(bool value)
    {
        if (this.animator.GetBool("death") != value)
        {
            this.animator.SetBool("death", value);
        }
    }

    IEnumerator CoroutineShoot()
    {
        yield return new WaitForSeconds(0.68f);

        if (this.shooting)
        {
            this.audioSource.PlayOneShot(this.fireSound);
            RaycastHit hit;
            if (Physics.Raycast(this.headGun.transform.position, -this.headGun.transform.up, out hit)
                && hit.transform.tag.Equals("Player"))
            {
                hit.transform.GetComponent<PlayerController>().ReceiveDamage(this.zombieShootDamage);
            }

            yield return new WaitForSeconds(0.68f);
            this.SetShootAction(false);
        }
    }

    void DrawTrajectory()
    {
        RaycastHit hit;
        if (this.shooting && Physics.Raycast(this.headGun.transform.position, -this.headGun.transform.up, out hit))
        {
            string tag = hit.transform.tag;
            if (!tag.StartsWith("Zombie"))
            {
                this.line.SetPositions(new Vector3[]{
                    this.headGun.transform.position,
                    hit.point
                });
            }
        }
        else
        {
            this.line.SetPositions(new Vector3[] {
                this.headGun.transform.position,
                this.headGun.transform.position
            });
        }
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
            this.goal = this.gameObject;
            Destroy(this.GetComponent<CapsuleCollider>());

            SetSpeedAction(-Time.deltaTime);
            SetShootAction(false);
            SetAttackAction(false);
            SetDeathAction(true);

            this.audioSource.PlayOneShot(this.deathSound);

            Destroy(this.gameObject, 3f);
            return;
        }

        DrawTrajectory();

        float distance = Vector3.Distance(this.transform.position, this.goal.transform.position);
        if (distance > 7)
        {
            MoveToGoal();
            SetSpeedAction(GetSpeedAction() + Time.deltaTime);
            SetShootAction(false);
            SetAttackAction(false);
        }
        else if (distance > 1.8)
        {
            this.agent.speed = 0;
            SetSpeedAction(-Time.deltaTime);
            if (!this.shooting)
            {
                SetShootAction(true);
            }
            SetAttackAction(false);
        }
        else
        {
            this.agent.speed = 0;
            SetSpeedAction(-Time.deltaTime);
            SetShootAction(false);
            SetAttackAction(true);
        }
    }
}
