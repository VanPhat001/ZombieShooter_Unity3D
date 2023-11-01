using System.Collections;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;

public class GhoulZombieController : MonoBehaviour
{
    public GameObject goal;
    public float currentHP { get; private set; } = 100;
    public float maxHP { get; private set; } = 100;
    public bool isDead { get; private set; } = false;
    public AudioClip deathSound;

    bool canAttack = true;
    NavMeshAgent agent;
    Animation anima;
    AudioSource audioSource;

    private void Start()
    {
        this.agent = this.GetComponent<NavMeshAgent>();
        this.anima = this.GetComponent<Animation>();
        this.audioSource = this.GetComponent<AudioSource>();
    }

    public void ReceiveDamage(float damage)
    {
        this.currentHP = Mathf.Clamp(this.currentHP - damage, 0, this.maxHP);
    }

    IEnumerator CoroutineEnableAttack()
    {
        yield return new WaitForSeconds(1.6f);
        this.canAttack = true;
    }

    void SetIdleAction()
    {
        this.anima.Play("Idle");
    }

    void SetWalkAction()
    {
        this.anima.Play("Walk");
    }

    void SetRunAction()
    {
        this.anima.Play("Run");
    }

    void SetAttack1Action()
    {
        canAttack = false;
        this.anima.Play("Attack1");
        StartCoroutine("CoroutineEnableAttack");
    }

    void SetAttack2Action()
    {
        canAttack = false;
        this.anima.Play("Attack2");
        StartCoroutine("CoroutineEnableAttack");
    }

    void SetDeathAction()
    {
        this.anima.Play("Death");
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
            Destroy(this.GetComponent<CapsuleCollider>());

            SetDeathAction();

            this.audioSource.PlayOneShot(this.deathSound);

            Destroy(this.gameObject, 1.5f);
            return;
        }

        MoveToGoal();

        if (Vector3.Distance(this.transform.position, this.goal.transform.position) <= 1.7)
        {
            if (!canAttack)
            {
                return;
            }

            switch (Random.Range(0, 2))
            {
                case 0:
                    SetAttack1Action();
                    break;

                case 1:
                    SetAttack2Action();
                    break;

                default:
                    break;
            }
        }
        else
        {
            SetRunAction();
        }
    }
}
