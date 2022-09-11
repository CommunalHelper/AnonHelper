module AnonhelperInvisibleSeekerBarrier

using ..Ahorn, Maple

@mapdef Entity "Anonhelper/InvisibleSeekerBarrier" InvisibleSeekerBarrier(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Invisible Seeker Barrier (Anonhelper)" => Ahorn.EntityPlacement(
        InvisibleSeekerBarrier,
        "rectangle"
    ),
)

Ahorn.minimumSize(entity::InvisibleSeekerBarrier) = 8, 8
Ahorn.resizable(entity::InvisibleSeekerBarrier) = true, true

function Ahorn.selection(entity::InvisibleSeekerBarrier)
    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 8))
    height = Int(get(entity.data, "height", 8))

    return Ahorn.Rectangle(x, y, width, height)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::InvisibleSeekerBarrier, room::Maple.Room)
    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    Ahorn.drawRectangle(ctx, 0, 0, width, height, (0.25, 0.25, 0.25, 0.8), (0.0, 0.0, 0.0, 0.0))
end

end