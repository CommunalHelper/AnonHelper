module AnonhelperBoosterRefill

using ..Ahorn, Maple

@mapdef Entity "Anonhelper/BoosterRefill" BoostRefill(x::Integer, y::Integer, oneUse::Bool=false, boostOnEnd::Bool=false)


const placements = Ahorn.PlacementDict(
    "Red Booster Refill (Anonhelper)" => Ahorn.EntityPlacement(
        BoostRefill, 
	"point"
    )
)


function boosterSprite(entity::BoostRefill)
    red = get(entity.data, "boostOnEnd", false)
    
    if red
        return "objects/AnonHelper/boosterRefillOnDash/idle00"

    else
        return "objects/AnonHelper/boosterRefill/idle00"
    end
end

function Ahorn.selection(entity::BoostRefill)
    oneUse = get(entity.data, "oneUse", false)
    x, y = Ahorn.position(entity)
    sprite = boosterSprite(entity)

    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::BoostRefill, room::Maple.Room)
    sprite = boosterSprite(entity)
    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end
