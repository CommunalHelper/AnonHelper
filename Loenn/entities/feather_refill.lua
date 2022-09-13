local utils = require("utils")

local featherRefill = {}

featherRefill.name = "Anonhelper/FeatherRefill"
featherRefill.depth = -100
featherRefill.placements = {
    {
        name = "feather_refill",
        data = {
            oneUse = false,
        }
    }
}
featherRefill.texture = "objects/AnonHelper/featherRefill/idle00"

function featherRefill.selection(room, entity)
	return utils.rectangle(entity.x - 4, entity.y - 6, 8, 12)
end

return featherRefill