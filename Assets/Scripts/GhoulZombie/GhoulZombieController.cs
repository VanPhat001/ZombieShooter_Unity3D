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

    void MoveToGoal()
    {
        this.agent.destination = this.goal.transform.position;
    }

    void SetRunAction()
    {
        this.anima.Play("Run");
    }


    void SetAttack1Action()
    {
        this.anima.Play("Attack1");
    }


    void SetAttack2Action()
    {
        this.anima.Play("Attack2");
    }


    void SetDeathAction()
    {
        this.anima.Play("Death");
    }

    IEnumerator CoroutineActiveAttack()
    {
        this.canAttack = false;
        yield return new WaitForSeconds(2f);
        this.canAttack = true;
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

            SetDeathAction();

            this.audioSource.PlayOneShot(this.deathSound);

            Destroy(this.gameObject, 1.02f);
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
