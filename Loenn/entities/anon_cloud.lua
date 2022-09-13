local utils = require("utils")

local anonCloud = {}

anonCloud.name = "Anonhelper/AnonCloud"
anonCloud.depth = 0
anonCloud.placements = {
    {
        name = "normal",
        data = {
            pink = false,
            small = false
        }
    },
    {
        name = "fragile",
        data = {
            pink = true,
            small = false
        }
    }
}

function anonCloud.texture(room, entity)
    local small = entity.small
    local fragile = entity.pink

    if small then
        if fragile then
            return "objects/AnonHelper/clouds/pinkcloudRemix00"
        else
            return "objects/AnonHelper/clouds/whitecloudRemix00"
        end
    else
        if fragile then
            return "objects/AnonHelper/clouds/pinkcloud00"
        else
            return "objects/AnonHelper/clouds/whitecloud00"
        end
    end
end

function anonCloud.selection(room, entity)
    if entity.small then
        return utils.rectangle(entity.x - 13, entity.y - 6, 24, 15)
    else
        return utils.rectangle(entity.x - 18, entity.y - 6, 36, 15)
    end
end

return anonCloud