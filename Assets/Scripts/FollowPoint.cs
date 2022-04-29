using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPoint
{
    private Vector3 pos;
    private FollowPoint next;
    public FollowPoint(Vector3 pos, FollowPoint next)
    {
        SetPos(pos);
        SetNext(next);
    }
    private void SetPos(Vector3 pos)
    {
        this.pos = pos;
    }
    public void SetNext(FollowPoint next)
    {
        this.next = next;
    }
    public FollowPoint GetNext()
    {
        return next;
    }
    public Vector3 GetPos()
    {
        return pos;
    }
}
