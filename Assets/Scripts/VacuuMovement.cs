using UnityEngine;
using XiheFramework;
using Random = UnityEngine.Random;

public class VacuuMovement : GridMovement {
    //ai
    public Vector2 waitTimeRange;

    public int state;
    private float m_Timer = 0f;

    private void Start() {
        //state = 1;
    }

    private void Update() {
        InitCoordinate();

        switch (state) {
            case 0:
                //idle
                break;
            case 1:
                //waiting
                if (m_Timer > 0) {
                    m_Timer -= Time.deltaTime;
                    return;
                }

                SetNewDestination();
                state = 2;
                break;
            case 2:
                Debug.Log(Vector3.Distance(transform.position, destination));
                Debug.Log(transform.rotation.eulerAngles);
                Debug.Log(lookDirection.eulerAngles);

                //moving
                if (Vector3.Distance(transform.position, destination) < 0.01f &&
                    Vector3.Angle(transform.rotation.eulerAngles, lookDirection.eulerAngles) < 1f) {
                    Debug.Log("Change to 1");
                    state = 1;
                    m_Timer = Random.Range(waitTimeRange.x, waitTimeRange.y);
                }

                transform.position = Vector3.Lerp(transform.position, destination, 0.05f);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookDirection, 0.1f);
                break;
        }
    }

    void SetNewDestination() {
        var direction = Random.Range(0, 4);
        var tempX = x;
        var tempY = y;
        switch (direction) {
            case 0:
                tempY += 1;
                break;
            case 1:
                tempY -= 1;
                break;
            case 2:
                tempX -= 1;
                break;
            case 3:
                tempX += 1;
                break;
        }

        if (GameManager.GetModule<WindFieldModule>().RegisterWalkable(x, y, tempX, tempY, 2f)) {
            x = tempX;
            y = tempY;
            destination = new Vector3(x * 1f, 0f, y * 1f);
            lookDirection = Quaternion.LookRotation(destination - transform.position, Vector3.up);
        }
    }
}