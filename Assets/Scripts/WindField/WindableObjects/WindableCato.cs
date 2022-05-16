using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class WindableCato : MonoBehaviour, IWindable {
    public WindBlock owner;

    public float gridSize;
    public float boundaryWindAmount;
    public float windTweak = 1.0f;

    private Vector3 m_Velocity;

    private WindBlock m_LastOwner;

    private bool m_Locked;

    private void Start() {
        Game.Event.Subscribe("OnWindAdded", OnWindAdded);
    }

    private void Update() {
        UpdateOwner();
        UpdateVelocity();
        BoundaryCheck();
        LockCato();

        m_LastOwner = owner;
    }

    private void LockCato() {
        if (m_Locked) {
            return;
        }

        if (GameManager.GetModule<WindFieldModule>().WithinDeadZone(transform.position.ToVector2(V3ToV2Type.XZ), DeadZoneType.Cato, out Vector3 pos,
            out Quaternion rot)) {
            Game.Event.Invoke("OnCatoLocked", pos, rot);
            m_Velocity = Vector3.zero;
            m_Locked = true;
            Game.Audio.Play(AudioChannelTypes.BackGroundSound, "SFX-RoboCat-4", 1f, false);
        }
    }

    private void UpdateOwner() {
        var wind = GameManager.GetModule<WindFieldModule>();
        owner = wind.GetBlockByPosition(transform.position);

        if (m_LastOwner == null) {
            return;
        }

        if (owner != m_LastOwner) {
            wind.SetWalkable(m_LastOwner.x, m_LastOwner.y, true);
        }
    }

    private void OnWindAdded(object sender, object e) {
        var ne = (bool) e;

        if (!owner) {
            return;
        }

        if (ne == false) {
            return;
        }

        var wind = owner.GetRandomWindVector() * windTweak;
        if (wind.magnitude < 0.3f) {
            return;
        }

        ApplyWind(wind);
        Game.Event.Invoke("OnCatoFlowed", this, null);
        StartCoroutine(nameof(CatoFlowCo));
    }

    IEnumerator CatoFlowCo() {
        yield return new WaitForSeconds(0.5f);
        Game.Event.Invoke("OnCatoLanded", this, null);
    }

    private void BoundaryCheck() {
        var pos = transform.position;

        var area = new Vector4(owner.x * gridSize - 0.5f * gridSize, owner.x * gridSize + 0.5f * gridSize, owner.y * gridSize - 0.5f * gridSize,
            owner.y * gridSize + 0.5f * gridSize);
        if (area.Contain(new Vector2(transform.position.x, transform.position.z))) {
            return;
        }

        var delta = new Vector2(pos.x, pos.z).GetNearestPointFromOutside(area).ToVector3() - pos;
        ApplyWind(delta.normalized * boundaryWindAmount);
    }


    private void UpdateVelocity() {
        var gravity = new Vector3(0, 0.04f - transform.position.y, 0f);
        transform.position = transform.position + (m_Velocity + gravity * 9.81f) * (0.1f * Time.deltaTime);
        m_Velocity = Vector3.Lerp(m_Velocity, Vector3.zero, 0.1f);
    }

    public void ApplyWind(Vector3 vector) {
        m_Velocity += vector;
    }
}