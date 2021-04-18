using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDisplay : MonoBehaviour {

    [Header("Backgrounds")]
    [SerializeField] GameObject[] backgrounds;

    [Header("Buffs")]
    [SerializeField] GameObject damageBuff;
    [SerializeField] GameObject pointBuff;
    [SerializeField] GameObject shieldBuff;
    [SerializeField] GameObject zapBuff;

    List<RectTransform> activeBuffRts = new List<RectTransform>();
    RectTransform damageBuffRt;
    RectTransform pointBuffRt;
    RectTransform shieldBuffRt;
    RectTransform zapBuffRt;

    Player player;

    // Start is called before the first frame update
    private void Start() {
        Setup();
    }

    // Update is called once per frame
    private void Update() {
        SetActiveBuffs();
    }

    private void Setup() {
        player = FindObjectOfType<Player>();
        damageBuffRt = damageBuff.GetComponentInChildren<RectTransform>();
        pointBuffRt = pointBuff.GetComponentInChildren<RectTransform>();
        shieldBuffRt = shieldBuff.GetComponentInChildren<RectTransform>();
        zapBuffRt = zapBuff.GetComponentInChildren<RectTransform>();
    }

    private void SetActiveBuffs() {
        if (player.PuDamage && !damageBuff.activeInHierarchy) {
            SetAndAddBuff(damageBuff, damageBuffRt, true);
        } else if (!player.PuDamage && damageBuff.activeInHierarchy) {
            SetAndAddBuff(damageBuff, damageBuffRt, false);
        }
        if (player.PuPoints && !pointBuff.activeInHierarchy) {
            SetAndAddBuff(pointBuff, pointBuffRt, true);
        } else if (!player.PuPoints && pointBuff.activeInHierarchy) {
            SetAndAddBuff(pointBuff, pointBuffRt, false);
        }
        if (player.PuShield && !shieldBuff.activeInHierarchy) {
            SetAndAddBuff(shieldBuff, shieldBuffRt, true);
        } else if (!player.PuShield && shieldBuff.activeInHierarchy) {
            SetAndAddBuff(shieldBuff, shieldBuffRt, false);
        }
        if (player.PuZap && !zapBuff.activeInHierarchy) {
            SetAndAddBuff(zapBuff, zapBuffRt, true);
        } else if (!player.PuZap && zapBuff.activeInHierarchy) {
            SetAndAddBuff(zapBuff, zapBuffRt, false);
        }
    }

    private void SetAndAddBuff(GameObject go, RectTransform rt, bool active) {
        go.SetActive(active);
        if (active) {
            activeBuffRts.Add(rt);
        } else {
            activeBuffRts.Remove(rt);
        }
        ShowBuffs();
    }

    private void ShowBuffs() {
        float yPos = -39;
        Vector2 vector = new Vector2(0, yPos);
        for (int i = 0; i < activeBuffRts.Count; i++) {
            vector = new Vector2(0, yPos);
            activeBuffRts[i].anchoredPosition = vector;
            yPos -= 78;
        }
        for (int i = 0; i < 4; i++) {
            if (i == activeBuffRts.Count - 1) {
                backgrounds[i].SetActive(true);
            } else {
                backgrounds[i].SetActive(false);
            }
        }
    }
}
