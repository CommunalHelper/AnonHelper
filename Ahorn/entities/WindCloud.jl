module AnonhelperAnonCloud

using ..Ahorn, Maple

@mapdef Entity "Anonhelper/WindCloud" WindCloud(x::Integer, y::Integer, small::Bool=false, fragile::Bool=false)


const placements = Ahorn.PlacementDict(
    "Wind Cloud (Anonhelper)" => Ahorn.EntityPlacement(
	WindCloud,
	"point"
   )
)

const sprites = Dict{Tuple{Bool, Bool}, String}(
	(false, false) => "objects/AnonHelper/clouds/windcloud00",
	(true, false) => "objects/AnonHelper/clouds/windfragile00",
	(false, true) => "objects/AnonHelper/clouds/windcloudRemix00",
	(true, true) => "objects/AnonHelper/clouds/windfragileRemix00",
)

function Ahorn.selection(entity::WindCloud)
    fragile = get(entity.data, "fragile", false)
    small = get(entity.data, "small", false)
    sprite = sprites[(fragile, small)]
    x, y = Ahorn.position(entity)
    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::WindCloud, room::Maple.Room)
	fragile = get(entity.data, "fragile", false)
	small = get(entity.data, "small", false)
	sprite = sprites[(fragile, small)]
   Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end