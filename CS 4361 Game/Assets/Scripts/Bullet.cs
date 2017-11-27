using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    const float speed = 25, bulletLifetime = 3;
    public float bulletDamage = 30;
    public LayerMask mask;

    void Start()
    {
        Destroy(gameObject, 5);
    }

	void Update ()
    {
        float distance = Time.deltaTime * speed;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        transform.Translate(Vector3.forward * distance);
        if (Physics.Raycast(ray, out hit, distance, mask, QueryTriggerInteraction.Collide))
        {
            IDamagable dmgObj;
            if ((dmgObj = hit.collider.GetComponent<IDamagable>()) != null)
                dmgObj.TakeDamage(bulletDamage);

            Destroy(gameObject);
        }   
	}
}
