//这个Shader 可以用于后处理
//主要可以设置 材质的亮度 饱和度 对比度
Shader "MxDemo/ColorSet" 
{
	Properties
	{
		_MainTex("Base(RGB)",2D) = "write" {}
		//亮度
		_Brightness("Brightness", Float) = 1
		//饱和度
		_Saturation("Saturation", Float) = 1
		//对比度
		_Contrast("Contrast", Float) = 1
	}
	SubShader
	{
		pass
		{
			//屏幕后处理渲染设置的标配
			//关闭深度写入，防止挡住在其后面被渲染的物体
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
		    half _Brightness;
			half _Saturation;
			half _Contrast;

			struct v2f{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};
			
			//顶点着色器用于赋值顶点坐标和Uv信息
			v2f vert(appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			//片元着色器处理颜色显示
			fixed4 frag(v2f i) : SV_TARGET
			{
				//对原屏幕图像进行采样
				fixed4 renderTex = tex2D(_MainTex, i.uv);
			    //调整亮度
			    //原颜色乘以亮度系数
			    fixed3 finalColor = renderTex.rgb * _Brightness;

				//调整饱和度
				//计算该像素对应的亮度值，每一个颜色分量乘以一个特定的系数值再相加
				fixed luminance = 0.2125 * renderTex.r + 0.7154 * renderTex.g + 0.0721 * renderTex.b;
				//创建一个饱和度为0的颜色值
				fixed3 luminanceColor = fixed3(luminance, luminance, luminance);
				//使用_Saturation和其上一步得到的颜色之间进行插值，得到希望的饱和度
				finalColor = lerp(luminanceColor, finalColor, _Saturation);

				//调整对比度
				//创建一个对比度为0的颜色值，每个分量均为0.5
				fixed3 avgColor = fixed3(0.5, 0.5, 0.5);
				//使用_Contrast在其和上一步得到的颜色之间进行插值
				finalColor = lerp(avgColor, finalColor, _Contrast);

				return fixed4(finalColor, renderTex.a);
			}
			ENDCG
		}
	}

	Fallback "Off"
}
