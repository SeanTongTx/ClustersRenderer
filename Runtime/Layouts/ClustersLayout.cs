using SeanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
public abstract class ClustersLayout : MonoBehaviour
{
    public abstract IClusterObject Templete { get; }
    public abstract IList OverrideObjects { get; }
    [NonSerialized]
    public bool Changed=true;
    [SerializeField]
    [Range(1, 1022), Tooltip("0-512 1pass|512-1022 2pass")]
    public int InstanceCount = 100;
    /// <summary>
    /// 统计实际的实例数量
    /// </summary>
    /// <returns></returns>
    public virtual int SummaryInstanceCount()
    {
        return InstanceCount;
    }
    public ClustersCache renderCache;
    // Use this for initialization
    public virtual void Init(ClustersCache renderCache)
    {
        this.renderCache = renderCache;
        InitCullBound();
        //force update object Transform
        ResetObjectTransform();
        Changed = false;
    }
    // Update is called once per frame
    public virtual void Apply()
    {
        if (transform.hasChanged)
        {
            ResetObjectTransform();
        }
        else
        {
            for (int i = 0; i < OverrideObjects.Count; i++)
            {
                var overrideObj = OverrideObjects[i] as IClusterObject;
                ApplyOverrideTransform(overrideObj.Index, overrideObj,transform);
            }
           // ResetCullBound();
        }
    }
    public virtual void UpdateCacheState()
    {
        if(renderCache==null|| renderCache.Count != InstanceCount)
        {
            Changed = true;
        }
    }
    public virtual void ApplyMaterialProperty(int MatIndex)
    {
    }
    public virtual void PostRender()
    {
        renderCache.VisiblityChanged = false;
    }
    public virtual Matrix4x4[] GetVisible()
    {
        return renderCache.GetVisible();
    }
    public virtual void Dispose()
    {
        if (Application.isPlaying)
        {
            if (m_CullingGroup != null)
                m_CullingGroup.Dispose();
        }
    }
    #region Culling
    private CullingGroup m_CullingGroup;
    internal virtual void InitCullBound()
    {
        if (Application.isPlaying)
        {
            if (m_CullingGroup == null)
            {
                m_CullingGroup = new CullingGroup();
                m_CullingGroup.targetCamera = Camera.main;
                m_CullingGroup.onStateChanged += OnStateChanged;
            }
        }
    }
    internal virtual void OnStateChanged(CullingGroupEvent sphere)
    {
        if (renderCache.Visible[sphere.index] != sphere.isVisible)
        {
            renderCache.VisibleCount += sphere.isVisible?1:-1;
            renderCache.VisiblityChanged = true;
            renderCache.Visible[sphere.index] = sphere.isVisible;
        }
    }
    internal virtual void ResetCullBound()
    {
        if (Application.isPlaying && m_CullingGroup != null)
        {
            m_CullingGroup.SetBoundingSpheres(renderCache.BoundingBuffer);
            m_CullingGroup.SetBoundingSphereCount(InstanceCount);
            renderCache.VisibleCount = 0;
            for (int i = 0; i < renderCache.Visible.Length; i++)
            {
                renderCache.Visible[i] = m_CullingGroup.IsVisible(i);
                if (renderCache.Visible[i])
                {
                    renderCache.VisibleCount++;
                }
            }
        }
    }
    #endregion
    #region Transform
    /// <summary>
    /// if objects count changed
    /// all buffer should rebuild
    /// </summary>
    [InspectorPlus.Button(Editor = true)]
    public virtual void ResetObjectTransform()
    {
        Transform renderTrans = this.transform;
        for (int i = 0; i < InstanceCount; i++)
        {
            TempleteObjectsTransform(i, renderTrans);
        }
        foreach (var item in OverrideObjects)
        {
            var overrideObj = item as IClusterObject;
            ApplyOverrideTransform(overrideObj.Index, overrideObj, renderTrans);
        }
        ResetCullBound();
        renderTrans.hasChanged = false;
        if(renderCache!=null)
        {
            renderCache.VisiblityChanged = true;
        }
    }
    /// <summary>
    /// default objects layout
    /// </summary>
    /// <param name="index"></param>
    /// <param name="root"></param>
    internal virtual void TempleteObjectsTransform(int index, Transform root)
    {
        var matrix = Matrix4x4.TRS(root.position, root.rotation, root.lossyScale) * Matrix4x4.TRS(Templete.LocalPostion, Quaternion.Euler(Templete.LocalRotation), Templete.LocalScale);
        renderCache.TRSmatrices.All[index] = matrix;
        SetBoundingBuffer(index, Templete, matrix, root);
    }
    /// <summary>
    /// override special object transform
    /// </summary>
    /// <param name="overrideObject"></param>
    /// <param name="root"></param>
    public virtual void ApplyOverrideTransform(int index, IClusterObject overrideObject, Transform root)
    {
        if (renderCache==null|| renderCache.Count == 0) return;
        var objtrans = Matrix4x4.TRS(root.position, root.rotation, root.lossyScale) * Matrix4x4.TRS(overrideObject.LocalPostion, Quaternion.Euler(overrideObject.LocalRotation), overrideObject.LocalScale);

        SetBoundingBuffer(index, overrideObject, objtrans, root);
        renderCache.TRSmatrices.All[index] = objtrans;
    }
    internal virtual void SetBoundingBuffer(int index, IClusterObject obj, Matrix4x4 objTransMatrix, Transform root)
    {
        //Set cullBound
        Vector3 rootscale = root.lossyScale;
        renderCache.BoundingBuffer[index].position = new Vector3(objTransMatrix.m03, objTransMatrix.m13, objTransMatrix.m23) + obj.BoundOffset;
        //ignore gc
        float max = (rootscale.x * obj.LocalScale.x > rootscale.y * obj.LocalScale.y) ? rootscale.x * obj.LocalScale.x : rootscale.y * obj.LocalScale.y;
        renderCache.BoundingBuffer[index].radius = max * obj.BoundScale;
    }
    #endregion

    private void OnValidate()
    {
        Changed = true;
    }
}
