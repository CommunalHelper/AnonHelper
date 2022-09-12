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

return anonCloud