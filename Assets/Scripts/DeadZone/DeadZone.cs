using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class DeadZone : MonoBehaviour {
    public DeadZoneType type;
    public float minX;
    public float minZ;
    public float maxX;
    public float maxZ;

    protected virtual void Start() {
        GameManager.GetModule<WindFieldModule>().RegisterDeadZone(this);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(new Vector3((minX + maxX) / 2f, transform.position.y, (minZ + maxZ) / 2f), new Vector3(maxX - minX, 10f, maxZ - minZ));
    }
}

public enum DeadZoneType {
    Cato,
    Dust
}