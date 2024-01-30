using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manifest : NodeBase
{
    public MonsterMovementController controller;

    public override void OnTransition()
    {
        controller.Manifest();
        controller.Walk();
    }
}
