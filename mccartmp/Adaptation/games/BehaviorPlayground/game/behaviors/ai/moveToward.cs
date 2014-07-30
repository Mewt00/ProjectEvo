//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(MoveTowardBehavior))
{
   %template = new BehaviorTemplate(MoveTowardBehavior);
   
   %template.friendlyName = "Move Toward";
   %template.behaviorType = "AI";
   %template.description  = "Set the object to move toward another object";

   %template.addBehaviorField(object, "The object to move toward", object, "", t2dSceneObject);
   %template.addBehaviorField(speed, "The speed to move toward the object at (world units per second)", float, 2.0);
}

function MoveTowardBehavior::onAddToScene(%this)
{
   %this.owner.enableUpdateCallback();
}

function MoveTowardBehavior::onUpdate(%this)
{
   if (!isObject(%this.object))
      return;
   
   %this.owner.moveTo(%this.object.position, %this.speed);
}
