module AnonhelperAnonCloud

using ..Ahorn, Maple

@mapdef Entity "Anonhelper/WindCloud" WindCloud(x::Integer, y::Integer, small::Bool=false, fragile::Bool=false)


const placements = Ahorn.PlacementDict(
    "WindCloud (Anonhelper)" => Ahorn.EntityPlacement(
	WindCloud,
	"point"
   )
)

const sprites = Dict{Tuple{Bool, Bool}, String}(
	(false, false) => "objects/Clouds/windcloud00",
	(true, false) => "objects/Clouds/windfragile00",
	(false, true) => "objects/Clouds/windcloudRemix00",
	(true, true) => "objects/Clouds/windfragileRemix00",
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