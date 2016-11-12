using UnityEngine;
using System.Collections;

public class NormalizeTest : MonoBehaviour {

    public Transform target;
    Vector3 normDist;
    Vector3 temp;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        normDist = Vector3.Normalize(target.position - transform.position);

        for (var i = -5; i < 5; i++) {
            //if (i == 0) {

            //}
            temp = new Vector3(0.1f, 0, 0.1f);
            temp *= i;
            Debug.DrawLine(transform.position, transform.position + ((normDist + temp) * 10), Color.red, 1);
            //Debug.Log(temp);
        }
    }
}
