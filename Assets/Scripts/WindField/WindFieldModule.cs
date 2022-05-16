using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using XiheFramework;

public class WindFieldModule : GameModule {
    public bool update;

    private Dictionary<int, WindBlock> m_GridBlocks = new Dictionary<int, WindBlock>();

    private Dictionary<int, bool> m_Walkables = new Dictionary<int, bool>();

    private List<DeadZone> m_DeadZones = new List<DeadZone>();

    public void RegisterDeadZone(DeadZone zone) {
        m_DeadZones.Add(zone);
    }

    public bool WithinDeadZone(Vector2 position, DeadZoneType type, out Vector3 zonePos, out Quaternion zoneRot) {
        foreach (var zone in m_DeadZones) {
            if (zone.type != type) {
                continue;
            }

            if (new Vector4(zone.minX, zone.maxX, zone.minZ, zone.maxZ).Contain(position)) {
                zonePos = zone.transform.position;
                zoneRot = zone.transform.rotation;
                return true;
            }
        }

        zonePos = Vector3.zero;
        zoneRot = Quaternion.identity;
        return false;
    }

    public void AddWind(int x, int y, Vector3 vector, bool effectCato = true) {
        Debug.Log("who the fuck");
        var id = CantorPairUtility.CantorPair(x, y);
        if (m_GridBlocks.ContainsKey(id)) {
            m_GridBlocks[id].ApplyWindVector(vector);
        }

        Game.Event.Invoke("OnWindAdded", null, effectCato);
    }

    public bool RegisterWalkable(int originX, int originY, int targetX, int targetY, float leaveDelay) {
        if (!IsWalkable(targetX, targetY)) {
            return false;
        }

        SetWalkable(targetX, targetY, false);
        StartCoroutine(UnregisterWalkable(originX, originY, leaveDelay));
        return true;
    }

    IEnumerator UnregisterWalkable(int x, int y, float leaveDelay) {
        yield return new WaitForSeconds(leaveDelay);
        SetWalkable(x, y, true);
    }


    public bool IsWalkable(int x, int y) {
        if (!HasBlock(x, y)) {
            return false;
        }

        var id = CantorPairUtility.CantorPair(x, y);
        if (m_Walkables.ContainsKey(id)) {
            return m_Walkables[id];
        }
        else {
            Debug.LogError(x + " " + y);
        }

        return false;
    }

    public Vector3 GetWindVector(int x, int y) {
        var id = CantorPairUtility.CantorPair(x, y);
        return m_GridBlocks[id].windVector;
    }

    public void RegisterBlock(int x, int y, WindBlock windBlock) {
        var id = CantorPairUtility.CantorPair(x, y);
        if (m_GridBlocks.ContainsKey(id)) {
            m_GridBlocks[id] = windBlock;
        }
        else {
            m_GridBlocks.Add(id, windBlock);
        }

        SetWalkable(x, y, true);
    }

    public bool HasBlock(int x, int y) {
        if (x < 0 || y < 0) {
            return false;
        }

        var id = CantorPairUtility.CantorPair(x, y);
        return m_GridBlocks.ContainsKey(id);
    }

    public WindBlock GetBlockByPosition(Vector3 position) {
        int result = -1;

        var shortest = float.MaxValue;
        foreach (var key in m_GridBlocks.Keys) {
            float dst = Vector3.Distance(m_GridBlocks[key].transform.position, position);
            if (dst < shortest) {
                shortest = dst;
                result = key;
            }
        }

        if (result == -1) {
            return null;
        }

        return m_GridBlocks[result];
    }

    public void SetWalkable(int x, int y, bool value) {
        var id = CantorPairUtility.CantorPair(x, y);
        if (!m_GridBlocks.ContainsKey(id)) {
            return;
        }

        if (m_Walkables.ContainsKey(id)) {
            m_Walkables[id] = value;
        }
        else {
            m_Walkables.Add(id, value);
        }
    }

    public override void Update() {
        if (!update) {
            return;
        }


        foreach (var value in m_GridBlocks.Values) {
            value.UpdateAttenuation();
        }
    }

    public override void ShutDown(ShutDownType shutDownType) {
        m_GridBlocks.Clear();
    }
}