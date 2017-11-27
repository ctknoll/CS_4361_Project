using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Character
{
    bool inRange;
    float timeToNextAttack = 0;
    Player player;
    NavMeshAgent navMesh;

    protected override void Start()
    {
        base.Start();

        type = "Enemy";
        damage = 20;
        inRange = false;
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.speed = 1.5f;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        StartCoroutine(Pathing());
    }

    /// <summary>
    /// Basic movement logic for the enemy
    /// </summary>
    /// <returns></returns>
    IEnumerator Pathing()
    {
        float colliderRadius = GetComponent<CapsuleCollider>().radius;
        Vector3 direction = (player.transform.position - transform.position).normalized;

        while (!player.isDead && !isDead)
        {
            if (navMesh.enabled)
            {
                navMesh.SetDestination(player.transform.position - direction * (colliderRadius));
            }

            if (inRange && Time.time > timeToNextAttack)
            {
                player.TakeDamage(damage);
                timeToNextAttack = Time.time + 2;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            inRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            inRange = false;
    }
}
