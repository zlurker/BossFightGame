using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ListExperimentScript : MonoBehaviour {

    public List<bool> tests;
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        for (var i=0; i < tests.Count; i++)
            if (tests[i])
                tests.Remove(tests[i]);
    }
}
