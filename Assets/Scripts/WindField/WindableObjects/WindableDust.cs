using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class WindableDust : MonoBehaviour, IWindable {
    public WindBlock owner;

    public float gridSize;
    public float boundaryWindAmount; //the amount of wind to fix the position so this particle is within the area of its owner block

    // private Rigidbody m_Rigidbody;
    private Vector3 m_Velocity;

    private WindFieldModule m_Wind;
    private float m_Timer;

    private void Start() {
        // m_Rigidbody = GetComponent<Rigidbody>();
        m_Wind = GameManager.GetModule<WindFieldModule>();
        Game.Event.Subscribe("OnWindAdded", OnWindAdded);
    }

    private void Update() {
        UpdateVelocity();
        UpdateOwner();
        KillParticle();


        BoundaryCheck();
    }

    private void UpdateOwner() {
        if (m_Timer <= 0) {
            owner = m_Wind.GetBlockByPosition(transform.position);
            m_Timer = 0.5f;
        }

        m_Timer -= Time.deltaTime;
    }

    private void BoundaryCheck() {
        var pos = transform.position;

        //coord
        // var targetX = (int) (transform.position.x / gridSize);
        // var targetY = (int) (transform.position.y / gridSize);
        // //has block
        // if (m_Wind.HasBlock(targetX, targetY)) {
        //     return;
        // }
        if (Vector3.Distance(owner.transform.position, transform.position) < 0.8f) {
            return;
        }

        var area = new Vector4(owner.x * gridSize - 0.5f * gridSize, owner.x * gridSize + 0.5f * gridSize, owner.y * gridSize - 0.5f * gridSize,
            owner.y * gridSize + 0.5f * gridSize);
        if (area.Contain(new Vector2(transform.position.x, transform.position.z))) {
            return;
        }

        var delta = new Vector2(pos.x, pos.z).GetNearestPointFromOutside(area).ToVector3() - pos;
        ApplyWind(delta * boundaryWindAmount);
    }

    private void KillParticle() {
        if (GameManager.GetModule<WindFieldModule>().WithinDeadZone(transform.position.ToVector2(V3ToV2Type.XZ), DeadZoneType.Dust, out Vector3 pos,
            out Quaternion rot)) {
            Destroy(gameObject);
            Game.Event.Invoke("OnPointGet", gameObject.name, null);
        }
    }

    private void OnWindAdded(object sender, object e) {
        OnWindAdded();
    }

    private void OnWindAdded() {
        if (!owner) {
            return;
        }

        ApplyWind(owner.GetRandomWindVector());
    }

    public void ApplyWind(Vector3 vector) {
        // m_Rigidbody.velocity += vector;
        AddVelocity(vector);
    }

    private void AddVelocity(Vector3 vector) {
        m_Velocity += vector;
    }

    private void UpdateVelocity() {
        var gravity = new Vector3(0, 0.04f - transform.position.y, 0f);
        transform.position = transform.position + (m_Velocity + gravity * 9.81f) * (0.1f * Time.deltaTime);
        m_Velocity = Vector3.Lerp(m_Velocity, Vector3.zero, 0.1f);
    }
}