local utils = require("utils")

local cloudRefill = {}

cloudRefill.name = "Anonhelper/CloudRefill"
cloudRefill.depth = -100
cloudRefill.placements = {
    {
        name = "cloud_refill",
        data = {
            oneUse = false,
        }
    }
}
cloudRefill.texture = "objects/AnonHelper/cloudRefill/idle00"

function cloudRefill.selection(room, entity)
	local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 5, y - 4, 11, 7)
end

return cloudRefill