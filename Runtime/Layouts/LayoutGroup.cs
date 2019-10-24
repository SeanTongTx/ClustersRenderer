using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LayoutGroup : ClustersLayout
{
    public ClustersRender render;
    public List<ClustersLayout> Layouts = new List<ClustersLayout>();

    public override IClusterObject Templete
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override IList OverrideObjects
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override void UpdateCacheState()
    {
        base.UpdateCacheState();
        foreach (var layout in Layouts)
        {
            layout.UpdateCacheState();
            if(layout.Changed)
            {
                Changed = true;
                return;
            }
        }
    }
    public override int SummaryInstanceCount()
    {
        InstanceCount = 0;
        foreach (var layout in Layouts)
        {
            InstanceCount += layout.SummaryInstanceCount();
        }
        return InstanceCount;
    }
    public override void Init(ClustersCache renderCache)
    {
        if(SummaryInstanceCount()> 1022)
        {
            Debug.LogError("Instance count >1022");
            return;
        }
        this.renderCache = renderCache;
        foreach (var layout in Layouts)
        {
            ClustersCache cache = new ClustersCache(layout.InstanceCount);
            cache.matProperty = renderCache.matProperty;
            layout.Init(cache);
        }
        Changed = false;
    }
    public override void Apply()
    {
        int index = 0, length = 0;
        foreach (var item in Layouts)
        {
            item.Apply();
            length = item.renderCache.Count;
            renderCache.CopyFrom(item.renderCache, index, length);
            index += length;
            if (item.renderCache.VisiblityChanged)
            {
                this.renderCache.VisiblityChanged = true;
            }
        }
    }
    public override Matrix4x4[] GetVisible()
    {
        int sum = 0;
        foreach (var item in Layouts)
        {
            item.GetVisible();
            sum = item.renderCache.VisibleCount;
        }
        this.renderCache.VisibleCount = sum;
            return base.GetVisible();
    }
    public override void PostRender()
    {
        base.PostRender();
        foreach (var item in Layouts)
        {
            item.PostRender();
        }
    }
    public override void ApplyMaterialProperty(int MatIndex)
    {
        /*  //应用材质数据
          int index = 0, length = 0;
          foreach (var item in Layouts)
          {
              item.ApplyMaterialProperty(MatIndex);
              length = item.renderCache.VisibleCount;
              ///材质参数 只复制可见部分
              var v4_enum = item.renderCache.CacheData_v4.GetEnumerator();
              while (v4_enum.MoveNext())
              {
                  var v4_kv = v4_enum.Current;
                  var thisVector = this.renderCache.GetVector(v4_kv.Key, this.renderCache.VisibleCount);
                  Array.Copy(v4_kv.Value, 0, thisVector, index, length);
              }

              var f_enum = item.renderCache.CacheData_f.GetEnumerator();
              while (f_enum.MoveNext())
              {
                  var f_kv = f_enum.Current;
                  var thisfloat = this.renderCache.GetFloat(f_kv.Key, this.renderCache.VisibleCount);
                  Array.Copy(f_kv.Value, 0, thisfloat, index, length);
              }
              var mx_enum = item.renderCache.CacheData_mx.GetEnumerator();
              while (mx_enum.MoveNext())
              {
                  var mx_kv = mx_enum.Current;
                  var thisMtx = this.renderCache.GetMatrix(mx_kv.Key, this.renderCache.VisibleCount);
                  Array.Copy(mx_kv.Value, 0, thisMtx, index, length);
              }
              index += length;
              if (item.renderCache.VisiblityChanged)
              {
                  this.renderCache.VisiblityChanged = true;
              }
          }*/

    }
    public override void Dispose()
    {
        base.Dispose();
        foreach (var item in Layouts)
        {
            item.Dispose();
        }
    }
    public override void ResetObjectTransform()
    {
        foreach (var item in Layouts)
        {
            item.ResetObjectTransform();
        }
    }


}
