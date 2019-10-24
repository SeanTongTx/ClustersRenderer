using SeanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class ClustersRender : MonoBehaviour
{
    [InspectorPlus.ReadOnly]
    public ClustersCache Cache = new ClustersCache(0);
    public Mesh mesh;

    [InspectorPlus.Orderable]
    public List<Material> Materials = new List<Material>();

    public ClustersLayout ObjectLayout;
    private void Start()
    {
        if (!ObjectLayout) ObjectLayout = GetComponent<ClustersLayout>();
    }

    public bool ShowBound;
    private void OnDestroy()
    {
        if (ObjectLayout)
        {
            ObjectLayout.Dispose();
        }
    }
    void OnDrawGizmos()
    {
        if (ShowBound)
        {
            // Draw gizmos to show the culling sphere.
            //画出显示筛除剔除球形的小图标
            Color c = Gizmos.color;
            Gizmos.color = Color.yellow;
            Cache.OnDrawGizmos();
            Gizmos.color = c;
        }
    }
    public void Update()
    {
        if (!ObjectLayout || mesh == null)
        {
            return;
        }
        ObjectLayout.UpdateCacheState();
        //Init if Instance count changed
        if (ObjectLayout.Changed)
        {
            Init();
        }
        ObjectLayout.Apply();
        var Visibles = ObjectLayout.GetVisible();
        ObjectLayout.ApplyMaterialProperty(0);
        SetCachedMatProperty();
        if (Cache.VisibleCount > 0)
        {
            for (int i = 0; i < Materials.Count; i++)
            {
                Graphics.DrawMeshInstanced(mesh, i, Materials[i], Visibles, Cache.VisibleCount, Cache.matProperty);
            }
        }
        //延迟到最后 用来减少因裁剪导致的额外计算
        ObjectLayout.PostRender();
    }
    public void Init()
    {
        if (!ObjectLayout) return;
        Cache = new ClustersCache(ObjectLayout.SummaryInstanceCount());
        Cache.matProperty = new MaterialPropertyBlock();
        ObjectLayout.Init(Cache);
    }
    public void SetCachedMatProperty()
    {
        //只有可见性变化时 设置材质属性
        if (Cache.VisiblityChanged)
        {
            var prop_enum = Cache.Properties.GetEnumerator();
            while (prop_enum.MoveNext())
            {
                var property = prop_enum.Current;
                if (property.Value.GetVisible(Cache.Visible).Length > 0)
                {
                    var cacheType = property.Value.CacheType();
                    if (cacheType == typeof(Vector4))
                    {
                        Cache.matProperty.SetVectorArray(property.Key, (property.Value as VisibleCache<Vector4>).Visible);
                    }
                    else if (cacheType == typeof(float))
                    {
                        Cache.matProperty.SetFloatArray(property.Key, (property.Value as VisibleCache<float>).Visible);
                    }
                    else if (cacheType == typeof(Matrix4x4))
                    {
                        Cache.matProperty.SetMatrixArray(property.Key, (property.Value as VisibleCache<Matrix4x4>).Visible);
                    }
                }
            }
        }

    }
}

