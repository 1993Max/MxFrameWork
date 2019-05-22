 -- @FileName Main
 -- @Create by mx
 -- @Create time 2019/05/25 22:08:38
 -- @FileInfo 禁止隐式声明 访问全局变量

 --提供全局函数声明全局对象
function DeclareGlobal(name, intValue)
    if _G[name] then
        LogError(" please check u code, _G Exist Name："..name,"It is already exist")
    else
        rawset(_G, name, intValue or false)
    end
end

--提供全局函数访问全局对象
function GetGlobal(name)
    if _G[name] then
        return rawget(_G, name)
    else
        LogError(" please check u code, _G dont Exist Name："..name)
        return nil
    end
end

--给_G设置_newindex元表
setmetatable(
    _G,
    {
        __newindex = function(table, key, value)
            local calling_func = debug.getinfo(2)
            --对于module进行特殊处理
            if _G.module == calling_func.func then
                DeclareGlobal(key, value)
            else
                LogError("Attempt to write to undeclared global variable: " .. key )
            end
        end
    }
)