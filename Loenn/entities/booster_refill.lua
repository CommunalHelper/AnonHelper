local utils = require("utils")

local boostRefill = {}

boostRefill.name = "Anonhelper/BoosterRefill"
boostRefill.depth = -100
boostRefill.placements = {
    {
        name = "boost_refill",
        data = {
            oneUse = false,
            boostOnEnd = false,
        }
    }
}
function boostRefill.texture(room, entity)
    local red = entity.boostOnEnd

    if red then
        return "objects/AnonHelper/boosterRefillOnDash/idle00"

    else
        return "objects/AnonHelper/boosterRefill/idle00"
    end
end

function boostRefill.selection(room, entity)
	local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 5, y - 4, 10, 9)
end

return boostRefill
