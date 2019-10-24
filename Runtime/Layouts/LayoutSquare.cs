using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LayoutSquare : ClustersLayout
{
    /*[InspectorPlus.MinValue(1)]*/
    public int ColumSize=50;
    public Vector3 Spacing = Vector3.one;

  /*  [NonSerialized]
    public Vector4[] Colors;*/
    public override IClusterObject Templete
    {
        get
        {
            return templete;
        }
    }
    public ClusterObject templete;
    [Header("Random")]
    System.Random random;
    public bool Random;
    public Vector2 randomRange = new Vector2(-20, 80);
    public int RandomSeed = 0;

    public List<ClusterObject> overriders = new List<ClusterObject>();
    public override IList OverrideObjects
    {
        get
        {
            return overriders;
        }
    }
    public override void Init(ClustersCache renderCache)
    {
        base.Init(renderCache);
    }
    public override void ResetObjectTransform()
    {
        random = new System.Random(RandomSeed);
        base.ResetObjectTransform();
    }
    internal override void TempleteObjectsTransform(int index, Transform root)
    {
        if (renderCache.Count == 0) return;
        int row = index / ColumSize - renderCache.Count / ColumSize / 2;
        int colum = index%ColumSize- ColumSize/2;

        Vector3 pos =new Vector3(Spacing.x*colum,Spacing.y*row,Spacing.z*row);
        Quaternion rot = Quaternion.Euler(Templete.LocalRotation);
        Vector3 scale = Templete.LocalScale;
        if (Random)
        {
            float r = (float)random.Next((int)randomRange.x, (int)randomRange.y) / 100f;
            Vector3 posR = Templete.LocalPostion * r;
            pos = pos + posR;

            Vector3 rotR = Templete.LocalRotation * r;
            rot= Quaternion.Euler(Templete.LocalRotation+rotR);

            Vector3 scalR = scale * r;
            scale = scale + scalR;
        }
        var matrix = Matrix4x4.TRS(root.position, root.rotation, root.lossyScale) * Matrix4x4.TRS(pos, rot, scale);
        renderCache.TRSmatrices.All[index] = matrix;

        SetBoundingBuffer(index, Templete, matrix, root);
    }
}
