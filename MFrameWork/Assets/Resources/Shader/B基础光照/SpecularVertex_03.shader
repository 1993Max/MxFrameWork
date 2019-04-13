// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
学习光照部分遇到的名词
辐射度 用于量化光
光线由光源发射 就会和物体相交 结果通常有两个 散射和吸收
散射只改变光线的方向 不改变光照的密度和颜色
吸收只改变光线的密度和颜色 不改变光线的方向

标准光照模型
自发光（emissive）
高光反射（specular）
漫反射（diffuse）
环境光（ambient）
*/

//Shader实现逐个顶点的高光反射
Shader "MxDemo/SpecularVertex_03" 
{
	Properties
	{
		//申明并用于控制漫反射的颜色
		_Diffuse("Diffuse",Color) = (1,1,1,1)
		//申明控制材质高光反射的颜色
		_Specular("Specular",Color) = (1,1,1,1)
		//用于控制材质高光反射的区域
		_Gloss("Gloss",Range(8.0,256)) = 20

	}

	SubShader
	{
		pass
		{
			Tags {"LightMode" = "ForwardBase"}
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"

			fixed4 _Diffuse;
			fixed4 _Specular;
			float _Gloss;

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				fixed3 color : COLOR;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				//通过Unity的内置函数得到环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				//得到世界空间的法线
				fixed3 worldNormal = normalize(mul(v.normal,(float3x3)unity_WorldToObject));
				//得到世界空间的方向
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

				//计算漫反射
				fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal,worldLightDir));

				fixed3 reflectDir = normalize(reflect(-worldLightDir,worldNormal));

				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld,v.vertex).xyz);

				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(reflectDir,viewDir)),_Gloss);

				o.color = ambient + diffuse + specular;

				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET
			{
				return fixed4(i.color,1);
			}

			ENDCG
		}
	}
	Fallback "Specular"
}
