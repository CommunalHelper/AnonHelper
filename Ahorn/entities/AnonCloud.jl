module AnonhelperAnonCloud

using ..Ahorn, Maple

@mapdef Entity "Anonhelper/AnonCloud" AnonCloud(x::Integer, y::Integer, small::Bool=false, pink::Bool=false)


const placements = Ahorn.PlacementDict(
    "AnonCloud (Anonhelper)" => Ahorn.EntityPlacement(
	AnonCloud,
	"point"
   )
)

const sprites = Dict{Tuple{Bool, Bool}, String}(
	(false, false) => "objects/Clouds/whitecloud00",
	(true, false) => "objects/Clouds/pinkcloud00",
	(false, true) => "objects/Clouds/whitecloudRemix00",
	(true, true) => "objects/Clouds/pinkcloudRemix00",
)

function Ahorn.selection(entity::AnonCloud)
    pink = get(entity.data, "pink", false)
    small = get(entity.data, "small", false)
    sprite = sprites[(pink, small)]
    x, y = Ahorn.position(entity)
    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::AnonCloud, room::Maple.Room)
	pink = get(entity.data, "pink", false)
	small = get(entity.data, "small", false)
	sprite = sprites[(pink, small)]
   Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end