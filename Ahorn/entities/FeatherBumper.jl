module AnonhelperFeatherBumper

using ..Ahorn, Maple
@mapdef Entity "Anonhelper/FeatherBumper" FeatherBumper(x::Integer, y::Integer, Wobble::Bool=true)
const placements = Ahorn.PlacementDict(
    "Feather Bumper (Anonhelper)" => Ahorn.EntityPlacement(
        FeatherBumper
    )
)

Ahorn.nodeLimits(entity::FeatherBumper) = 0, 1

sprite = "objects/AnonHelper/featherBumper/plugin.png"

function Ahorn.selection(entity::FeatherBumper)
    x, y = Ahorn.position(entity)
    nodes = get(entity.data, "nodes", ())

    if !isempty(nodes)
        nx, ny = Int.(nodes[1])

        return [Ahorn.getSpriteRectangle(sprite, x, y), Ahorn.getSpriteRectangle(sprite, nx, ny)]
    end

    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::FeatherBumper)
    x, y = Ahorn.position(entity)
    nodes = get(entity.data, "nodes", ())

    if !isempty(nodes)
        nx, ny = Int.(nodes[1])

        theta = atan(y - ny, x - nx)
        Ahorn.drawArrow(ctx, x, y, nx + cos(theta) * 8, ny + sin(theta) * 8, Ahorn.colors.selection_selected_fc, headLength=6)
        Ahorn.drawSprite(ctx, sprite, nx, ny)
    end
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::FeatherBumper, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end