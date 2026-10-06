using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnockBack 
{
    void KnockBack(Vector2 angle, float knockbackForce, int direction);
}
