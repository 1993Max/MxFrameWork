// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

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

//Shader实现逐像素光照 可以实现更平滑的光照想过
Shader "MxDemo/DiffusePixelLevel_02" 
{
	Properties
	{
		//申明并用于控制漫反射的颜色
		_Diffuse("Diffuse",Color) = (1,1,1,1)
	}

	SubShader
	{
		pass
		{
			//LightMode标签是Pass标签的一种 用于定义改Pass在流水线中的角色
			Tags {"LightMode" = "ForwardBase"}
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"

			fixed4 _Diffuse;

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
			};

			v2f vert(a2v v)
			{
				v2f o;
				//顶点位置转换 模型空间转换到裁剪空间
				o.pos = UnityObjectToClipPos(v.vertex);
				//法线空间转换 从模型空间法线转换为世界空间法线
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);

				return o; 
			}

			fixed4 frag(v2f i) : SV_TARGET
			{
				//通过Unity的内置函数得到环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				//得到世界空间的法线方向
				fixed3 worldNormal = normalize(i.worldNormal);
				//获取光照在世界空间的方向
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				
				fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal,worldLightDir));

				fixed3 color = ambient + diffuse;

				return fixed4(color,1.0);
			}

			ENDCG
		}
	}
	Fallback "Diffuse"
}
