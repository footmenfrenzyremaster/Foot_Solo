-- Future tavern hero population system.

local M = {}

local state = {
    heroes = nil,
    taverns = nil,
}

function M.configure(config)
    state.heroes = config.heroes
    state.taverns = config.taverns
end

function M.pick_random_unique(pool, count, rng)
    rng = rng or math.random
    local available = {}
    for i, hero in ipairs(pool) do
        available[i] = hero
    end

    local selected = {}
    while #selected < count and #available > 0 do
        local index = rng(#available)
        selected[#selected + 1] = available[index]
        table.remove(available, index)
    end

    return selected
end

function M.populate_all_taverns()
    -- TODO: Implement after tavern rawcodes and current stock rules are mapped.
end

return M
