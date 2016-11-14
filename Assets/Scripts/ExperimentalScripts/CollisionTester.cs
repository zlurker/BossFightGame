using UnityEngine;
using System.Collections;

public class CollisionTester : MonoBehaviour {

	void OnCollisionStay(Collision collision) {
        Debug.Log(collision.transform.tag);
    }
}
