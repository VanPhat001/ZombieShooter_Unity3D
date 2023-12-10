using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ZombieController : MonoBehaviour
{
    public Slider healthBar;
    public GameObject goal;
    public float zombieScore = 10f;
    public float zombieAttackDamage = 4f;
    public float currentHP { get; private set; } = 100;
    public float maxHP { get; private set; } = 100;
    public bool isDead { get; private set; } = false;
    public AudioClip deathSound;
    public Material superMaterial;

    NavMeshAgent agent;
    Animator animator;
    AudioSource audioSource;

    private void Start()
    {
        this.agent = this.GetComponent<NavMeshAgent>();
        this.animator = this.GetComponent<Animator>();
        this.audioSource = this.GetComponent<AudioSource>();
        UpdateHealthBar();
        SetWalkAction(true);
    }

    void ApplyMaterialToAllChildren(Transform parent)
    {
        if (parent.GetComponent<Renderer>() != null)
        {
            parent.GetComponent<Renderer>().material = this.superMaterial;
        }

        foreach (Transform child in parent)
        {
            ApplyMaterialToAllChildren(child);
        }
    }

    public void UpgradeToSuperState()
    {
        this.zombieScore *= 2;
        this.zombieAttackDamage *= 1.6f;
        this.GetComponent<NavMeshAgent>().speed *= 2.4f; // don't use this.agent at here, because this.agent has no value
        ApplyMaterialToAllChildren(this.transform);
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

    void SetWalkAction(bool value)
    {
        if (this.animator.GetBool("walk") != value)
        {
            this.animator.SetBool("walk", value);
        }
    }

    void SetAttackAction(bool value)
    {
        if (this.animator.GetBool("attack") != value)
        {
            this.animator.SetBool("attack", value);
        }
    }

    void SetDeathAction(bool value)
    {
        if (this.animator.GetBool("death") != value)
        {
            this.animator.SetBool("death", value);
        }
    }

    void MoveToGoal()
    {
        this.agent.destination = this.goal.transform.position;
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

            SetWalkAction(false);
            SetAttackAction(false);
            SetDeathAction(true);

            this.audioSource.PlayOneShot(this.deathSound);

            Destroy(this.gameObject, 1.5f);
            return;
        }

        MoveToGoal();

        if (Vector3.Distance(this.transform.position, this.goal.transform.position) <= 1.7)
        {
            SetWalkAction(false);
            SetAttackAction(true);
        }
        else
        {
            SetWalkAction(true);
            SetAttackAction(false);
        }
    }
}
