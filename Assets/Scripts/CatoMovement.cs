using UnityEngine;
using XiheFramework;
using Random = UnityEngine.Random;

public class CatoMovement : GridMovement {
    //ai
    public Vector2 waitTimeRange;
    public Animator animator;

    public int state;
    private float m_Timer = 0f;

    private Vector3 m_LockedPosition;

    private void Start() {
        //state = 1;

        Game.Event.Subscribe("OnCatoFlowed", OnCatoFlowed);
        Game.Event.Subscribe("OnCatoLanded", OnCatoLanded);
        Game.Event.Subscribe("OnCatoLocked", OnCatoLocked);
    }

    private void OnCatoLocked(object sender, object e) {
        transform.position = (Vector3) sender;
        transform.rotation = (Quaternion) e * Quaternion.Euler(Vector3.up * 180f);
        m_LockedPosition = (Vector3) sender;
        state = 3;
    }

    private void OnCatoLanded(object sender, object e) {
        state = 4;
    }

    private void OnCatoFlowed(object sender, object e) {
        state = 0;
    }

    private void Update() {
        InitCoordinate();

        switch (state) {
            case 0:
                if (m_LockedPosition != Vector3.zero) {
                    destination = m_LockedPosition;
                    transform.position = m_LockedPosition;
                }

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
                //moving
                if (Vector3.Distance(transform.position, destination) < 0.01f &&
                    Vector3.Angle(transform.rotation.eulerAngles, lookDirection.eulerAngles) < 1f) {
                    state = 1;
                    m_Timer = Random.Range(waitTimeRange.x, waitTimeRange.y);
                }

                transform.position = Vector3.Lerp(transform.position, destination, 0.3f);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookDirection, 0.66f);
                break;
            case 3:
                animator.SetBool("Locked", true);
                state = 0;

                break;
            case 4: //landing

                UpdateCoordinate();
                state = 1;

                break;
        }
    }

    private void UpdateCoordinate() {
        var currentBlock = GameManager.GetModule<WindFieldModule>().GetBlockByPosition(transform.position);
        if (GameManager.GetModule<WindFieldModule>().RegisterWalkable(x, y, currentBlock.x, currentBlock.y, 0.25f)) {
            x = currentBlock.x;
            y = currentBlock.y;
            destination = new Vector3(x * 1f, 0f, y * 1f);
            lookDirection = Quaternion.LookRotation(destination - transform.position, Vector3.up);
        }
    }

    void SetNewDestination() {
        var direction = Random.Range(0, 4);
        var currentBlock = GameManager.GetModule<WindFieldModule>().GetBlockByPosition(transform.position);
        x = currentBlock.x;
        y = currentBlock.y;
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

        if (GameManager.GetModule<WindFieldModule>().RegisterWalkable(x, y, tempX, tempY, 0.5f)) {
            x = tempX;
            y = tempY;
            destination = new Vector3(x * 1f, 0f, y * 1f);
            lookDirection = Quaternion.LookRotation(destination - transform.position, Vector3.up);
        }
    }
}