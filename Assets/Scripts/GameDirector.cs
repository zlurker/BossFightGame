using UnityEngine;
using System.Collections;

public class GameDirector : MonoBehaviour {

    public GameDirector instance;
    public GameObject[] bossesOrder;
    public int currentBoss;

    public GameObject boss;
    void Start() {
        instance = this;
    }

    void Update() {
        if (currentBoss < bossesOrder.Length && !boss) {
            Instantiate(bossesOrder[currentBoss], Vector3.zero, Quaternion.identity);
            currentBoss++;
        }
    }
}
