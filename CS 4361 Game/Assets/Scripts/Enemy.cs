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
    float gravity = 12, verticalVelocity;

    protected override void Start()
    {
        type = "Enemy";
        damage = 5 + (Time.time / 2) > 75 ? 75 : 5 + (Time.time / 2);
        startingHealth = 60F + (Time.time * 1.5F) > 400F ? 400F : 60F + (Time.time * 1.5F);
        inRange = false;
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.speed = 1.5f;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        base.Start();
    }

    /// <summary>
    /// Basic movement logic for the enemy
    /// </summary>
    /// <returns></returns>
    void Update()
    {
        Debug.Log(health / startingHealth);
        GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.green, health / startingHealth);
        float colliderRadius = GetComponent<CapsuleCollider>().radius / 1.5F;
        Vector3 direction = (player.transform.position - transform.position).normalized;

        if (!player.isDead && !isDead)
        {
            if (navMesh.enabled)
            {
                navMesh.SetDestination(player.transform.position - direction * (colliderRadius));
            }

            if (inRange && Time.time > timeToNextAttack)
            {
                player.TakeDamage(damage);
                timeToNextAttack = Time.time + 1;
            }
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

    void Gravity()
    {
        verticalVelocity -= gravity * Time.deltaTime;
        if (verticalVelocity < -gravity * .75F) verticalVelocity = -gravity * .75F;
        Vector3 moveVector = new Vector3(0, verticalVelocity, 0);
        transform.position += moveVector * Time.deltaTime;
    }
}
