module AnonhelperOneUseBooster

using ..Ahorn, Maple

@mapdef Entity "Anonhelper/OneUseBooster" OneUseBooster(x::Integer, y::Integer, red::Bool=false)

const placements = Ahorn.PlacementDict(
    "One Use Booster (Anonhelper)" => Ahorn.EntityPlacement(
        OneUseBooster
    )
)

function boosterSprite(entity::OneUseBooster)
    red = get(entity.data, "red", false)
    
    if red
        return "objects/booster/boosterRed00"

    else
        return "objects/booster/booster00"
    end
end

function Ahorn.selection(entity::OneUseBooster)
    x, y = Ahorn.position(entity)
    sprite = boosterSprite(entity)

    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::OneUseBooster, room::Maple.Room)
    sprite = boosterSprite(entity)

    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end