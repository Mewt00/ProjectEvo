//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(SwapImageOnCollisionBehavior))
{
   %template = new BehaviorTemplate(SwapImageOnCollisionBehavior);
   
   %template.friendlyName = "Swap Image On Collision";
   %template.behaviorType = "Effects";
   %template.description  = "Switches the image of an object when it collides";

   %template.addBehaviorField(image, "The image to swap to", object, "", t2dImageMapDatablock);
   %template.addBehaviorField(frame, "The frame of the image to swap to", int, 0);
}

function SwapImageOnCollisionBehavior::onBehaviorAdd(%this)
{
   %this.owner.collisionCallback = true;
}

function SwapImageOnCollisionBehavior::onCollision(%this, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts)
{
   if (isObject(%this.image))
      %this.owner.setImageMap(%this.image, %this.frame);
}
