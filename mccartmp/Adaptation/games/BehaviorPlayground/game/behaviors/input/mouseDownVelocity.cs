//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(MouseDownVelocityBehavior))
{
   %template = new BehaviorTemplate(MouseDownVelocityBehavior);
   
   %template.friendlyName = "Mouse Down Velocity";
   %template.behaviorType = "Input";
   %template.description  = "Set the object's velocity on mouse down";

   %template.addBehaviorField(xVelocity, "The speed at which the object will move horizontally (world units per second)", float, 5.0);
   %template.addBehaviorField(yVelocity, "The speed at which the object will move vertically (world units per second)", float, 5.0);
}

function MouseDownVelocityBehavior::onBehaviorAdd(%this)
{
   %this.owner.setUseMouseEvents(true);
}

function MouseDownVelocityBehavior::onMouseDown(%this, %modifier, %worldPos)
{
   %this.owner.linearVelocity = %this.xVelocity SPC %this.yVelocity;
}
