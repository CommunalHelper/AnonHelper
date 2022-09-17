local utils = require("utils")

local jellyRefill = {}

jellyRefill.name = "Anonhelper/JellyRefill"
jellyRefill.depth = -100
jellyRefill.placements = {
    {
        name = "jelly_refill",
        data = {
            oneUse = false,
        }
    }
}
jellyRefill.texture = "objects/AnonHelper/jellyRefill/idle00"

function jellyRefill.selection(room, entity)
	local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 5, y - 4, 10, 9)
end

return jellyRefill