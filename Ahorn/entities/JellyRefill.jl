module AnonhelperJellyRefill

using ..Ahorn, Maple

@mapdef Entity "Anonhelper/JellyRefill" JellyRefill(x::Integer, y::Integer, oneUse::Bool=false)


const placements = Ahorn.PlacementDict(
    "JellyRefill (Anonhelper)" => Ahorn.EntityPlacement(
        JellyRefill, 
	"point"
    )
)

sprite = "objects/AnonHelper/jellyRefill/idle00"

function getSprite(entity::JellyRefill)

    return sprite
end

function Ahorn.selection(entity::JellyRefill)
    oneUse = get(entity.data, "oneUse", false)
    x, y = Ahorn.position(entity)
    sprite = getSprite(entity)

    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::JellyRefill, room::Maple.Room)
    sprite = getSprite(entity)
    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end