--<<Create By Mx>>--
--<<主入口函数>>--

-- lua垃圾回收设置 避免内存泄露 设置内存回收参数
collectgarbage("setpause", 100)
collectgarbage("setstepmul", 5000)

--主入口函数。从这里开始lua逻辑
function Main()					
	print("logic start")	 		
end

--Lua 逻辑入口
function Start()
	
end

function Close()
	
end

--场景切换内存回收
function OnLevelWasLoaded(level)
	collectgarbage("collect")
end

function ShowLog(str)
	
end

print("This is a script from a utf8 file")

