﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class MFrameWork_MBaseSingletonWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(MFrameWork.MBaseSingleton), typeof(System.Object));
		L.RegFunction("Init", Init);
		L.RegFunction("UnInit", UnInit);
		L.RegFunction("OnLogOut", OnLogOut);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			MFrameWork.MBaseSingleton obj = (MFrameWork.MBaseSingleton)ToLua.CheckObject<MFrameWork.MBaseSingleton>(L, 1);
			bool o = obj.Init();
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UnInit(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			MFrameWork.MBaseSingleton obj = (MFrameWork.MBaseSingleton)ToLua.CheckObject<MFrameWork.MBaseSingleton>(L, 1);
			obj.UnInit();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnLogOut(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			MFrameWork.MBaseSingleton obj = (MFrameWork.MBaseSingleton)ToLua.CheckObject<MFrameWork.MBaseSingleton>(L, 1);
			obj.OnLogOut();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

