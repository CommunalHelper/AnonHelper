module AnonhelperFeatherRefill

using ..Ahorn, Maple

@mapdef Entity "Anonhelper/FeatherRefill" FeatherRefill(x::Integer, y::Integer, oneUse::Bool=false)


const placements = Ahorn.PlacementDict(
    "Feather Refill (Anonhelper)" => Ahorn.EntityPlacement(
        FeatherRefill, 
	"point"
    )
)

sprite = "objects/featherRefill/idle00"

function getSprite(entity::FeatherRefill)

    return sprite
end

function Ahorn.selection(entity::FeatherRefill)
    oneUse = get(entity.data, "oneUse", false)
    x, y = Ahorn.position(entity)
    sprite = getSprite(entity)

    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::FeatherRefill, room::Maple.Room)
    sprite = getSprite(entity)
    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end