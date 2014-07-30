//-----------------------------------------------------------------------------

if (!isObject(FaceObjectBehavior))
{
   %template = new BehaviorTemplate(FaceObjectBehavior);
   
   %template.friendlyName = "Face Object";
   %template.behaviorType = "AI";
   %template.description  = "Set the object to face another object";

   %template.addBehaviorField(object, "The object to face", object, "", t2dSceneObject);
   %template.addBehaviorField(rotationOffset, "The rotation offset (degrees)", float, 0.0);
}

function FaceObjectBehavior::onBehaviorAdd(%this)
{
   %this.owner.setUpdateCallback(true);
}

function FaceObjectBehavior::onUpdate(%this)
{
   if (!isObject(%this.object))
      return;
   
   %targetRotation = Vector2AngleToPoint (%this.owner.getPosition(), %this.object.getPosition()) - 90;
   
   %this.owner.rotateTo(%targetRotation, %this.owner.turnSpeed);
}
