local utils = require("utils")

local windCloud = {}

windCloud.name = "Anonhelper/WindCloud"
windCloud.depth = 0
windCloud.placements = {
    {
        name = "normal",
        data = {
            fragile = false,
            small = false
        }
    },
    {
        name = "fragile",
        data = {
            fragile = true,
            small = false
        }
    }
}

function windCloud.texture(room, entity)
    local small = entity.small
    local fragile = entity.fragile

    if small then
        if fragile then
            return "objects/AnonHelper/clouds/windfragileRemix00"
        else
            return "objects/AnonHelper/clouds/windcloudRemix00"
        end
    else
        if fragile then
            return "objects/AnonHelper/clouds/windfragile00"
        else
            return "objects/AnonHelper/clouds/windcloud00"
        end
    end
end

function windCloud.selection(room, entity)
	local x, y = entity.x or 0, entity.y or 0
    if entity.small then
        return utils.rectangle(x - 13, y - 6, 24, 15)
    else
        return utils.rectangle(x - 18, y - 6, 36, 15)
    end
end

return windCloud