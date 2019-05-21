 -- @FileName Log
 -- @Create by mx
 -- @Create time 2019/05/21 19:53:38
 -- @FileInfo Lua层对Log函数做一个封装

 local function _formatStr(...)
    local l_arg = { ... }
    if not next(l_arg) then
        return
    elseif #l_arg==1 then
        return tostring(l_arg[1])
    end
    local str = ""
    local n = select('#', ...)
    local first = tostring(select(1, ...))
    local isFormat = first and string.find(first, "%{%d+%}")
    if isFormat then
        local args = {}
        for i = 2, n do
            table.insert(args, tostring(select(i, ...)))
        end
        str = StringEx.Format(first, unpack(args))
    else
        for i = 1, n do
            str = str .. ( n > 1 and '\t' or '') .. tostring(select(i, ...))
        end
    end
    return str
end

function Log(...)
    MLuaCommonHelper.Log("[lua] ".._formatStr(...).."\n[traceback] "..debug.traceback())
end

function LogError(...)
    MLuaCommonHelper.LogError("[lua] ".._formatStr(...).."\n[traceback] "..debug.traceback())
end