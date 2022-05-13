using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class WindableDust : WindableObject {
    public WindBlock owner;

    public float gridSize;
    public float fixWindAmount; //the amount of wind to fix the position so this particle is within the area of its owner block

    private Rigidbody m_Rigidbody;

    private void Start() {
        m_Rigidbody = GetComponent<Rigidbody>();
        Game.Event.Subscribe("OnWindAdded", OnWindAdded);
    }

    private void Update() {
        var pos = transform.position;
        owner = GameManager.GetModule<WindFieldModule>().GetBlockByPosition(pos);

        var area = new Vector4(owner.x * gridSize - 0.5f * gridSize, owner.x * gridSize + 0.5f * gridSize, owner.y * gridSize - 0.5f * gridSize,
            owner.y * gridSize + 0.5f * gridSize);
        if (area.Contain(new Vector2(transform.position.x, transform.position.z))) {
            return;
        }

        var delta = new Vector2(pos.x, pos.z).GetNearestPointFromOutside(area).ToVector3() - pos;
        ApplyWind(delta.normalized * fixWindAmount);
    }

    private void OnWindAdded(object sender, object e) {
        OnWindAdded();
    }

    private void OnWindAdded() {
        ApplyWind(owner.GetRandomWindVector());
    }

    public override void ApplyWind(Vector3 vector) {
        m_Rigidbody.velocity += vector;
    }
}