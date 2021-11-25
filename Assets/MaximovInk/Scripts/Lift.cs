using MaximovInk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum LiftState { 
    Placing,
    Static
}

public class Lift : MonoBehaviour
    {

    public LiftState State;

    private void Awake()
    {
        State = LiftState.Placing;
    }

    private void Update()
    {
        if (State == LiftState.Placing)
        {
            var cam = GameManager.instance.player.Camera;

            if (Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out RaycastHit hit, Mathf.Infinity, GameManager.instance.GroundMask)) {
                transform.position = hit.point;
                if (Input.GetMouseButtonDown(0) && !Utils.IsOverUI())
                {
                    State = LiftState.Static;
                    GameManager.instance.hotbar.SelectedSlot = -1;
                }
            }
        }
    }
}

