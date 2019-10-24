Shader "SeanLib/Instanced/Grass"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		[KeywordEnum(none,wind)] _Debug("Debug", int) = 0
	}
	SubShader
	{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
		Cull Off Lighting Off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//开启gpu instancing
			#pragma multi_compile_instancing


			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				half4 color : COLOR;
				//法线
				fixed3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID// necessary only if you want to access instanced properties in fragment Shader.
			};

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
			UNITY_INSTANCING_BUFFER_END(Props)
			sampler2D _MainTex;
			float4 _MainTex_ST;


			float _Cutoff;

			float4 _Wind;//风向
			//GlobleScale Speed
			uniform float4 _GlobleWindParams;

			uniform sampler2D _GlobleWind;
			
			v2f vert (appdata v, uint vid : SV_VertexID)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				v2f o;

				UNITY_TRANSFER_INSTANCE_ID(v, o);//necessary only if you want to access instanced properties in fragment Shader.
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);


				//世界坐标
				float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
				//计算全局uv
				half2 globleUV =-0.5+ posWorld.xz*_GlobleWindParams.x-_Time.x*_GlobleWindParams.y*_Wind.xz;

				//旋转全局uv
				float c = dot(_Wind.xyz, float3(0, 0, 1));
				float s = sin(sign(_Wind.x)*acos(c));
				//旋转矩阵公式
				globleUV = float2(globleUV.x * c - globleUV.y *s, globleUV.x * s + globleUV.y * c);

				half4 globleWind = tex2Dlod(_GlobleWind, float4(globleUV, 0, 0));

				float3 wind = mul((float3x3)unity_WorldToObject, _Wind.xyz);

				float fVtxPhase = dot(v.vertex.xyz, 0.5);
				v.vertex.xz += wind.xz*v.color.r*(sin(v.color.b*3.141592654*fVtxPhase + _Time.y*_GlobleWindParams.y)+ fVtxPhase)*_Wind.w*globleWind.r;

				o.vertex = UnityObjectToClipPos(v.vertex);
				///
				o.color = globleWind;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 col = tex2D(_MainTex, i.uv);
				clip(col.a - _Cutoff);
				return col*UNITY_ACCESS_INSTANCED_PROP(Props, _Color)+ i.color*0.2;
			}
			ENDCG
		}
	}
}
