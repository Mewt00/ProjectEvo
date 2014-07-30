//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(FollowMouseExBehavior))
{
   %template = new BehaviorTemplate(FollowMouseExBehavior);
   
   %template.friendlyName = "Follow Mouse Ex";
   %template.behaviorType = "Input";
   %template.description  = "Set the object to follow the position of the mouse. This version can be " @
                            "restricted by world limits and will obey collisions.";

   %template.addBehaviorField(followX, "Follow the mouse's X position", bool, true);
   %template.addBehaviorField(followY, "Follow the mouse's Y position", bool, true);
   %template.addBehaviorField(trackingSpeed, "The rate at which the object will move toward the mouse", float, 15.0);
}

function FollowMouseExBehavior::onBehaviorAdd(%this)
{
   %this.owner.enableUpdateCallback();
}

function FollowMouseExBehavior::onUpdate(%this)
{
   if (!isObject(sceneWindow2d))
      return;
   
   %mousePos = sceneWindow2D.getMousePosition();
   %position = %this.owner.position;
   
   %difference = t2dVectorSub(%mousePos, %position);
   %amount = t2dVectorScale(%difference, %this.trackingSpeed);
   
   if (%this.followX)
      %this.owner.setLinearVelocityX(%amount.x);
      
   if (%this.followY)
      %this.owner.setLinearVelocityY(%amount.y);
}
