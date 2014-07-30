//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(RandomVelocityBehavior))
{
   %template = new BehaviorTemplate(RandomVelocityBehavior);
   
   %template.friendlyName = "Random Velocity";
   %template.behaviorType = "AI";
   %template.description  = "Sets a random velocity on the object when it spawns";

   %template.addBehaviorField(minAngle, "The minimum angle of the object's direction (degrees)", float, 0.0);
   %template.addBehaviorField(maxAngle, "The maximum angle of the object's direction (degrees)", float, 360.0);
   %template.addBehaviorField(minSpeed, "The minimum speed of the object (world units per second)", float, 10.0);
   %template.addBehaviorField(maxSpeed, "The maximum speed of the object (world units per second)", float, 25.0);
   %template.addBehaviorField(minRotationSpeed, "The minimum rotation speed of the object (degrees)", float, -90.0);
   %template.addBehaviorField(maxRotationSpeed, "The maximum rotation speed of the object (degrees)", float, 90.0);
}

function RandomVelocityBehavior::onAddToScene(%this, %scenegraph)
{
   %direction = getRandom(%this.minAngle * 1000, %this.maxAngle * 1000);
   %speed = getRandom(%this.minSpeed * 1000, %this.maxSpeed * 1000);
   %this.owner.setLinearVelocityPolar(%direction * 0.001, %speed * 0.001);
   
   %rotation = getRandom(%this.minRotationSpeed * 1000, %this.maxRotationSpeed * 1000);
   %this.owner.setAngularVelocity(%rotation * 0.001);
}
