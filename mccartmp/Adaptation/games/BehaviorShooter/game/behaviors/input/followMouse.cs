//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(FollowMouseBehavior))
{
   %template = new BehaviorTemplate(FollowMouseBehavior);
   
   %template.friendlyName = "Follow Mouse";
   %template.behaviorType = "Input";
   %template.description  = "Set the object to follow the position of the mouse";

   %template.addBehaviorField(followX, "Follow the mouse's X position", bool, true);
   %template.addBehaviorField(followY, "Follow the mouse's Y position", bool, true);
}

//[neo, 5/18/2007 - #3014
//function FollowMouseBehavior::onBehaviorAdd(%this)
function FollowMouseBehavior::onAddToScene( %this )
{
   %this.owner.setUseMouseEvents(true);
   %this.owner.setMouseLocked(true);
}

function FollowMouseBehavior::onMouseMove(%this, %modifier, %worldPos)
{
   if (%this.followX)
      %this.owner.setPositionX(%worldPos.x);
   
   if (%this.followY)
      %this.owner.setPositionY(%worldPos.y);
}
