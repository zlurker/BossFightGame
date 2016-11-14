using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour {

    public GameDirector instance;
    public UnitBase[] bossesOrder;
    public int currentBoss;

    public Unit boss;
    public PlayerUnit player;

    public Text bossName;
    public Text bossHP;
    public float bossSpawnPeriod;

    float nextBossSpawnTime;
    float initialBossHealth;
    Vector3 initialPlayerPos;
    Vector3 initialPlayerRot;

    void Start() {
        instance = this;

        if (boss) {
            boss.target = player.transform;
            initialBossHealth = boss.health;
            bossName.text = boss.name;
        }

        initialPlayerPos = player.transform.position;
        initialPlayerRot = player.transform.eulerAngles;
    }

    void Update() {
        if (currentBoss < bossesOrder.Length && !boss) {
            boss = (Unit)Instantiate(bossesOrder[currentBoss], new Vector3(0, 2.5f, 0), Quaternion.identity);
            boss.target = player.transform;
            initialBossHealth = boss.health;
            bossName.text = boss.name;
            boss.enabled = false;
            nextBossSpawnTime = Time.time + bossSpawnPeriod;
            boss.gameObject.tag = "Untagged";

            player.transform.position = initialPlayerPos;
            player.transform.eulerAngles = initialPlayerRot;
            currentBoss++;
        }

        if (!boss.isActiveAndEnabled && nextBossSpawnTime < Time.time) {
            boss.enabled = true;
            boss.gameObject.tag = "Enemy";
        }

        if (boss)
            bossHP.text = boss.health.ToString();

        //if (!player) {
        //lose
        //}

    }
}
