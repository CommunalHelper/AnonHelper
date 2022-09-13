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
	return utils.rectangle(entity.x - 5, entity.y - 4, 10, 9)
end

return jellyRefill