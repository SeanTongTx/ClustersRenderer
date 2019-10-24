using EditorPlus;
using SeanLib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class WindEffector : MonoBehaviour
{
    [InspectorPlus.Range(0.01f,0.5f)]
    public float Strength=0.1f;
    [Header("Globle Wind")]
    public Texture2D GlobleWind;
    [InspectorPlus.Range(0.0f, 0.1f)]
    public float GlobleScale=0.01f;
    [Range(1, 3f)]
    public float Speed = 2f;
    void Update()
    {
        Vector3 v3 = this.transform.forward;
        Vector4 dir = new Vector4(v3.x, v3.y, v3.z, Strength);
        Shader.SetGlobalVector("_Wind", dir);
        Shader.SetGlobalVector("_GlobleWindParams", new Vector4(GlobleScale, Speed, 0,0));
        if(GlobleWind)
        Shader.SetGlobalTexture("_GlobleWind", GlobleWind);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        OnGizmos.ForGizmo(this.transform.position, this.transform.forward*3,1,20);
    }
}
