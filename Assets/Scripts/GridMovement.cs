using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using XiheFramework;


//not reusable at this point (dependency on wind module)
public abstract class GridMovement : MonoBehaviour {
    public int x = -1;
    public int y = -1;
    public Vector3 destination;
    public Quaternion lookDirection;

    // Update is called once per frame
    protected void InitCoordinate() {
        var wind = GameManager.GetModule<WindFieldModule>();
        if (!wind) {
            return;
        }

        //init position
        if (x == -1 && y == -1) {
            var currentBlock = wind.GetBlockByPosition(transform.position);
            x = currentBlock.x;
            y = currentBlock.y;
            destination = currentBlock.transform.position;
        }
    }
}