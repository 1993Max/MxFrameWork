// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MxDemo/SimpleShader_02" 
{
	SubShader
	{
		pass
		{
			CGPROGRAM
			
			//下面两行指令告诉了Unity 那个函数包含了顶点着色器和片元着色器的代码
			#pragma vertex vert
			#pragma fragment frag
			
			//使用一个结构体来定义顶点着色器的输入
			struct a2v
			{
				//POSITION语义告诉Unity 用模型的顶点坐标填充Vertex变量
				float4 vertex : POSITION;
				//Normal语义告诉Unity 用模型空间的法线方向填充normal变量
				float3 normal : NORMAL;
				//TEXCOORD0 语义告诉Unity 用模型的第一套文理坐标填充texcoord变量
				float4 texcoord : TEXCOORD0;
			};

			//用结构体a2v来访问模型空间的顶点坐标
			float4 vert(a2v v) : SV_POSITION
			{
				return UnityObjectToClipPos(v.vertex);
			}

			fixed4 frag() : SV_TARGET
			{
				return fixed4(0.5,0.6,0.7,0.8);
			}

			ENDCG
		}
	}

	Fallback "Defause"
}
