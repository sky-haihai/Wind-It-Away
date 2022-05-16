using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

public class Bed : DeadZone {
    public LookDirection forward;

    private bool m_FirstFramed = false;

    private void Update() {
        if (!m_FirstFramed) {
            var wind = GameManager.GetModule<WindFieldModule>();

            var block = wind.GetBlockByPosition(transform.position);
            wind.SetWalkable(block.x, block.y, false);
            switch (forward) {
                case LookDirection.Forward:
                    wind.SetWalkable(block.x - 1, block.y, false);
                    wind.SetWalkable(block.x - 1, block.y - 1, false);
                    wind.SetWalkable(block.x - 1, block.y - 2, false);
                    wind.SetWalkable(block.x - 1, block.y - 3, false);
                    wind.SetWalkable(block.x - 1, block.y - 4, false);

                    wind.SetWalkable(block.x + 1, block.y, false);
                    wind.SetWalkable(block.x + 1, block.y - 1, false);
                    wind.SetWalkable(block.x + 1, block.y - 2, false);
                    wind.SetWalkable(block.x + 1, block.y - 3, false);
                    wind.SetWalkable(block.x + 1, block.y - 4, false);

                    wind.SetWalkable(block.x, block.y - 1, false);
                    wind.SetWalkable(block.x, block.y - 2, false);
                    wind.SetWalkable(block.x, block.y - 3, false);
                    wind.SetWalkable(block.x, block.y - 4, false);

                    break;
                case LookDirection.Backward:
                    wind.SetWalkable(block.x - 1, block.y, false);
                    wind.SetWalkable(block.x - 1, block.y + 1, false);
                    wind.SetWalkable(block.x - 1, block.y + 2, false);
                    wind.SetWalkable(block.x - 1, block.y + 3, false);
                    wind.SetWalkable(block.x - 1, block.y + 4, false);

                    wind.SetWalkable(block.x + 1, block.y, false);
                    wind.SetWalkable(block.x + 1, block.y + 1, false);
                    wind.SetWalkable(block.x + 1, block.y + 2, false);
                    wind.SetWalkable(block.x + 1, block.y + 3, false);
                    wind.SetWalkable(block.x + 1, block.y + 4, false);

                    wind.SetWalkable(block.x, block.y + 1, false);
                    wind.SetWalkable(block.x, block.y + 2, false);
                    wind.SetWalkable(block.x, block.y + 3, false);
                    wind.SetWalkable(block.x, block.y + 4, false);
                    break;
                case LookDirection.Left:
                    wind.SetWalkable(block.x, block.y - 1, false);
                    wind.SetWalkable(block.x + 1, block.y - 1, false);
                    wind.SetWalkable(block.x + 2, block.y - 1, false);
                    wind.SetWalkable(block.x + 3, block.y - 1, false);
                    wind.SetWalkable(block.x + 4, block.y - 1, false);


                    wind.SetWalkable(block.x, block.y + 1, false);
                    wind.SetWalkable(block.x + 1, block.y + 1, false);
                    wind.SetWalkable(block.x + 2, block.y + 1, false);
                    wind.SetWalkable(block.x + 3, block.y + 1, false);
                    wind.SetWalkable(block.x + 4, block.y + 1, false);

                    wind.SetWalkable(block.x + 1, block.y, false);
                    wind.SetWalkable(block.x + 2, block.y, false);
                    wind.SetWalkable(block.x + 3, block.y, false);
                    wind.SetWalkable(block.x + 4, block.y, false);

                    break;
                case LookDirection.Right:
                    wind.SetWalkable(block.x, block.y - 1, false);
                    wind.SetWalkable(block.x - 1, block.y - 1, false);
                    wind.SetWalkable(block.x - 2, block.y - 1, false);
                    wind.SetWalkable(block.x - 3, block.y - 1, false);
                    wind.SetWalkable(block.x - 4, block.y - 1, false);


                    wind.SetWalkable(block.x, block.y + 1, false);
                    wind.SetWalkable(block.x - 1, block.y + 1, false);
                    wind.SetWalkable(block.x - 2, block.y + 1, false);
                    wind.SetWalkable(block.x - 3, block.y + 1, false);
                    wind.SetWalkable(block.x - 4, block.y + 1, false);

                    wind.SetWalkable(block.x - 1, block.y, false);
                    wind.SetWalkable(block.x - 2, block.y, false);
                    wind.SetWalkable(block.x - 3, block.y, false);
                    wind.SetWalkable(block.x - 4, block.y, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            m_FirstFramed = true;
        }
    }
}