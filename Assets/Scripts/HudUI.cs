using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XiheFramework;

public class HudUI : UIBehaviour {
    //前景图
    public Image frontImage;

    public GameObject winResult;
    public GameObject loseResult;

    public Text timerText;
    public Text ultText;

    //放大的大小
    private float bigger = 0.25f;

    //累计多少出发放大动画
    private float biggerLimit = 0.2f;

    private float lastPercent = 0f;

    private Transform panelTransform;

    //加垃圾（百分比）
    // public void Add(float percent) {
    //     Debug.Log("Add");
    //     frontImage.fillAmount += percent;
    //     if (frontImage.fillAmount - lastPercent >= biggerLimit) {
    //         lastPercent = frontImage.fillAmount;
    //         StartCoroutine(BiggerEffect());
    //     }
    // }

    //放大动画
    IEnumerator BiggerEffect() {
        Debug.Log("开始");
        panelTransform.localScale = new Vector3(panelTransform.localScale.x + bigger, panelTransform.localScale.y + bigger,
            panelTransform.localScale.z);
        yield return new WaitForSeconds(0.3f);
        panelTransform.localScale = new Vector3(panelTransform.localScale.x - bigger, panelTransform.localScale.y - bigger,
            panelTransform.localScale.z);
        Debug.Log("结束");
    }

    //清空垃圾
    public void Clear() {
        frontImage.fillAmount = 0f;
        lastPercent = 0f;
    }


    // Start is called before the first frame update
    public override void Start() {
        base.Start();
        panelTransform = GetComponent<Transform>();

        Game.Event.Subscribe("OnLevelClear", OnLevelClear);
        Game.Event.Subscribe("OnLevelLost", OnLevelLost);
        // Game.Event.Subscribe("OnPointGet", OnPointGet);
    }

    private void OnPointGet(object sender, object e) {
        var ult = Game.Blackboard.GetData<float>("UltCharge");
        ultText.text = "ULT: " + (ult).ToString("0.0") + " %";
    }

    private void OnLevelLost(object sender, object e) {
        loseResult.SetActive(true);
    }

    private void OnLevelClear(object sender, object e) {
        winResult.SetActive(true);
    }

    // Update is called once per frame
    void Update() {
        frontImage.fillAmount = (float) Game.Blackboard.GetData<int>("Point") / Game.Blackboard.GetData<int>("ClearPoint");

        UpdateTimerText();
        
        var ult = Game.Blackboard.GetData<float>("UltCharge");
        if (ult>= 100f) {
            ultText.text = "ULT: " + "READY";
        }
        else {
            ultText.text = "ULT: " + (ult).ToString("0") + " %";
        }
        
    }

    private void UpdateTimerText() {
        float totalSeconds = Game.Blackboard.GetData<float>("LevelTimer");
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
        timerText.text = time.ToString("hh':'mm':'ss");
    }
}