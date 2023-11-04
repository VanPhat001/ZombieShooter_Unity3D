using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CyberZombieController : MonoBehaviour
{
    public GameObject gun;
    public Slider healthBar;
    public GameObject goal;
    public float zombieScore = 10f;
    public float currentHP { get; private set; } = 100;
    public float maxHP { get; private set; } = 100;
    public bool isDead { get; private set; } = false;
    public AudioClip deathSound;

    NavMeshAgent agent;
    Animator animator;
    AudioSource audioSource;

    private void Start()
    {
        this.agent = this.GetComponent<NavMeshAgent>();
        this.animator = this.GetComponent<Animator>();
        this.audioSource = this.GetComponent<AudioSource>();
        UpdateHealthBar();
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
        }
    }

    void SetDeathAction(bool value)
    {
        if (this.animator.GetBool("death") != value)
        {
            this.animator.SetBool("death", value);
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

            Destroy(this.gameObject, 1.5f);
            return;
        }


        float distance = Vector3.Distance(this.transform.position, this.goal.transform.position);
        if (distance > 5)
        {
            MoveToGoal();
            SetSpeedAction(GetSpeedAction() + Time.deltaTime);
            SetShootAction(false);
            SetAttackAction(false);
        }
        else if (distance > 2)
        {
            this.agent.destination = this.transform.position;
            SetSpeedAction(-Time.deltaTime);
            SetShootAction(true);
            SetAttackAction(false);
        }
        else
        {
            this.agent.destination = this.transform.position;
            SetSpeedAction(-Time.deltaTime);
            SetShootAction(false);
            SetAttackAction(true);
        }
    }
}
