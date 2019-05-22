﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class MFrameWork_MPathUtilsWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(MFrameWork.MPathUtils), typeof(System.Object));
		L.RegFunction("New", _CreateMFrameWork_MPathUtils);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("ASSETBUNDLE_PATH", get_ASSETBUNDLE_PATH, set_ASSETBUNDLE_PATH);
		L.RegVar("ASSETBUNDLE_CONFIG_PATH", get_ASSETBUNDLE_CONFIG_PATH, set_ASSETBUNDLE_CONFIG_PATH);
		L.RegVar("ASSETBUNDLE_XML_PATH", get_ASSETBUNDLE_XML_PATH, set_ASSETBUNDLE_XML_PATH);
		L.RegVar("ASSETBUNDLE_BYTES_PATH", get_ASSETBUNDLE_BYTES_PATH, set_ASSETBUNDLE_BYTES_PATH);
		L.RegVar("ASSETBUNDLE_AB_DATA_NAME", get_ASSETBUNDLE_AB_DATA_NAME, set_ASSETBUNDLE_AB_DATA_NAME);
		L.RegVar("ASSETBUNDLE_AB_BYTES_NAME", get_ASSETBUNDLE_AB_BYTES_NAME, set_ASSETBUNDLE_AB_BYTES_NAME);
		L.RegVar("RECYCLE_POOL_TRANSFORM", get_RECYCLE_POOL_TRANSFORM, null);
		L.RegVar("DEFAULT_OBJECT_TRANSFORM", get_DEFAULT_OBJECT_TRANSFORM, null);
		L.RegVar("UI_MAINPATH", get_UI_MAINPATH, null);
		L.RegVar("UI_ROOTPATH", get_UI_ROOTPATH, null);
		L.RegVar("UI_PREFAB_SUFFIX", get_UI_PREFAB_SUFFIX, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateMFrameWork_MPathUtils(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				MFrameWork.MPathUtils obj = new MFrameWork.MPathUtils();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: MFrameWork.MPathUtils.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ASSETBUNDLE_PATH(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, MFrameWork.MPathUtils.ASSETBUNDLE_PATH);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ASSETBUNDLE_CONFIG_PATH(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, MFrameWork.MPathUtils.ASSETBUNDLE_CONFIG_PATH);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ASSETBUNDLE_XML_PATH(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, MFrameWork.MPathUtils.ASSETBUNDLE_XML_PATH);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ASSETBUNDLE_BYTES_PATH(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, MFrameWork.MPathUtils.ASSETBUNDLE_BYTES_PATH);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ASSETBUNDLE_AB_DATA_NAME(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, MFrameWork.MPathUtils.ASSETBUNDLE_AB_DATA_NAME);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ASSETBUNDLE_AB_BYTES_NAME(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, MFrameWork.MPathUtils.ASSETBUNDLE_AB_BYTES_NAME);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_RECYCLE_POOL_TRANSFORM(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, MFrameWork.MPathUtils.RECYCLE_POOL_TRANSFORM);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_DEFAULT_OBJECT_TRANSFORM(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, MFrameWork.MPathUtils.DEFAULT_OBJECT_TRANSFORM);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_UI_MAINPATH(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, MFrameWork.MPathUtils.UI_MAINPATH);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_UI_ROOTPATH(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, MFrameWork.MPathUtils.UI_ROOTPATH);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_UI_PREFAB_SUFFIX(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushstring(L, MFrameWork.MPathUtils.UI_PREFAB_SUFFIX);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ASSETBUNDLE_PATH(IntPtr L)
	{
		try
		{
			string arg0 = ToLua.CheckString(L, 2);
			MFrameWork.MPathUtils.ASSETBUNDLE_PATH = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ASSETBUNDLE_CONFIG_PATH(IntPtr L)
	{
		try
		{
			string arg0 = ToLua.CheckString(L, 2);
			MFrameWork.MPathUtils.ASSETBUNDLE_CONFIG_PATH = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ASSETBUNDLE_XML_PATH(IntPtr L)
	{
		try
		{
			string arg0 = ToLua.CheckString(L, 2);
			MFrameWork.MPathUtils.ASSETBUNDLE_XML_PATH = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ASSETBUNDLE_BYTES_PATH(IntPtr L)
	{
		try
		{
			string arg0 = ToLua.CheckString(L, 2);
			MFrameWork.MPathUtils.ASSETBUNDLE_BYTES_PATH = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ASSETBUNDLE_AB_DATA_NAME(IntPtr L)
	{
		try
		{
			string arg0 = ToLua.CheckString(L, 2);
			MFrameWork.MPathUtils.ASSETBUNDLE_AB_DATA_NAME = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ASSETBUNDLE_AB_BYTES_NAME(IntPtr L)
	{
		try
		{
			string arg0 = ToLua.CheckString(L, 2);
			MFrameWork.MPathUtils.ASSETBUNDLE_AB_BYTES_NAME = arg0;
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

