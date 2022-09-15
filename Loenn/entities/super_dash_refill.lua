local utils = require("utils")

local superDashRefill = {}

superDashRefill.name = "Anonhelper/SuperDashRefill"
superDashRefill.depth = -100
superDashRefill.placements = {
    {
        name = "super_dash_refill",
        data = {
            oneUse = false,
        }
    }
}
superDashRefill.texture = "objects/AnonHelper/superDashRefill/idle00"

function superDashRefill.selection(room, entity)
	local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 6, y - 4, 12, 7)
end

return superDashRefill