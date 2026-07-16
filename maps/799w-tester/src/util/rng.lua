-- Random helpers for future systems.

local M = {}

function M.choice(list)
    if #list == 0 then
        return nil
    end
    return list[math.random(#list)]
end

return M
