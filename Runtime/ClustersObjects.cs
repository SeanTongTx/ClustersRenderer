
using System;
using UnityEngine;
public interface IClusterObject
{
    int Index { get; set; }
    Vector3 LocalPostion { get; set; }
    Vector3 LocalRotation { get; set; }
    Vector3 LocalScale { get; set; }
    float BoundScale { get; }
    Vector3 BoundOffset { get; }
}
[Serializable]
public class ClusterObject : IClusterObject
{
    public Color color=Color.white;
    ///------------------------------//

    [SerializeField]
    private int index;
    public int Index
    {
        get
        {
            return index;
        }

        set
        {
            index = value;
        }
    }
    [SerializeField]
    private float boundScale = 1;
    [SerializeField]
    private Vector3 boundOffset = Vector3.zero;
    [SerializeField]
    private Vector3 localPos;
    public Vector3 LocalPostion
    {
        get
        {
            return localPos;
        }
        set
        {
            localPos = value;
        }
    }
    [SerializeField]
    private Vector3 localRot;
    public Vector3 LocalRotation
    {
        get
        {
            return localRot;
        }
        set
        {
            localRot = value;
        }
    }
    [SerializeField]
    private Vector3 localScale = Vector3.one;
    public Vector3 LocalScale
    {
        get
        {
            return localScale;
        }
        set
        {
            localScale = value;
        }
    }

    public float BoundScale
    {
        get
        {
            return boundScale;
        }
    }

    public Vector3 BoundOffset
    {
        get
        {
            return boundOffset;
        }
    }
}