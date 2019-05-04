using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//调整高光 饱和度 对比度
public class ColorAdjustEffect : PostEffectBase 
{
    [Range(0f, 3.0f)]
    public float brightness = 1.0f;
    [Range(0f, 3.0f)]
    public float saturation = 1.0f;
    [Range(0f, 3.0f)]
    public float contrast = 1.0f;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(material != null)
        {
            material.SetFloat("_Brightness", brightness);
            material.SetFloat("_Saturation", saturation);
            material.SetFloat("_Contrast", contrast);

            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
