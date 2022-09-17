local oneUseBooster = {}

oneUseBooster.name = "Anonhelper/OneUseBooster"
oneUseBooster.depth = -8500
oneUseBooster.placements = {
    {
        name = "green",
        data = {
            red = false
        }
    },
    {
        name = "red",
        data = {
            red = true
        }
    }
}

function oneUseBooster.texture(room, entity)
    local red = entity.red

    if red then
        return "objects/booster/boosterRed00"
    else
        return "objects/booster/booster00"
    end
end

return oneUseBooster