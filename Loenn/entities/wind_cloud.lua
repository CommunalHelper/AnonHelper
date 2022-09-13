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
    if entity.small then
        return utils.rectangle(entity.x - 13, entity.y - 6, 24, 15)
    else
        return utils.rectangle(entity.x - 18, entity.y - 6, 36, 15)
    end
end

return windCloud