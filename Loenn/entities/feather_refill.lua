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
	local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 4, y - 6, 8, 12)
end

return featherRefill