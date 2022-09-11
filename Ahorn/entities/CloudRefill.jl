module AnonhelperCloudRefill

using ..Ahorn, Maple

@mapdef Entity "Anonhelper/CloudRefill" CloudRefill(x::Integer, y::Integer, oneUse::Bool=false)


const placements = Ahorn.PlacementDict(
    "CloudRefill (Anonhelper)" => Ahorn.EntityPlacement(
        CloudRefill, 
	"point"
    )
)

sprite = "objects/cloudRefill/idle00"

function getSprite(entity::CloudRefill)

    return sprite
end

function Ahorn.selection(entity::CloudRefill)
    oneUse = get(entity.data, "oneUse", false)
    x, y = Ahorn.position(entity)
    sprite = getSprite(entity)

    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::CloudRefill, room::Maple.Room)
    sprite = getSprite(entity)
    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end