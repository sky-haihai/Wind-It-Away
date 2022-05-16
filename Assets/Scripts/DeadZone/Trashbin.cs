using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class Trashbin : DeadZone {
    public LookDirection direction;
    public Renderer render;
    private bool m_FirstFramed = false;

    protected override void Start() {
        base.Start();

        Game.Event.Subscribe("OnPointGet", OnPointGet);
    }

    private void OnPointGet(object sender, object e) {
        StartCoroutine(GetPointCo());
    }

    IEnumerator GetPointCo() {
        var timer = 1f;
        while (timer > 0f) {
            var value = -Mathf.Pow(timer * 2f - 1f, 2f) + 1f;
            value *= 4f;
            value += 1f;
            render.material.SetFloat("_ColorMultiplier", value);
            timer -= Time.deltaTime;
            yield return null;
        }

        render.material.SetFloat("_ColorMultiplier", 1f);
    }

    void Update() {
        if (!m_FirstFramed) {
            var wind = GameManager.GetModule<WindFieldModule>();
            var block = wind.GetBlockByPosition(transform.position);
            switch (direction) {
                case LookDirection.Forward:
                    wind.SetWalkable(block.x, block.y, false);
                    wind.SetWalkable(block.x + 1, block.y, false);

                    break;
                case LookDirection.Backward:
                    wind.SetWalkable(block.x, block.y, false);
                    wind.SetWalkable(block.x - 1, block.y, false);
                    break;
                case LookDirection.Left:
                    wind.SetWalkable(block.x, block.y, false);
                    wind.SetWalkable(block.x, block.y + 1, false);
                    break;
                case LookDirection.Right:
                    wind.SetWalkable(block.x, block.y, false);
                    wind.SetWalkable(block.x, block.y - 1, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            m_FirstFramed = true;
        }
    }
}