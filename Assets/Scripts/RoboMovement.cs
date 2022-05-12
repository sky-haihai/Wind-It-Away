using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class RoboMovement : MonoBehaviour {
    public int x;
    public int y;

    public Vector3 destination;
    public Quaternion lookDirection;
    public float inputDelay = 0.5f;

    private float m_InputTimer = 0f;

    private const string WaitingInput = "IsReceivingAnyInput";

    private void Update() {
        Move();
        UpdateInputTimer();
    }

    void UpdateInputTimer() {
        m_InputTimer += Time.deltaTime;
    }

    void Move() {
        transform.position = Vector3.Lerp(transform.position, destination, 0.3f);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookDirection, 0.3f);
        if (Vector3.Distance(transform.position, destination) < 0.1f) {
            if (m_InputTimer >= inputDelay) {
                Game.Blackboard.SetData(WaitingInput, true);
            }
        }

        var input = Game.Input.GetWASDInput();
        var waiting = Game.Blackboard.GetData<bool>(WaitingInput);
        if (!waiting) {
            return;
        }

        var gridSize = Game.Blackboard.GetData<float>("GridSize");
        var gridMax = Game.Blackboard.GetData<Vector2>("GridMax");

        var tmpX = x;
        var tmpY = y;
        if (Game.Input.GetKey("Forward")) {
            if (GameManager.GetModule<WindFieldModule>().HasBlock(tmpX, tmpY + 1)) {
                tmpY += 1;
            }
        }

        if (Game.Input.GetKey("Backward")) {
            if (GameManager.GetModule<WindFieldModule>().HasBlock(tmpX, tmpY - 1)) {
                tmpY -= 1;
            }
        }

        if (Game.Input.GetKey("Left")) {
            if (GameManager.GetModule<WindFieldModule>().HasBlock(tmpX - 1, tmpY)) {
                tmpX -= 1;
            }
        }

        if (Game.Input.GetKey("Right")) {
            if (GameManager.GetModule<WindFieldModule>().HasBlock(tmpX + 1, tmpY)) {
                tmpX += 1;
            }
        }

        if (tmpX != x || tmpY != y) {
            MoveAxis(tmpX - x, tmpY - y, gridSize);
        }
    }

    void MoveAxis(int deltaX, int deltaY, float gridSize) {
        //no diagonal move
        if (deltaX != 0) {
            deltaY = 0;
        }
        else if (deltaY != 0) {
            deltaX = 0;
        }

        if (GameManager.GetModule<WindFieldModule>().HasBlock(x + deltaX, y + deltaY)) {
            x += deltaX;
            y += deltaY;
            destination = new Vector3(x * gridSize, 0f, y * gridSize);
            lookDirection = Quaternion.LookRotation(destination - transform.position, Vector3.up);

            LookDirection direction = default;
            if (deltaX > 0) {
                direction = LookDirection.Right;
            }
            else if (deltaX < 0) {
                direction = LookDirection.Left;
            }
            else if (deltaY > 0) {
                direction = LookDirection.Forward;
            }
            else if (deltaY < 0) {
                direction = LookDirection.Backward;
            }

            Game.Blackboard.SetData("Robo.LookDirection", direction);

            Game.Blackboard.SetData(WaitingInput, false);
            m_InputTimer = 0f;
        }
    }
}