using System;
using UnityEngine;
using XiheFramework;

public class RoboMovement : GridMovement {
    public float moveDelay = 0.5f;
    public float rotateDelay = 0.25f;

    private TBInputModule m_TbInputModule;

    private void Start() {
        m_TbInputModule = GameManager.GetModule<TBInputModule>();

        GameManager.GetModule<WindFieldModule>().RegisterWalkable(0, 0, 0, 0, 10f);
    }

    private void Update() {
        InitCoordinate();

        if (Vector3.Distance(transform.position, destination) > 0.01f || Quaternion.Angle(transform.rotation, lookDirection) > 1f) {
            transform.position = Vector3.Lerp(transform.position, destination, 0.3f);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookDirection, 0.5f);
        }

        Move();
        Rotate();
    }

    private void Rotate() {
        if (!m_TbInputModule.IsReceivingInput) {
            return;
        }

        LookDirection direction = Game.Blackboard.GetData<LookDirection>("Robo.LookDirection");

        bool changed = false;
        if (Game.Input.GetKey("RotateClockwise")) {
            changed = true;
            switch (direction) {
                case LookDirection.Forward:
                    direction = LookDirection.Right;
                    break;
                case LookDirection.Backward:
                    direction = LookDirection.Left;
                    break;
                case LookDirection.Left:
                    direction = LookDirection.Forward;
                    break;
                case LookDirection.Right:
                    direction = LookDirection.Backward;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (Game.Input.GetKey("RotateCounterClockwise")) {
            changed = true;
            switch (direction) {
                case LookDirection.Forward:
                    direction = LookDirection.Left;
                    break;
                case LookDirection.Backward:
                    direction = LookDirection.Right;
                    break;
                case LookDirection.Left:
                    direction = LookDirection.Backward;
                    break;
                case LookDirection.Right:
                    direction = LookDirection.Forward;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (!changed) {
            return;
        }

        switch (direction) {
            case LookDirection.Forward:
                lookDirection = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                break;
            case LookDirection.Backward:
                lookDirection = Quaternion.LookRotation(Vector3.back, Vector3.up);
                break;
            case LookDirection.Left:
                lookDirection = Quaternion.LookRotation(Vector3.left, Vector3.up);
                break;
            case LookDirection.Right:
                lookDirection = Quaternion.LookRotation(Vector3.right, Vector3.up);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        m_TbInputModule.AddDelay(rotateDelay);
        Game.Blackboard.SetData("Robo.LookDirection", direction);

        Game.Blackboard.SetData("Robo.LookDirection", direction);
    }

    void Move() {
        var input = Game.Input.GetWASDInput();
        if (!m_TbInputModule.IsReceivingInput) {
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

        if (GameManager.GetModule<WindFieldModule>().RegisterWalkable(x, y, x + deltaX, y + deltaY, 0.5f)) {
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

            m_TbInputModule.AddDelay(moveDelay);
        }
    }
}