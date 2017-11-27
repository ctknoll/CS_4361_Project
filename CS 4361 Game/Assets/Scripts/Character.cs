using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IDamagable
{
    public event Action OnDeath;

    public bool isDead;
    public float startingHealth, health, armor, damage;
    public string type;

    public GameObject gun, particles;
    public Transform grip, muzzle;

    protected virtual void Start()
    {
        isDead = false;
        health = startingHealth;
    }

    /// <summary>
    /// Calculates the amount of health the player loses after taking into account the 
    /// player's armor.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        health -= damage * (1 - armor); // reduce health after taking into account the character's armor

        if (health <= 0 && !isDead)
            Death();
    }

    /// <summary>
    /// Detaches the gun from the player and adds a rigidbody to it so the gun drops to the ground
    /// after it is detached from the player. Spawn the DeathEffect to simulate blood.
    /// </summary>
    void Death()
    {
        isDead = true;
        // spawn the particle system at the character's position
        Destroy(Instantiate(particles, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0))), 2);

        if (gun != null)
        {
            gun.AddComponent<Rigidbody>();
            gun.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 45));
            grip.GetComponent<BoxCollider>().isTrigger = false;

            // detach the gun to have it drop to the ground
            transform.DetachChildren();
        }

        if (type == "Enemy")
        {
            OnDeath();
        }

        // display the gameover UI here, or however...
        Destroy(gameObject, 0.5f);
    }
}
