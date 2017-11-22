using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    const float speed = 25, bulletLifetime = 3;

    void Start()
    {
        Destroy(gameObject, bulletLifetime);
    }

	void Update ()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
	}
}
