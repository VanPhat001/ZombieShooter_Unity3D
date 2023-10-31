using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public GameObject goal;
    public float currentHP { get; private set; } = 100;
    public float maxHP { get; private set; } = 100;
    public bool isDead { get; private set; } = false;

    NavMeshAgent agent;
    Animator animator;

    private void Start()
    {
        this.agent = this.GetComponent<NavMeshAgent>();
        this.animator = this.GetComponent<Animator>();
        SetWalkAction(true);
    }

    public void ReceiveDamage(float damage)
    {
        this.currentHP = Mathf.Clamp(this.currentHP - damage, 0, this.maxHP);
        Debug.Log(this.currentHP);
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
        if (!this.isDead && this.currentHP <= 0)
        {
            this.isDead = true;
            this.goal = this.gameObject;

            SetWalkAction(false);
            SetAttackAction(false);
            SetDeathAction(true);

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
