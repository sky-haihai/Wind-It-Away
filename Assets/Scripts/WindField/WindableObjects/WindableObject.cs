using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WindableObject : MonoBehaviour {
    public abstract void ApplyWind(Vector3 vector);
}
