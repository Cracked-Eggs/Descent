using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : NodeBase
{
    public override void OnTransition()
    {
        Debug.Log("Died");
        //Add kill logic here
    }
}
