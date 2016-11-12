using UnityEngine;
using System.Collections;

public class GameDirector : MonoBehaviour {

    public GameDirector instance;
    public GameObject[] bossesOrder;
    public int currentBoss;
    
    void Start() {
        instance = this;
    }

    // Update is called once per frame
    void Update() {
        if (currentBoss < bossesOrder.Length && !bossesOrder[currentBoss])
            currentBoss++;
    }

    void InitialiseBoss() {

    }
}
