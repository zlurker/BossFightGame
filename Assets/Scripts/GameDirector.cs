using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour {

    public GameDirector instance;
    public UnitBase[] bossesOrder;
    public int currentBoss;

    public Unit boss;
    public GameObject player;

    public Text bossName;
    public Text bossHP;

    float initialBossHealth;
    void Start() {
        instance = this;

        if (boss) {
            boss.target = player.transform;
            initialBossHealth = boss.health;
            bossName.text = boss.name;
        }
    }

    void Update() {
        if (currentBoss < bossesOrder.Length && !boss) {
            boss = (Unit)Instantiate(bossesOrder[currentBoss], new Vector3(0, 2.5f, 0), Quaternion.identity);
            boss.target = player.transform;
            initialBossHealth = boss.health;
            bossName.text = boss.name;
            currentBoss++;
        }

        if (boss)
            bossHP.text = boss.health.ToString();

        //if (!player) {
        //lose
        //}

    }
}
