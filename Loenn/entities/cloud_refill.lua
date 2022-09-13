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
    return utils.rectangle(entity.x - 5, entity.y - 4, 11, 7)
end

return cloudRefill