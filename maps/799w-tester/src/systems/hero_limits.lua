-- Future hero limit / sell / repick rules.

local M = {}

function M.mark_unavailable(player, hero_rawcode)
    -- Warcraft III implementation target:
    -- SetPlayerTechMaxAllowed(player, FourCC(hero_rawcode), 0)
end

return M
