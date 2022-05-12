using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class WindableDust : WindableObject {
    public WindBlock owner;

    private Rigidbody m_Rigidbody;

    private void Start() {
        m_Rigidbody = GetComponent<Rigidbody>();
        Game.Event.Subscribe("OnWindAdded", OnWindAdded);
    }

    private void OnWindAdded(object sender, object e) {
        OnWindAdded();
    }

    private void OnWindAdded() {
        owner = GameManager.GetModule<WindFieldModule>().GetBlockByPosition(transform.position);
        ApplyWind(owner.GetRandomWindVector());
    }

    public override void ApplyWind(Vector3 vector) {
        m_Rigidbody.velocity += vector;
    }
}