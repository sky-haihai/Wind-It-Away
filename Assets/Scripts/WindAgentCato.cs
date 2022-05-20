using Unity.Mathematics;
using UnityEngine;
using XiheFramework;

public class WindAgentCato : MonoBehaviour {
    public GameObject dustPrefab;
    public Transform spawnRoot;

    public WindBlock currentBlock;
    public float windAmount = 5f;

    private WindFieldModule m_WindModule;

    private void Start() {
        m_WindModule = GameManager.GetModule<WindFieldModule>();
        InvokeRepeating(nameof(SpawnDust), 0f, 0.3f);
        Game.Event.Subscribe("OnCatoLocked", OnCatoLocked);
    }

    private void OnCatoLocked(object sender, object o) {
        CancelInvoke(nameof(SpawnDust));
    }

    private void Update() {
        AddWindByBlockChange();
        // SpawnDust();
    }

    private void SpawnDust() {
        Instantiate(dustPrefab, spawnRoot.position, quaternion.identity);
        Game.Blackboard.SetData("ClearPoint", Game.Blackboard.GetData<int>("ClearPoint") + 1);
    }

    void AddWindByBlockChange() {
        var block = m_WindModule.GetBlockByPosition(transform.position);
        if (currentBlock != block) {
            if (currentBlock == null) {
                //OnBlockChanged(0, 0, block.x, block.y);
            }
            else {
                OnBlockChanged(currentBlock.x, currentBlock.y, block.x, block.y);
            }

            currentBlock = block;
        }
    }

    private void OnBlockChanged(int xi, int yi, int xf, int yf) {
        //i:initial f:final
        var delta = new Vector3(xf - xi, 0f, yf - yi).normalized * windAmount;
        m_WindModule.AddWind(xi, yi, -delta, false);
        m_WindModule.AddWind(xf, yf, delta, false);

        // var deltaX = xf - xi;
        // var deltaY = yf - yi;
        // if (deltaX > 0 && deltaY == 0) {
        //     m_WindModule.AddWind(xf, yf + 1, Vector3.Cross(delta, Vector3.up), false);
        //     m_WindModule.AddWind(xf, yf - 1, Vector3.Cross(-delta, Vector3.up), false);
        // }
        //
        // if (deltaX < 0 && deltaY == 0) {
        //     m_WindModule.AddWind(xf, yf + 1, Vector3.Cross(-delta, Vector3.up), false);
        //     m_WindModule.AddWind(xf, yf - 1, Vector3.Cross(delta, Vector3.up), false);
        // }
        //
        // if (deltaY > 0 && deltaX == 0) {
        //     m_WindModule.AddWind(xf - 1, yf, Vector3.Cross(-delta, Vector3.up), false);
        //     m_WindModule.AddWind(xf + 1, yf, Vector3.Cross(delta, Vector3.up), false);
        // }
        //
        // if (deltaY < 0 && deltaX == 0) {
        //     m_WindModule.AddWind(xf - 1, yf, Vector3.Cross(delta, Vector3.up), false);
        //     m_WindModule.AddWind(xf + 1, yf, Vector3.Cross(-delta, Vector3.up), false);
        // }

        // Debug.Log("Add wind at " + xi + " " + yi + " " + delta.ToString());
    }
}