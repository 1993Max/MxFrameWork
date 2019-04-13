// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//学习如何使用属性
Shader "MxDemo/SimpleShader_03" 
{
	Properties
	{
		//申明一个Color类型的属性
		_Color ("Color Tint",Color) = (1,1,1,1)
	}

	SubShader
	{
		pass
		{
			CGPROGRAM
			
			//下面两行指令告诉了Unity 那个函数包含了顶点着色器和片元着色器的代码
			#pragma vertex vert
			#pragma fragment frag
			
			//在CG代码中 我们需要定义一个与属性名称和类名称都匹配的变量
			fixed4 _Color;

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
				fixed3 c = i.color;
				//使用_Color属性来控制输出的颜色
				c *= _Color.rgb;
				return fixed4(c,1);
			}

			ENDCG
		}
	}

	Fallback "Defause"
}

/*
应用阶段传递模型数据给顶点着色器时候 Unity常用的语义
POSITION 模型空间中的顶点位置
NORMAL 顶点的法线 float3类型
TANGENT 顶点的切线 float4类型
COLOR 顶点的颜色 通常是fiexd4或float4类型
TEXCOORD 该顶点的纹理坐标 TEXCOORD0表示第一组纹理坐标 通常是float2或者float4类型
*/

从顶点着色器传给片元着色器Unity使用的常用语义
SV_POSITION 裁剪空间的顶点坐标 结构体中必须包含一个用该语义修饰的变量
COLOR0 通常用于输出第一组顶点的颜色 非必须
COLOR1 通常用于输出第二组顶点的颜色 非必须
TEXCOORD 通常用于输出的纹理坐标 不是必须的

片元着色器输出到Unity支持的常用语义


