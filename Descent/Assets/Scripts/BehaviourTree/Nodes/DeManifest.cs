using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeManifest : NodeBase
{
    public MonsterMovementController controller;

    public override void OnTransition() => controller.DeManifest();
}
