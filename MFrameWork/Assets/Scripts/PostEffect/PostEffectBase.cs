using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFrameWork;

//在编辑器下也可以运行
[ExecuteInEditMode]
//屏幕后处理夜宵一般都需要绑定在相机上
[RequireComponent(typeof(Camera))]

public class PostEffectBase : MonoBehaviour {

    public Shader shader;
    private Material _material;

    private void Start()
    {
        CheckResource();
    }

    protected void CheckResource()
    {
        if (!CheckSupport())
            NotSupported();
    }

    protected bool CheckSupport()
    {
        if(SystemInfo.supportsImageEffects == false)
        {
            MDebug.singleton.AddErrorLog("This PlatForm Does not support ImageEffects");
            return false;
        }
        return true;
    }

    protected void NotSupported(){
        enabled = false;        
    }

    public Material material
    {
        get
        {
            if(_material==null)
            {
                _material = GeneGenerateMaterial(shader);
            }
            return _material;
        }
    }

    protected Material GeneGenerateMaterial(Shader setShader)
    {
        if(setShader == null)
        {
            return null;
        }
        if(!setShader.isSupported)
        {
            return null;
        }

        Material material = new Material(setShader);
        material.hideFlags = HideFlags.DontSave;
        return material;
    }
}
