using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class TBInputModule : GameModule {
    public bool IsReceivingInput => m_IsReceivingInput;

    [SerializeField]
    private bool m_IsReceivingInput;

    [SerializeField]
    private float m_Timer = 0f;

    public void AddDelay(float delay) {
        m_Timer += delay;
    }

    public void HardReset() {
        Debug.Log("HardReset");
        m_Timer = 0;
    }

    public override void Update() {
        m_IsReceivingInput = m_Timer <= 0;

        if (m_Timer > 0) {
            m_Timer -= Time.deltaTime;
        }
    }

    public override void ShutDown(ShutDownType shutDownType) {
    }
}