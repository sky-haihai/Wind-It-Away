using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class WindBlock : MonoBehaviour {
    public float size = 1f;

    public float attenuation = 60f; //decay amount per second

    public int x;
    public int y;

    public Vector3 windVector;

    private void Awake() {
    }

    private void Start() {
        x = (int) (transform.position.x / size);
        y = (int) (transform.position.z / size);
        GameManager.GetModule<WindFieldModule>().RegisterBlock(x, y, this);
    }

    public void ApplyWindVector(Vector3 vector) {
        windVector += vector;
    }

    public Vector3 GetRandomWindVector() {
        var ran = UnityEngine.Random.Range(0.5f, 1f);
        return windVector * ran;
    }

    public void UpdateAttenuation() {
        var mul = (windVector.magnitude - attenuation * Time.deltaTime);
        if (mul <= 0f) {
            mul = 0f;
        }

        windVector = windVector.normalized * mul;
    }

    private void OnDrawGizmos() {
        const float scale = 5f;
        var strength = windVector.magnitude / scale;
        strength = Mathf.Clamp01(strength);
        Color c = Color.HSVToRGB(0f, strength, 1f);
        Gizmos.color = c;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.05f, new Vector3(size, .1f, size));
    }
}