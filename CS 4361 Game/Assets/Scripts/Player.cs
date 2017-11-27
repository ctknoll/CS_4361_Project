using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : Character
{
    enum AimStatus { AIM, HIP };
    const int maxAmmo = 15; // max capacity of the magazine, should not be changed in game
    const float rateOfFire = 0.25f; // rateOfFire is time in seconds, used as time between shots

    public Bullet bullet; // Bullet pre-fab
    public GameObject magazine; // DeathEffect and Magazine pre-fabs

    bool isReloading = false;
    float moveSpeed = 2, nextShot = 0, gravity = 12, jumpSpeed = 3, verticalVelocity;
    CharacterController controller;
    Camera cam;
    Rigidbody rb;
    AimStatus status;
    Animation anim;
    AnimationState animState;

    // player stat variables
    public int enemyCount;
    int ammo = 15;

    protected override void Start ()
    {
        base.Start();

        type = "Player";
        enemyCount = 0;
        controller = GetComponent<CharacterController>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        gun = transform.GetChild(0).gameObject;
        muzzle = gun.transform.GetChild(2);
        grip = gun.transform.GetChild(0);
        status = AimStatus.HIP;
        anim = gun.GetComponent<Animation>();
        animState = anim["ADS"];
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

        // plays an animation so that the player aims down the sight when they are holding down the
        // right mouse button
        if (Input.GetMouseButtonDown(1) && status != AimStatus.AIM && !anim.IsPlaying("ADS"))
        {
            status = AimStatus.AIM;
            animState.speed = 1;
            anim.Play("ADS");
        }
        else if (Input.GetMouseButtonUp(1) && status == AimStatus.AIM && !anim.IsPlaying("ADS"))
        {
            status = AimStatus.HIP;
            animState.speed = -1;
            animState.time = animState.length;
            anim.Play("ADS");
        }

        // Single fire, could be auto as well if you guys want, but since the player is using a pistol 
        // this might be a good challenge for them (I hope).
        // (change conditional statement to Input.GetMouseButton(0) for auto fire)
        if (Input.GetMouseButtonDown(0) && ammo > 0 && !isReloading)
        {
            Shoot();  
        }

        // Reload the weapon if the amount of bullets in the magazine is less than maxAmmo.
        // Simulate reloading time by invoking the method after a pre-determined time.
        if (Input.GetKeyDown(KeyCode.R) && ammo < maxAmmo && !isReloading)
        {
            isReloading = true;
            Destroy(Instantiate(magazine, grip.position, grip.rotation), 5); // despawn magazine after five seconds
            Invoke("ReloadWeapon", 3);
            Debug.Log("Reloading...");
        }

        // Update where the player is looking at
        LookAtPoint();

        // kill the player if they fell off of the map
        if (transform.position.y < -10)
            TakeDamage(1000);
    }

    /// <summary>
    /// Basic player movement
    /// </summary>
    void Move()
    {
        // reduce movement speed of the player if they are aiming down sight
        if (status == AimStatus.AIM)
            moveSpeed = 1;
        else
            moveSpeed = 2;

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
        if (Time.time > nextShot)
        {
            nextShot = Time.time + rateOfFire; 
            Instantiate(bullet, muzzle.position, muzzle.rotation);
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
    /// </summary>
    void LookAtPoint()
    {
        float distance;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (ground.Raycast(ray, out distance))
        {
            Vector3 point = ray.GetPoint(distance);
            transform.LookAt(new Vector3(point.x, transform.position.y, point.z));
        }
    }
}
