using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class TBInputModule : GameModule {
    public bool ReceivingInput => m_Timer <= 0f;

    private float m_Timer = 0f;

    public void AddDelay() {
        m_Timer += Time.deltaTime;
    }

    public override void Update() {
        m_Timer -= Time.deltaTime;
    }

    public override void ShutDown(ShutDownType shutDownType) {
    }
}