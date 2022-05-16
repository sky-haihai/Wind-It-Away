using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class Cage : DeadZone {
    public LookDirection forward;

    public Animator animator;
    private bool m_FirstFramed = false;

    protected override void Start() {
        base.Start();

        Game.Event.Subscribe("OnCatoLocked", OnCatoLock);
    }

    private void OnCatoLock(object sender, object e) {
        StartCoroutine(nameof(LockCo), 0.5f);
    }

    IEnumerator LockCo(float delay) {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger("Lock");
    }

    void Update() {
        if (!m_FirstFramed) {
            var wind = GameManager.GetModule<WindFieldModule>();
            var block = wind.GetBlockByPosition(transform.position);

            wind.SetWalkable(block.x, block.y, false);
            switch (forward) {
                case LookDirection.Forward:
                    wind.SetWalkable(block.x, block.y - 1, false);
                    break;
                case LookDirection.Backward:
                    wind.SetWalkable(block.x, block.y + 1, false);

                    break;
                case LookDirection.Left:
                    wind.SetWalkable(block.x + 1, block.y, false);

                    break;
                case LookDirection.Right:
                    wind.SetWalkable(block.x - 1, block.y, false);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            m_FirstFramed = true;
        }
    }
}