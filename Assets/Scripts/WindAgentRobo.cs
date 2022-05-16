using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;
using Random = UnityEngine.Random;

public class WindAgentRobo : MonoBehaviour {
    public WindBlock currentBlock;
    public Animator animator;
    public Renderer render;
    public float windAmount = 5f;
    public float ultWindAmount = 5f;
    public float fanDelay = 0.5f;
    public float blasterDelay = 4f;

    private WindFieldModule m_WindModule;
    private TBInputModule m_TbInputModule;

    private void Start() {
        m_WindModule = GameManager.GetModule<WindFieldModule>();
        m_TbInputModule = GameManager.GetModule<TBInputModule>();

        Game.Event.Subscribe("OnPointGet", UpdateBulb);
    }

    private void UpdateBulb(object sender, object e) {
        var value = Game.Blackboard.GetData<float>("UltCharge");
        value = Mathf.Clamp(value, 0f, 100f);
        render.materials[0].SetFloat("_ColorMultiplier", value / 10f);
    }

    private void Update() {
        //AddWindByBlockChange();
        var block = m_WindModule.GetBlockByPosition(transform.position);
        currentBlock = block;

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
                m_WindModule.AddWind(currentBlock.x, currentBlock.y, Vector3.forward * (windAmount * 2f));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 1, Vector3.forward * (windAmount * 1.5f));
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y + 1, (Vector3.forward + Vector3.left).normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y + 1, (Vector3.forward + Vector3.right).normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 2, Vector3.forward * windAmount);
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y + 2, (Vector3.forward + Vector3.left).normalized * (windAmount / 2));
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y + 2, (Vector3.forward + Vector3.right).normalized * (windAmount / 2));

                break;
            case LookDirection.Backward:
                m_WindModule.AddWind(currentBlock.x, currentBlock.y, Vector3.back * (windAmount * 2f));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 1, Vector3.back * (windAmount * 1.5f));
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y - 1, (Vector3.back + Vector3.left).normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y - 1, (Vector3.back + Vector3.right).normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 2, Vector3.back.normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y - 2, (Vector3.back + Vector3.left).normalized * (windAmount / 2));
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y - 2, (Vector3.back + Vector3.right).normalized * (windAmount / 2));
                break;
            case LookDirection.Left:
                m_WindModule.AddWind(currentBlock.x, currentBlock.y, Vector3.left * (windAmount * 2f));
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y, Vector3.left * (windAmount * 1.5f));
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y - 1, (Vector3.left + Vector3.back).normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y + 1, (Vector3.left + Vector3.forward).normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x - 2, currentBlock.y, Vector3.left.normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x - 2, currentBlock.y - 1, (Vector3.left + Vector3.back).normalized * (windAmount / 2));
                m_WindModule.AddWind(currentBlock.x - 2, currentBlock.y + 1, (Vector3.left + Vector3.forward).normalized * (windAmount / 2));
                break;
            case LookDirection.Right:
                m_WindModule.AddWind(currentBlock.x, currentBlock.y, Vector3.right * (windAmount * 2f));
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y, Vector3.right * (windAmount * 1.5f));
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y - 1, (Vector3.right + Vector3.back).normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y + 1, (Vector3.right + Vector3.forward).normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x + 2, currentBlock.y, Vector3.right.normalized * windAmount);
                m_WindModule.AddWind(currentBlock.x + 2, currentBlock.y - 1, (Vector3.right + Vector3.back).normalized * (windAmount / 2));
                m_WindModule.AddWind(currentBlock.x + 2, currentBlock.y + 1, (Vector3.right + Vector3.forward).normalized * (windAmount / 2));
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
        //se
        var ran = Random.Range(1, 4);
        var clipName = "SFX-NormalAttack-" + ran + "_1";
        Game.Audio.Play(AudioChannelTypes.SoundEffect, clipName, 1f, false);

        animator.SetTrigger("Fan");
        var particle = Instantiate(Game.Blackboard.GetData<ParticleSystem>("FanEffect"), transform);
        particle.Play();
        Destroy(particle.gameObject, 0.5f);
        yield return new WaitForSeconds(0.4f);
        animator.SetTrigger("Fan_End");
    }

    void AddWindByBlaster() {
        var ult = Game.Blackboard.GetData<float>("UltCharge");
        if (ult < 100f) return;
        if (!Game.Input.GetKey("Ultimate")) return;
        if (!m_TbInputModule.IsReceivingInput) return;

        Game.Blackboard.SetData("UltCharge", 0f);
        m_TbInputModule.AddDelay(blasterDelay);

        ApplyBlasterAnimation();
    }

    void AddDirectionalBlaster() {
        var direction = Game.Blackboard.GetData<LookDirection>("Robo.LookDirection");
        // Debug.Log(direction.ToString());
        switch (direction) {
            case LookDirection.Forward:
                m_WindModule.AddWind(currentBlock.x, currentBlock.y, Vector3.forward * (6f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 1, Vector3.forward * (5f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 2, Vector3.forward * (5f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 3, Vector3.forward * (5f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 4, Vector3.forward * (4.7f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 5, Vector3.forward * (3.2f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y + 6, Vector3.forward * (2.3f * ultWindAmount));
                break;
            case LookDirection.Backward:
                m_WindModule.AddWind(currentBlock.x, currentBlock.y, Vector3.back * (6f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 1, Vector3.back * (5f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 2, Vector3.back * (5f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 3, Vector3.back * (5f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 4, Vector3.back * (4.7f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 5, Vector3.back * (3.2f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x, currentBlock.y - 6, Vector3.back * (2.3f * ultWindAmount));
                break;
            case LookDirection.Left:
                m_WindModule.AddWind(currentBlock.x, currentBlock.y, Vector3.left * (6f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y, Vector3.left * (5f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x - 2, currentBlock.y, Vector3.left * (5f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x - 3, currentBlock.y, Vector3.left * (5f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x - 4, currentBlock.y, Vector3.left * (4.7f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x - 5, currentBlock.y, Vector3.left * (3.2f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x - 6, currentBlock.y, Vector3.left * (2.3f * ultWindAmount));
                break;
            case LookDirection.Right:
                m_WindModule.AddWind(currentBlock.x, currentBlock.y, Vector3.right * (6f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y, Vector3.right * (5f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x + 2, currentBlock.y, Vector3.right * (5f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x + 3, currentBlock.y, Vector3.right * (5f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x + 4, currentBlock.y, Vector3.right * (4.7f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x + 5, currentBlock.y, Vector3.right * (3.2f * ultWindAmount));
                m_WindModule.AddWind(currentBlock.x + 6, currentBlock.y, Vector3.right * (2.3f * ultWindAmount));
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
        var suckAround = Instantiate(Game.Blackboard.GetData<ParticleSystem>("ConcentrateEffect"), transform);
        suckAround.Play(true);
        //se
        Game.Audio.Play(AudioChannelTypes.SoundEffect, "SFX-WatergunBlaster", 1f, false);
        //around
        m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y + 1, (Vector3.back + Vector3.right).normalized * windAmount, true);
        m_WindModule.AddWind(currentBlock.x, currentBlock.y + 1, (Vector3.back).normalized * windAmount, true);
        m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y + 1, (Vector3.back + Vector3.left).normalized * windAmount, true);
        m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y, (Vector3.right).normalized * windAmount, true);
        m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y, (Vector3.left).normalized * windAmount, true);
        m_WindModule.AddWind(currentBlock.x - 1, currentBlock.y - 1, (Vector3.forward + Vector3.right).normalized * windAmount, true);
        m_WindModule.AddWind(currentBlock.x, currentBlock.y - 1, (Vector3.forward).normalized * windAmount, true);
        m_WindModule.AddWind(currentBlock.x + 1, currentBlock.y - 1, (Vector3.forward + Vector3.left).normalized * windAmount, true);

        Destroy(suckAround.gameObject, 1.5f);
        float _timer = 0f;
        while (_timer < 2f) {
            render.material.SetFloat("_ColorMultiplier", MathXi.Map(_timer, 0f, 2.5f, 1f, 20f));
            _timer += Time.deltaTime;
            yield return null;
        }

        _timer = 0f;

        AddDirectionalBlaster();

        animator.SetTrigger("Blaster_Shoot");

        var blaster = Instantiate(Game.Blackboard.GetData<ParticleSystem>("BlasterEffect"), transform);
        blaster.Play(true);
        Destroy(blaster.gameObject, 1.5f);

        yield return null;

        while (_timer < 0.5f) {
            render.material.SetFloat("_ColorMultiplier", MathXi.Map(0.5f - _timer, 0f, 0.5f, 0.1f, 20f));
            _timer += Time.deltaTime;
            yield return null;
        }

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