using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    const int maxAmmo = 15; // max capacity of the magazine, should not be changed in game

    public Bullet bullet; // Bullet pre-fab
    public GameObject particles, magazine; // DeathEffect and Magazine pre-fabs

    bool isJumping = false, isReloading = false, isPlayerDead = false;
    float moveSpeed = 1, jumpForce = 125, reloadSpeed = 3, nextShot = 0, gravity = 12, jumpSpeed = 3, 
        verticalVelocity;
    GameObject gun;
    CharacterController controller;
    Rigidbody rb;
    Camera cam;
    Transform muzzle, grip;

    // player stat variables
    int ammo = 15;
    float health = 100, armor = 0.15f; // should the player have armor? 

    void Start ()
    {
        controller = GetComponent<CharacterController>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        gun = transform.GetChild(0).gameObject;
        muzzle = gun.transform.GetChild(2);
        grip = gun.transform.GetChild(0);
        //Invoke("PlayerDeath", 5); // testing purposes
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x, cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.z);
        JumpAndGravity();
        // Basic movement for the player
        // Only move if input from the axis are detected
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            Move();
        }

        // Single fire, could be auto as well if you guys want, but since the player is using a pistol 
        // this might be a good challenge for them (I hope).
        // (change conditional statement to Input.GetMouseButton(0) for auto fire)
        if (Input.GetMouseButtonDown(0) && ammo > 0 && !isReloading)
        {
            Shoot();  
        }

        // Reload the weapon if the amount of bullets in the magazine is less than maxAmmo.
        // Simulate reloading time by invoking the method after a time pre-determined by reloadSpeed
        if (Input.GetKeyDown(KeyCode.R) && ammo < maxAmmo && !isReloading)
        {
            isReloading = true;
            GameObject mag = Instantiate(magazine, grip.position, grip.rotation);
            Destroy(mag, 5); // despawn magazine after five seconds
            Invoke("ReloadWeapon", reloadSpeed);
            Debug.Log("Reloading...");
        }

        // I'll add jumping for the player here, but we can take it out if it seems out of place

        // Update where the player is looking at and camera position
        LookAtPoint();
    }

    /// <summary>
    /// Basic player movement
    /// </summary>
    void Move()
    {
        Vector3 moveInput, moveVelocity;
        moveInput = Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward;
        moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity * Time.fixedDeltaTime);
    }

    void JumpAndGravity()
    {
        verticalVelocity -= gravity * Time.deltaTime;
        if (verticalVelocity < -gravity * .75F) verticalVelocity = -gravity * .75F;
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            verticalVelocity = jumpSpeed;
        }
        Vector3 moveVector = new Vector3(0, verticalVelocity, 0);
        controller.Move(moveVector * Time.deltaTime);
    }

    public bool isGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            if (hit.distance - (GetComponent<CapsuleCollider>().height / 2) < .1F) return true;
        }
        return false;
    }

    /// <summary>
    /// This method spawns a bullet based on the rate of fire. Can only spawn bullet if it is within the 
    /// time constraints.
    /// Used for auto fire, but also works for single fire.
    /// </summary>
    void Shoot()
    {
        const float rateOfFire = 0.25f; // rateOfFire is time in seconds, used as time between shots
        if (Time.time > nextShot)
        {
            nextShot = Time.time + rateOfFire; 
            Bullet newBullet = Instantiate(bullet, muzzle.position, muzzle.rotation) as Bullet;
            ammo--;
            Debug.Log("Ammo: " + ammo + "/" + maxAmmo);
        }   
    }

    /// <summary>
    /// Simple reload mechanic for the gun
    /// </summary>
    void ReloadWeapon()
    {
        ammo = maxAmmo;
        isReloading = false;
        Debug.Log("Ammo: " + ammo + "/" + maxAmmo);
    }

    /// <summary>
    /// Have the player look at where the mouse cursor currently is on the map.
    /// Also updates the camera to follow the player around at a certain offset.
    /// </summary>
    void LookAtPoint()
    {
        float distance;
        Vector3 point, cameraOffset = new Vector3(.1f, transform.lossyScale.y, 0f);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (ground.Raycast(ray, out distance))
        {
            point = ray.GetPoint(distance);
            // draws a line from the camera to the ground to indicate the direction the player is looking towards
            // (this line doesn't appear in game)
            Debug.DrawLine(ray.origin, point, Color.blue);
            transform.LookAt(new Vector3(point.x, transform.position.y, point.z));
        }

        // updates the camera's position
    }

    /// <summary>
    /// Unity funciton to check if the player is colliding with any colliders.
    /// Simply check if the player is touching the ground, if so, give the 
    /// player the ability to jump.
    /// </summary>
    void OnCollisionStay()
    {
        isJumping = false;
    }

    /// <summary>
    /// Calculates the amount of health the player loses after taking into account the 
    /// player's armor.
    /// </summary>
    /// <param name="damage"></param>
    void PlayerTakeDamage(float damage)
    {
        // we can choose to add armor into the equation or not
        health -= damage * (1 - armor); // reduce health after taking into account player's armor

        if (health <= 0)
            PlayerDeath();
    }

    /// <summary>
    /// Detaches the gun from the player and adds a rigidbody to it so the gun drops to the ground
    /// after it is detached from the player. Spawn the DeathEffect to simulate blood.
    /// </summary>
    void PlayerDeath()
    {
        isPlayerDead = true;
        gun.AddComponent<Rigidbody>();
        gun.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 45));

        // spawn the particle system at the players position while having the rotation pointing upwards
        Instantiate(particles, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
        // detach the gun to have it drop to the ground
        transform.DetachChildren();
        // destroy the script so the mesh still stays on the map and then show the game over UI
        // this will only be done for the player, the enemy will have its mesh destroyed
        Destroy(this);
    }
}
