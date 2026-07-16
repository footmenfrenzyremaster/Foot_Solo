-- Entry point for future Lua systems.
--
-- Warcraft III maps need a build/import step before these modules affect the
-- packed map. For now, this folder is source-control-friendly design space.

local heroes = require("src.data.heroes")
local taverns = require("src.data.taverns")
local tavern_pool = require("src.systems.tavern_pool")

local M = {}

function M.init()
    tavern_pool.configure({
        heroes = heroes,
        taverns = taverns,
    })
end

return M
