module AnonhelperDestructableBounceBlock

using ..Ahorn, Maple

@mapdef Entity "Anonhelper/DestructableBounceBlock" DestructableBounceBlock(x::Integer, y::Integer, notCoreMode::Bool=false)

const placements = Ahorn.PlacementDict(
    "Destructible Bounce Block (Anonhelper)" => Ahorn.EntityPlacement(
        DestructableBounceBlock,
        "rectangle"
    ),
)

Ahorn.minimumSize(entity::DestructableBounceBlock) = 16, 16
Ahorn.resizable(entity::DestructableBounceBlock) = true, true

Ahorn.selection(entity::DestructableBounceBlock) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::DestructableBounceBlock, room::Maple.Room)
    x = Int(get(entity.data, "x", 0))
    y = Int(get(entity.data, "y", 0))

    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    notCoreMode = get(entity.data, "notCoreMode", false)

    if notCoreMode == true
        frameResource = "objects/AnonHelper/bumpBlockNew/ice00"
        crystalResource = "objects/AnonHelper/bumpBlockNew/ice_center00"
    else
        frameResource = "objects/AnonHelper/bumpBlockNew/fire00"
        crystalResource = "objects/AnonHelper/bumpBlockNew/fire_center00"
    end

    crystalSprite = Ahorn.getSprite(crystalResource, "Gameplay")
    
    tilesWidth = div(width, 8)
    tilesHeight = div(height, 8)

    Ahorn.Cairo.save(ctx)

    Ahorn.rectangle(ctx, 0, 0, width, height)
    Ahorn.clip(ctx)

    for i in 0:ceil(Int, tilesWidth / 6)
        Ahorn.drawImage(ctx, frameResource, i * 48 + 8, 0, 8, 0, 48, 8)

        for j in 0:ceil(Int, tilesHeight / 6)
            Ahorn.drawImage(ctx, frameResource, i * 48 + 8, j * 48 + 8, 8, 8, 48, 48)

            Ahorn.drawImage(ctx, frameResource, 0, j * 48 + 8, 0, 8, 8, 48)
            Ahorn.drawImage(ctx, frameResource, width - 8, j * 48 + 8, 56, 8, 8, 48)
        end

        Ahorn.drawImage(ctx, frameResource, i * 48 + 8, height - 8, 8, 56, 48, 8)
    end

    Ahorn.drawImage(ctx, frameResource, 0, 0, 0, 0, 8, 8)
    Ahorn.drawImage(ctx, frameResource, width - 8, 0, 56, 0, 8, 8)
    Ahorn.drawImage(ctx, frameResource, 0, height - 8, 0, 56, 8, 8)
    Ahorn.drawImage(ctx, frameResource, width - 8, height - 8, 56, 56, 8, 8)
    
    Ahorn.drawImage(ctx, crystalSprite, div(width - crystalSprite.width, 2), div(height - crystalSprite.height, 2))

    Ahorn.restore(ctx)
end

end