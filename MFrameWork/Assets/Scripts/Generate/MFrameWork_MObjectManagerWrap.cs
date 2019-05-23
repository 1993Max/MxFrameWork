﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class MFrameWork_MObjectManagerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(MFrameWork.MObjectManager), typeof(MFrameWork.MSingleton<MFrameWork.MObjectManager>));
		L.RegFunction("InitASyncObjectManager", InitASyncObjectManager);
		L.RegFunction("CancleAsyncResObjectLoad", CancleAsyncResObjectLoad);
		L.RegFunction("IsInAsyncLoad", IsInAsyncLoad);
		L.RegFunction("IsObjectManagerCreate", IsObjectManagerCreate);
		L.RegFunction("InstantiateGameObejectAsync", InstantiateGameObejectAsync);
		L.RegFunction("Init", Init);
		L.RegFunction("OnLogOut", OnLogOut);
		L.RegFunction("UnInit", UnInit);
		L.RegFunction("InitObjectManager", InitObjectManager);
		L.RegFunction("InitSyncObjectManager", InitSyncObjectManager);
		L.RegFunction("ClearCatch", ClearCatch);
		L.RegFunction("ClearPoolObject", ClearPoolObject);
		L.RegFunction("PreLoadGameObject", PreLoadGameObject);
		L.RegFunction("InstantiateGameObeject", InstantiateGameObeject);
		L.RegFunction("GetObjectFromPool", GetObjectFromPool);
		L.RegFunction("ReleaseObjectComopletly", ReleaseObjectComopletly);
		L.RegFunction("ReleaseObject", ReleaseObject);
		L.RegFunction("GetObjOffLineData", GetObjOffLineData);
		L.RegFunction("New", _CreateMFrameWork_MObjectManager);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("m_asyncResourcesObjectsDic", get_m_asyncResourcesObjectsDic, set_m_asyncResourcesObjectsDic);
		L.RegVar("RecycleObjectPoolTrans", get_RecycleObjectPoolTrans, null);
		L.RegVar("DefaultObjectTrans", get_DefaultObjectTrans, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateMFrameWork_MObjectManager(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				MFrameWork.MObjectManager obj = new MFrameWork.MObjectManager();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: MFrameWork.MObjectManager.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitASyncObjectManager(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
			bool o = obj.InitASyncObjectManager();
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CancleAsyncResObjectLoad(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
			long arg0 = LuaDLL.tolua_checkint64(L, 2);
			obj.CancleAsyncResObjectLoad(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsInAsyncLoad(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
			long arg0 = LuaDLL.tolua_checkint64(L, 2);
			bool o = obj.IsInAsyncLoad(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IsObjectManagerCreate(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
			bool o = obj.IsObjectManagerCreate(arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InstantiateGameObejectAsync(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 4)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				MFrameWork.OnAsyncLoadObjectFinished arg1 = (MFrameWork.OnAsyncLoadObjectFinished)ToLua.CheckDelegate<MFrameWork.OnAsyncLoadObjectFinished>(L, 3);
				MFrameWork.LoadResPriority arg2 = (MFrameWork.LoadResPriority)ToLua.CheckObject(L, 4, typeof(MFrameWork.LoadResPriority));
				long o = obj.InstantiateGameObejectAsync(arg0, arg1, arg2);
				LuaDLL.tolua_pushint64(L, o);
				return 1;
			}
			else if (count == 5)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				MFrameWork.OnAsyncLoadObjectFinished arg1 = (MFrameWork.OnAsyncLoadObjectFinished)ToLua.CheckDelegate<MFrameWork.OnAsyncLoadObjectFinished>(L, 3);
				MFrameWork.LoadResPriority arg2 = (MFrameWork.LoadResPriority)ToLua.CheckObject(L, 4, typeof(MFrameWork.LoadResPriority));
				bool arg3 = LuaDLL.luaL_checkboolean(L, 5);
				long o = obj.InstantiateGameObejectAsync(arg0, arg1, arg2, arg3);
				LuaDLL.tolua_pushint64(L, o);
				return 1;
			}
			else if (count == 6)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				MFrameWork.OnAsyncLoadObjectFinished arg1 = (MFrameWork.OnAsyncLoadObjectFinished)ToLua.CheckDelegate<MFrameWork.OnAsyncLoadObjectFinished>(L, 3);
				MFrameWork.LoadResPriority arg2 = (MFrameWork.LoadResPriority)ToLua.CheckObject(L, 4, typeof(MFrameWork.LoadResPriority));
				bool arg3 = LuaDLL.luaL_checkboolean(L, 5);
				object[] arg4 = ToLua.CheckObjectArray(L, 6);
				long o = obj.InstantiateGameObejectAsync(arg0, arg1, arg2, arg3, arg4);
				LuaDLL.tolua_pushint64(L, o);
				return 1;
			}
			else if (count == 7)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				MFrameWork.OnAsyncLoadObjectFinished arg1 = (MFrameWork.OnAsyncLoadObjectFinished)ToLua.CheckDelegate<MFrameWork.OnAsyncLoadObjectFinished>(L, 3);
				MFrameWork.LoadResPriority arg2 = (MFrameWork.LoadResPriority)ToLua.CheckObject(L, 4, typeof(MFrameWork.LoadResPriority));
				bool arg3 = LuaDLL.luaL_checkboolean(L, 5);
				object[] arg4 = ToLua.CheckObjectArray(L, 6);
				bool arg5 = LuaDLL.luaL_checkboolean(L, 7);
				long o = obj.InstantiateGameObejectAsync(arg0, arg1, arg2, arg3, arg4, arg5);
				LuaDLL.tolua_pushint64(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: MFrameWork.MObjectManager.InstantiateGameObejectAsync");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
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
	static int OnLogOut(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
			obj.OnLogOut();
			return 0;
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
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
			obj.UnInit();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitObjectManager(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
			bool o = obj.InitObjectManager();
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InitSyncObjectManager(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
			bool o = obj.InitSyncObjectManager();
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClearCatch(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
			obj.ClearCatch();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClearPoolObject(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
			uint arg0 = (uint)LuaDLL.luaL_checknumber(L, 2);
			obj.ClearPoolObject(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PreLoadGameObject(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				obj.PreLoadGameObject(arg0);
				return 0;
			}
			else if (count == 3)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				obj.PreLoadGameObject(arg0, arg1);
				return 0;
			}
			else if (count == 4)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				bool arg2 = LuaDLL.luaL_checkboolean(L, 4);
				obj.PreLoadGameObject(arg0, arg1, arg2);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: MFrameWork.MObjectManager.PreLoadGameObject");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InstantiateGameObeject(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				UnityEngine.GameObject o = obj.InstantiateGameObeject(arg0);
				ToLua.PushSealed(L, o);
				return 1;
			}
			else if (count == 3)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				bool arg1 = LuaDLL.luaL_checkboolean(L, 3);
				UnityEngine.GameObject o = obj.InstantiateGameObeject(arg0, arg1);
				ToLua.PushSealed(L, o);
				return 1;
			}
			else if (count == 4)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				string arg0 = ToLua.CheckString(L, 2);
				bool arg1 = LuaDLL.luaL_checkboolean(L, 3);
				bool arg2 = LuaDLL.luaL_checkboolean(L, 4);
				UnityEngine.GameObject o = obj.InstantiateGameObeject(arg0, arg1, arg2);
				ToLua.PushSealed(L, o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: MFrameWork.MObjectManager.InstantiateGameObeject");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetObjectFromPool(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
			uint arg0 = (uint)LuaDLL.luaL_checknumber(L, 2);
			MFrameWork.MResourceObjectItem o = obj.GetObjectFromPool(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReleaseObjectComopletly(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
			obj.ReleaseObjectComopletly(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ReleaseObject(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
				obj.ReleaseObject(arg0);
				return 0;
			}
			else if (count == 3)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				obj.ReleaseObject(arg0, arg1);
				return 0;
			}
			else if (count == 4)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				bool arg2 = LuaDLL.luaL_checkboolean(L, 4);
				obj.ReleaseObject(arg0, arg1, arg2);
				return 0;
			}
			else if (count == 5)
			{
				MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
				UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				bool arg2 = LuaDLL.luaL_checkboolean(L, 4);
				bool arg3 = LuaDLL.luaL_checkboolean(L, 5);
				obj.ReleaseObject(arg0, arg1, arg2, arg3);
				return 0;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to method: MFrameWork.MObjectManager.ReleaseObject");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetObjOffLineData(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)ToLua.CheckObject<MFrameWork.MObjectManager>(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
			MFrameWork.MResOffLineDataBase o = obj.GetObjOffLineData(arg0);
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_asyncResourcesObjectsDic(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)o;
			System.Collections.Generic.Dictionary<long,MFrameWork.MResourceObjectItem> ret = obj.m_asyncResourcesObjectsDic;
			ToLua.PushSealed(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index m_asyncResourcesObjectsDic on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_RecycleObjectPoolTrans(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)o;
			UnityEngine.Transform ret = obj.RecycleObjectPoolTrans;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index RecycleObjectPoolTrans on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_DefaultObjectTrans(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)o;
			UnityEngine.Transform ret = obj.DefaultObjectTrans;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index DefaultObjectTrans on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_asyncResourcesObjectsDic(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			MFrameWork.MObjectManager obj = (MFrameWork.MObjectManager)o;
			System.Collections.Generic.Dictionary<long,MFrameWork.MResourceObjectItem> arg0 = (System.Collections.Generic.Dictionary<long,MFrameWork.MResourceObjectItem>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.Dictionary<long,MFrameWork.MResourceObjectItem>));
			obj.m_asyncResourcesObjectsDic = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index m_asyncResourcesObjectsDic on a nil value");
		}
	}
}
