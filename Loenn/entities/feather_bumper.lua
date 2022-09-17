local utils = require("utils")

local featherBumper = {}

featherBumper.name = "Anonhelper/FeatherBumper"
featherBumper.depth = 0
featherBumper.nodeLineRenderType = "line"
featherBumper.texture = "objects/AnonHelper/featherBumper/Idle22"
featherBumper.nodeLimits = {0, 1}
featherBumper.placements = {
    name = "feather_bumper",
    data = {
        wobble = true,
    }
}

function featherBumper.selection(room, entity)
	local ex, ey = entity.x or 0, entity.y or 0
    local main = utils.rectangle(ex - 11 , ey - 11, 22, 22)

    if entity.nodes then
        local nodeSelections = {}
        for _, node in ipairs(entity.nodes) do
			local nx, ny = entity.x or 0, entity.y or 0
            table.insert(nodeSelections, utils.rectangle(nx - 11, ny -11, 22, 22))
        end
        return main, nodeSelections
    end

    return main, { }
end

return featherBumper