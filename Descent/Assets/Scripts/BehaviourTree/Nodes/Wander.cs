using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : NodeBase
{
    public MonsterMovementController controller;
    public MonsterVariables variables;

    public override void OnTransition()
    {
        return;
    }

    public override void OnExit()
    {
        return;
    }

    public override void Tick()
    {
        controller.Move(variables.playerLastKnownPos);
    }
}
