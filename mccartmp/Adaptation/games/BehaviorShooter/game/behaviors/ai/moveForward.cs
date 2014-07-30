//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(MoveForwardBehavior))
{
   %template = new BehaviorTemplate(MoveForwardBehavior);
   
   %template.friendlyName = "Move Forward";
   %template.behaviorType = "AI";
   %template.description  = "Makes the object move in the direction it's facing.";

   %template.addBehaviorField(speed, "Speed to move", float, 5.0);
   %template.addBehaviorField(angleOffset, "The direction to move relative to the current rotation (degrees)", float, 0.0);
   %template.addBehaviorField(updateRate, "Time between thruster activations (seconds)", float, 0.25);
}

function MoveForwardBehavior::onAddToScene(%this, %scenegraph)
{
   %this.schedule( %this.updateRate * 1000, "update");
}

function MoveForwardBehavior::update(%this)
{
   %this.owner.setLinearVelocityPolar( %this.owner.rotation + %this.angleOffset, %this.speed );
   %this.schedule( %this.updateRate * 1000, "update");
}
