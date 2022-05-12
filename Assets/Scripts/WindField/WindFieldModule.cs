using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using XiheFramework;

public class WindFieldModule : GameModule {
    public bool update;
    private Dictionary<int, WindBlock> m_GridBlocks = new Dictionary<int, WindBlock>();

    public void AddWind(int x, int y, Vector3 vector) {
        var id = CantorPairUtility.CantorPair(x, y);
        if (m_GridBlocks.ContainsKey(id)) {
            m_GridBlocks[id].ApplyWindVector(vector);
        }
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