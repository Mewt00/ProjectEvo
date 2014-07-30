//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(FleeMouseBehavior))
{
   %template = new BehaviorTemplate(FleeMouseBehavior);
   
   %template.friendlyName = "Flee Mouse";
   %template.behaviorType = "Input";
   %template.description  = "Set the object to attempt to avoid the mouse";
   
   %template.addBehaviorField(impulse, "Avoidance impulse", float, 2.0);
   %template.addBehaviorField(damping, "Amount to damp movement (percentage of velocity per second)", float, 8.0);
}

function FleeMouseBehavior::onBehaviorAdd(%this)
{
   %this.owner.setUseMouseEvents(true);
   %this.owner.setDamping(%this.damping);
}

function FleeMouseBehavior::onMouseMove(%this, %modifier, %worldPos)
{
   %xImpulse = %this.owner.position.x - %worldPos.x;
   %yImpulse = %this.owner.position.y - %worldPos.y;
   
   // We need this if the mouse is locked.
   %halfWidth = %this.owner.getWidth() / 2;
   %halfHeight = %this.owner.getHeight() / 2;
   if ((mAbs(%xImpulse) > %halfWidth) || (mAbs(%yImpulse) > %halfHeight))
   {
      %xImpulse = 0;
      %yImpulse = 0;
   }
   
   %this.owner.setImpulseForce(%xImpulse * %this.impulse, %yImpulse * %this.impulse);
}
