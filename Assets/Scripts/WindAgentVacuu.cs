using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class WindAgentVacuu : MonoBehaviour {
    public WindBlock currentBlock;
    public ParticleSystem vacuumEffect;
    public float windPeriod = 0.5f;
    public float windAmount = 1f;

    private WindFieldModule m_WindModule;


    public void StartVacuum() {
        StartCoroutine(nameof(VacuumCo));
        vacuumEffect.Play();
    }

    public void StopVacuum() {
        StopCoroutine(nameof(VacuumCo));
        vacuumEffect.Stop();
    }

    private void Start() {
        m_WindModule = GameManager.GetModule<WindFieldModule>();
        vacuumEffect = Instantiate(vacuumEffect, transform);

        StartVacuum();
    }

    private void Update() {
        currentBlock = m_WindModule.GetBlockByPosition(transform.position);
    }

    private void AddWindAround() {
        if (currentBlock == null) {
            return;
        }

        m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y + 1, (Vector3.back + Vector3.right).normalized * windAmount);
        m_WindModule.AddWind(currentBlock.x, currentBlock.y + 1, (Vector3.back).normalized * windAmount);
        m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y + 1, (Vector3.back + Vector3.left).normalized * windAmount);
        m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y, (Vector3.right).normalized * windAmount);
        m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y, (Vector3.left).normalized * windAmount);
        m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y - 1, (Vector3.forward + Vector3.right).normalized * windAmount);
        m_WindModule.AddWind(currentBlock.x, currentBlock.y - 1, (Vector3.forward).normalized * windAmount);
        m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y - 1, (Vector3.forward + Vector3.left).normalized * windAmount);
    }

    IEnumerator VacuumCo() {
        float timer = 0f;
        while (true) {
            AddWindAround();

            while (timer < windPeriod) {
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0f;
        }
    }
}