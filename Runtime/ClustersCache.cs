using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface VisibleCache
{
    Array GetVisible(bool[] visibility);
    Type CacheType();
}

public class VisibleCache<T>: VisibleCache 
{
    public T[] All;
    public T[] Visible;
    public VisibleCache(T[] data)
    {
        All = new T[data.Length];
        Visible = new T[data.Length];
        Array.Copy(data, All, data.Length);
        Array.Copy(data, Visible, data.Length);
    }
    public VisibleCache(int count)
    {
        All = new T[count];
        Visible = new T[count];
    }

    public Type CacheType()
    {
        return typeof(T);
    }

    public T[] GetVisible(bool[] visibility)
    {
        int index = 0;
        for (int i = 0; i < visibility.Length; i++)
        {
            if(visibility[i])
            {
                Visible[index] = All[i];
                index++;
            }
        }
        return Visible;
    }

    Array VisibleCache.GetVisible(bool[] visibility)
    {
        return GetVisible(visibility);
    }
}

public class ClustersCache
{
    //坐标
    [NonSerialized]
    public VisibleCache<Matrix4x4> TRSmatrices;
    //可见性检测
    [NonSerialized]
    public BoundingSphere[] BoundingBuffer = new BoundingSphere[] { };
    //可见性
    [NonSerialized]
    public bool[] Visible = new bool[] { };
    private int visibleCount;
    public int VisibleCount
    {
        get
        {
            if(Application.isPlaying)
            {
                return visibleCount;
            }
            else
            {
                return Count;
            }
        }
        set
        {
            visibleCount = value;
        }
    }
    public bool VisiblityChanged = true;
    //材质参数
    [NonSerialized]
    public MaterialPropertyBlock matProperty;
    //材质参数
    public Dictionary<string, VisibleCache> Properties = new Dictionary<string, VisibleCache>();
    public VisibleCache<T>GetProperty<T>(string propertyKey)
    {
        VisibleCache value = null;
        if (!Properties.TryGetValue(propertyKey, out value))
        {
            value = new VisibleCache<T>(Visible.Length);
            Properties[propertyKey] = value;
        }
        value = Properties[propertyKey];
        return value as VisibleCache<T>;
    }
    public Matrix4x4[] GetVisible()
    {
        if (Application.isPlaying)
        {
            if (VisiblityChanged)
            {
                return TRSmatrices.GetVisible(Visible);
            }
            return TRSmatrices.Visible;
        }
        else
        {
            return TRSmatrices.All;
        }
    }
    public ClustersCache(int count)
    {
        TRSmatrices = new VisibleCache<Matrix4x4>(count);
        BoundingBuffer = new BoundingSphere[count];
        Visible = new bool[count];
    }
    public int Count
    {
        get
        {
            return Visible.Length;
        }
    }
    public void OnDrawGizmos()
    {
        foreach (var Bound in BoundingBuffer)
        {
            Gizmos.DrawWireSphere(Bound.position, Bound.radius);
        }
    }
    public Vector3 ExtractScale(Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }
    public void CopyFrom(ClustersCache cache, int index, int length)
    {
       /* 
        Array.Copy(cache.TRSmatrices, 0, this.TRSmatrices, index, length);
        Array.Copy(cache.BoundingBuffer, 0, this.BoundingBuffer, index, length);
        Array.Copy(cache.Visible, 0, this.Visible, index, length);
        */
    }
}
