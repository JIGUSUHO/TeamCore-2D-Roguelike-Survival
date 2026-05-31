using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownController : MonoBehaviour

{
    public event Action<Vector2> onMoveEvent;
    public event Action<Vector2> onLookEvent;
    
    public void CallMoveEvent (Vector2 direction)
    {
        onMoveEvent?.Invoke(direction);
    }
    public void CallLookEvent (Vector2 direction)
    { onLookEvent?.Invoke(direction);}
    // Update is called once per frame
    
}

