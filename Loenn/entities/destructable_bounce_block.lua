local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableSprite = require("structs.drawable_sprite")

local destructableBounceBlock = {}

destructableBounceBlock.name = "Anonhelper/DestructableBounceBlock"
destructableBounceBlock.depth = 8990
destructableBounceBlock.minimumSize = {16, 16}
destructableBounceBlock.placements = {
    {
        name = "fire",
        data = {
            width = 16,
            height = 16,
            notCoreMode = false
        }
    },
    {
        name = "ice",
        data = {
            width = 16,
            height = 16,
            notCoreMode = true
        }
    },
}

local ninePatchOptions = {
    mode = "fill",
    borderMode = "repeat",
    fillMode = "repeat"
}

local fireBlockTexture = "objects/AnonHelper/bumpBlockNew/fire00"
local fireCrystalTexture = "objects/AnonHelper/bumpBlockNew/fire_center00"

local iceBlockTexture = "objects/AnonHelper/bumpBlockNew/ice00"
local iceCrystalTexture = "objects/AnonHelper/bumpBlockNew/ice_center00"

local function getBlockTexture(entity)
    if entity.notCoreMode or entity.iceMode then
        return iceBlockTexture, iceCrystalTexture
    else
        return fireBlockTexture, fireCrystalTexture
    end
end

function destructableBounceBlock.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, entity.height or 24

    local blockTexture, crystalTexture = getBlockTexture(entity)

    local ninePatch = drawableNinePatch.fromTexture(blockTexture, ninePatchOptions, x, y, width, height)
    local crystalSprite = drawableSprite.fromTexture(crystalTexture, entity)
    local sprites = ninePatch:getDrawableSprite()

    crystalSprite:addPosition(math.floor(width / 2), math.floor(height / 2))
    table.insert(sprites, crystalSprite)

    return sprites
end

return destructableBounceBlock