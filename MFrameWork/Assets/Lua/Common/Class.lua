function Class(className, ...)
    local cls = {__cname = className}

    local supers = {...}
    for _, super in ipairs(supers) do
        local superType = type(super)
        assert(superType == "nil" or superType == "table" or superType == "function",
            string.format("Class() - create Class \"%s\" with invalid super Class type \"%s\"",
                className, superType))

        if superType == "function" then
            assert(cls.__create == nil,
                string.format("Class() - create Class \"%s\" with more than one creating function",
                    className));
            -- if super is function, set it to __create
            cls.__create = super
        elseif superType == "table" then
            if super[".isClass"] then
                -- super is native Class
                assert(cls.__create == nil,
                    string.format("Class() - create Class \"%s\" with more than one creating function or native Class",
                        className));
                cls.__create = function() return super:create() end
            else
                -- super is pure lua Class
                cls.__supers = cls.__supers or {}
                cls.__supers[#cls.__supers + 1] = super
                if not cls.super then
                    -- set first super pure lua Class as Class.super
                    cls.super = super
                end
            end
        else
            error(string.format("Class() - create Class \"%s\" with invalid super type",
                        className), 0)
        end
    end

    cls.__index = cls
    if not cls.__supers or #cls.__supers == 1 then
        setmetatable(cls, {__index = cls.super})
    else
        setmetatable(cls, {__index = function(_, key)
            local supers = cls.__supers
            for i = 1, #supers do
                local super = supers[i]
                if super[key] then return super[key] end
            end
        end})
    end

    if not cls.Ctor then
        -- add default construCtor
        cls.Ctor = function() end
    end
    cls.New = function(...)
        local instance
        if cls.__create then
            instance = cls.__create(...)
        else
            instance = {}
        end
        setmetatableindex(instance, cls)
        instance.Class = cls
        instance:Ctor(...)
        return instance
    end
    cls.create = function(_, ...)
        return cls.New(...)
    end

    return cls
end

local setmetatableindex_
setmetatableindex_ = function(t, index)
    if type(t) == "userdata" then
        local peer = tolua.getpeer(t)
        if not peer then
            peer = {}
            tolua.setpeer(t, peer)
        end
        setmetatableindex_(peer, index)
    else
        local mt = getmetatable(t)
        if not mt then mt = {} end
        if not mt.__index then
            mt.__index = index
            setmetatable(t, mt)
        elseif mt.__index ~= index then
            setmetatableindex_(mt, index)
        end
    end
end
setmetatableindex = setmetatableindex_