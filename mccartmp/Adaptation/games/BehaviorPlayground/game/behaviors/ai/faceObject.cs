//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(FaceObjectBehavior))
{
   %template = new BehaviorTemplate(FaceObjectBehavior);
   
   %template.friendlyName = "Face Object";
   %template.behaviorType = "AI";
   %template.description  = "Set the object to face another object";

   %template.addBehaviorField(object, "The object to face", object, "", t2dSceneObject);
   %template.addBehaviorField(turnSpeed, "The speed to rotate at (degrees per second). Use 0 to snap", float, 0.0);
   %template.addBehaviorField(rotationOffset, "The rotation offset (degrees)", float, 0.0);
}

function FaceObjectBehavior::onBehaviorAdd(%this)
{
   %this.owner.enableUpdateCallback();
}

function FaceObjectBehavior::onUpdate(%this)
{
   if (!isObject(%this.object))
      return;
   
   %vector = t2dVectorSub(%this.object.position, %this.owner.position);
   %targetRotation = mRadToDeg(mAtan(%vector.y, %vector.x)) + 90 + %this.rotationOffset;
   
   if (%this.turnSpeed == 0)
      %this.owner.setRotation(%targetRotation);
   else
      %this.owner.rotateTo(%targetRotation, %this.turnSpeed, true, false, true, 0.1);
}
