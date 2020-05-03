using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotorSpin : MonoBehaviour
{
    private Rigidbody rigidbody;

    private enum rotateDirection
    {
        x = 'x',
        y = 'y',
        z = 'z'
    }
    [SerializeField]
    private rotateDirection rotatedirection;
    [SerializeField]
    [Range(0, 2)]
    private float speed;

    // Use this for initialization
    void Start ()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.maxAngularVelocity = 9999999999999999999;

        if (rotatedirection == rotateDirection.x)
        {
            rigidbody.angularVelocity = transform.InverseTransformDirection(new Vector3(10 * speed, 0, 0));
        }
        if (rotatedirection == rotateDirection.y)
        {
            rigidbody.angularVelocity = transform.InverseTransformDirection(new Vector3(0, 10 * speed, 0));
        }
        if (rotatedirection == rotateDirection.z)
        {
            rigidbody.angularVelocity = transform.InverseTransformDirection(new Vector3(0, 0, 10 * speed));
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
    }
}
