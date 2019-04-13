// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//学习顶点数据和片元之间的数据通信
Shader "MxDemo/SimpleShader_03" 
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

			struct v2f
			{
				//SV_POSITION语义告诉Unity Pos里面包含了顶点在裁剪空间中的位置信息
				float4 pos : SV_POSITION;
				//COLOR0可以存储颜色信息
				fixed3 color : COLOR0;
			};

			//顶点着色器的输出语义中必须包含一个输出结构 SV_POSITION 否则渲染器将无法得到裁剪空间中的顶点坐标 
			//用结构体a2v来访问模型空间的顶点坐标
			v2f vert(a2v v)
			{
				//声明输出结构体
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				//v.Normal包含了顶点的法线方向 它的分量范围在{-1，1}
				//下面代码把Color的范围映射到了{0,1}之间
				//存储o.color传递给片元着色器
				o.color = v.normal * 0.5 + fixed3(0.5,0.5,0.5);
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET
			{
				return fixed4(i.color,1);
			}

			ENDCG
		}
	}

	Fallback "Defause"
}
