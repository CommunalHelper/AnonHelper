local utils = require("utils")

local featherBumper = {}

featherBumper.name = "Anonhelper/FeatherBumper"
featherBumper.depth = 0
featherBumper.nodeLineRenderType = "line"
featherBumper.texture = "objects/AnonHelper/featherBumper/plugin"
featherBumper.nodeLimits = {0, 1}
featherBumper.placements = {
    name = "feather_bumper"
}

function featherBumper.selection(room, entity)
    local main = utils.rectangle(entity.x - 11 , entity.y - 11, 22, 22)

    if entity.nodes then
        local nodeSelections = {}
        for _, node in ipairs(entity.nodes) do
            table.insert(nodeSelections, utils.rectangle(node.x - 11, node.y -11, 22, 22))
        end
        return main, nodeSelections
    end

    return main, { }
end

return featherBumper