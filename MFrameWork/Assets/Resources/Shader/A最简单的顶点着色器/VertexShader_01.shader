// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MxDemo/01_SimpleShader" 
{
	SubShader
	{
		pass
		{
			CGPROGRAM
			
			//下面两行指令告诉了Unity 那个函数包含了顶点着色器和片元着色器的代码
			#pragma vertex vert
			#pragma fragment frag
			//输入参数V输入的是所有顶点的位置信息
			float4 vert(float4 v : POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(v);
			}
			
			fixed4 frag() : SV_TARGET
			{
				return fixed4(1.0,1.0,1.0,1.0);
			}

			ENDCG
		}
	}

	Fallback "Defause"
}
