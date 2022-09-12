module AnonhelperSuperDashRefill

using ..Ahorn, Maple

@mapdef Entity "Anonhelper/SuperDashRefill" SuperDashRefill(x::Integer, y::Integer, oneUse::Bool=false)


const placements = Ahorn.PlacementDict(
    "Super Dash Refill (Anonhelper)" => Ahorn.EntityPlacement(
        SuperDashRefill, 
	"point"
    )
)

sprite = "objects/AnonHelper/superDashRefill/idle00"

function getSprite(entity::SuperDashRefill)

    return sprite
end

function Ahorn.selection(entity::SuperDashRefill)
    oneUse = get(entity.data, "oneUse", false)
    x, y = Ahorn.position(entity)
    sprite = getSprite(entity)

    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::SuperDashRefill, room::Maple.Room)
    sprite = getSprite(entity)
    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end