//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

$CardSprite::DefaultMinSpeed = 230;  // In general, cards move at this speed.
$CardSprite::DefaultMaxTime = 0.2;  // If movement will take longer than this many seconds, increase speed.

$CardSprite::SIZE_X = 20;     // The size of the card sprites.
$CardSprite::SIZE_Y = 27.143;

$CardSprite::CameraArea = sceneWindow2D.getCurrentCameraArea();
$CardSprite::SortSizeMultiplier = (getWord($CardSprite::CameraArea, 3) - getWord($CardSprite::CameraArea, 1)) / $CardSprite::SIZE_Y;

$CardSprite::MotionLayer = 5;  // A high layer, so cards in flight don't end up underneath other cards.

$CardSprite::ButtonLayer = 0;  // The highest layer -- the GUI is above all!


/// (SimID this)
/// This callback is used to move the sprite from the "move" layer
/// to the layer it will rest in.
///
/// @param this The CardSprite object
///
function CardSprite::onPositionTarget(%this)
{
   %this.setLayer(%this.card.owner.getLayer());
}
