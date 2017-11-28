using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    const float speed = 25, bulletLifetime = 3;
    public float bulletDamage = 30;

    void Start()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, speed * 5))
        {
            Debug.Log("hit");
            IDamagable dmgObj;
            if ((dmgObj = hit.collider.GetComponent<IDamagable>()) != null)
                dmgObj.TakeDamage(bulletDamage);
            float time = hit.distance / speed;
            Destroy(gameObject, time);
        }
        else
            Destroy(gameObject, 5);
    }

	void Update ()
    {
        float distance = Time.deltaTime * speed;
        Ray ray = new Ray(transform.position, transform.forward);
        transform.Translate(Vector3.forward * distance);
	}
}
