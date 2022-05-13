using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class WindAgentRobo : MonoBehaviour {
    public WindBlock currentBlock;
    public Animator animator;
    public float windAmount = 5f;
    public float fanDelay = 0.5f;
    public float blasterDelay = 3f;

    private WindFieldModule m_WindModule;
    private TBInputModule m_TbInputModule;

    private void Start() {
        m_WindModule = GameManager.GetModule<WindFieldModule>();
        m_TbInputModule = GameManager.GetModule<TBInputModule>();
    }

    private void Update() {
        AddWindByBlockChange();

        AddWindByFan();
        AddWindByBlaster();
    }

    private void AddWindByFan() {
        if (!Game.Input.GetKey("Attack")) return;
        if (!m_TbInputModule.IsReceivingInput) return;

        m_TbInputModule.AddDelay(fanDelay);

        var direction = Game.Blackboard.GetData<LookDirection>("Robo.LookDirection");
        Debug.Log(direction.ToString());
        switch (direction) {
            case LookDirection.Forward:
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 1, Vector3.forward * windAmount);
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y + 1, (Vector3.forward + Vector3.left).normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y + 1, (Vector3.forward + Vector3.right).normalized * windAmount);
                break;
            case LookDirection.Backward:
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 1, Vector3.back * windAmount);
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y - 1, (Vector3.back + Vector3.left).normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y - 1, (Vector3.back + Vector3.right).normalized * windAmount);
                break;
            case LookDirection.Left:
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y, Vector3.left * windAmount);
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y - 1, (Vector3.left + Vector3.back).normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y + 1, (Vector3.left + Vector3.forward).normalized * windAmount);
                break;
            case LookDirection.Right:
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y, Vector3.right * windAmount);
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y - 1, (Vector3.right + Vector3.back).normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y + 1, (Vector3.right + Vector3.forward).normalized * windAmount);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        ApplyFanAnimation();
    }

    private void ApplyFanAnimation() {
        StartCoroutine(nameof(FanAnimationCo));
    }

    IEnumerator FanAnimationCo() {
        animator.SetTrigger("Fan");
        var particle = Instantiate(Game.Blackboard.GetData<ParticleSystem>("FanEffect"), transform);
        particle.Play();
        Destroy(particle.gameObject, 0.5f);
        yield return new WaitForSeconds(0.4f);
        animator.SetTrigger("Fan_End");
    }

    void AddWindByBlaster() {
        if (Game.Blackboard.GetData<float>("UltimateCharge") < 1f) return;
        if (!Game.Input.GetKey("Ultimate")) return;
        if (!m_TbInputModule.IsReceivingInput) return;

        m_TbInputModule.AddDelay(blasterDelay);

        ApplyBlasterAnimation();
    }

    void AddDirectionalBlaster() {
        var direction = Game.Blackboard.GetData<LookDirection>("Robo.LookDirection");
        Debug.Log(direction.ToString());
        switch (direction) {
            case LookDirection.Forward:
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 1, Vector3.forward * (3f * windAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 2, Vector3.forward * (3f * windAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 3, Vector3.forward * (3f * windAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 4, Vector3.forward * (2.7f * windAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 5, Vector3.forward * (1.2f * windAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 6, Vector3.forward * (0.3f * windAmount));
                break;
            case LookDirection.Backward:
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 1, Vector3.back * (3f * windAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 2, Vector3.back * (3f * windAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 3, Vector3.back * (3f * windAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 4, Vector3.back * (2.7f * windAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 5, Vector3.back * (1.2f * windAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 6, Vector3.back * (0.3f * windAmount));
                break;
            case LookDirection.Left:
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y, Vector3.left * (3f * windAmount));
                m_WindModule.AddWind(currentBlock.x - 2, currentBlock.y, Vector3.left * (3f * windAmount));
                m_WindModule.AddWind(currentBlock.x - 3, currentBlock.y, Vector3.left * (3f * windAmount));
                m_WindModule.AddWind(currentBlock.x - 4, currentBlock.y, Vector3.left * (2.7f * windAmount));
                m_WindModule.AddWind(currentBlock.x - 5, currentBlock.y, Vector3.left * (1.2f * windAmount));
                m_WindModule.AddWind(currentBlock.x - 6, currentBlock.y, Vector3.left * (0.3f * windAmount));
                break;
            case LookDirection.Right:
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y, Vector3.right * (3f * windAmount));
                m_WindModule.AddWind(currentBlock.x + 2, currentBlock.y, Vector3.right * (3f * windAmount));
                m_WindModule.AddWind(currentBlock.x + 3, currentBlock.y, Vector3.right * (3f * windAmount));
                m_WindModule.AddWind(currentBlock.x + 4, currentBlock.y, Vector3.right * (2.7f * windAmount));
                m_WindModule.AddWind(currentBlock.x + 5, currentBlock.y, Vector3.right * (1.2f * windAmount));
                m_WindModule.AddWind(currentBlock.x + 6, currentBlock.y, Vector3.right * (0.3f * windAmount));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ApplyBlasterAnimation() {
        StartCoroutine(nameof(BlasterAnimationCo));
    }

    IEnumerator BlasterAnimationCo() {
        animator.SetTrigger("Blaster");
        var suckAround = Instantiate(Game.Blackboard.GetData<ParticleSystem>("SuckAroundEffect"), transform);
        suckAround.Play();
        Destroy(suckAround.gameObject, 1f);
        yield return new WaitForSeconds(1.5f);
        AddDirectionalBlaster();
        var blaster = Instantiate(Game.Blackboard.GetData<ParticleSystem>("BlasterEffect"), transform);
        blaster.Play();
        Destroy(blaster.gameObject, 1.5f);
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Blaster_End");
    }

    void AddWindByBlockChange() {
        var block = m_WindModule.GetBlockByPosition(transform.position);
        if (currentBlock != block) {
            if (currentBlock == null) {
                OnBlockChanged(0, 0, block.x, block.y);
            }
            else {
                OnBlockChanged(currentBlock.x, currentBlock.y, block.x, block.y);
            }

            currentBlock = block;
        }
    }

    private void OnBlockChanged(int xi, int yi, int xf, int yf) {
        //i:initial f:final
        var delta = new Vector3(xf - xi, 0f, yf - yi) * windAmount;
        m_WindModule.AddWind(xi, yi, delta);
        // Debug.Log("Add wind at " + xi + " " + yi + " " + delta.ToString());
    }
}