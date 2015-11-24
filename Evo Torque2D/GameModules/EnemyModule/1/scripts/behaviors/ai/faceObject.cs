//-----------------------------------------------------------------------------

if (!isObject(FaceObjectBehavior))
{
   %template = new BehaviorTemplate(FaceObjectBehavior);
   
   %template.friendlyName = "Face Object";
   %template.behaviorType = "AI";
   %template.description  = "Set the object to face another object";

   %template.addBehaviorField(target, "The object to face", object, "", t2dSceneObject);
   %template.addBehaviorField(number, "The number of behaviors", int, 0);
}

function FaceObjectBehavior::onBehaviorAdd(%this)
{
   %this.owner.setUpdateCallback(true);
   %this.owner.turnSpeed = 60 * %this.number;
}

function FaceObjectBehavior::onUpdate(%this)
{
	if (!isObject(%this.target))
		return;
	
	//echo("faceObject owner position:    " @ %this.owner.getPosition());
	//echo("faceObject target position:    " @ %this.target.getPosition());
	
	%targetRotation = Vector2AngleToPoint (%this.owner.getPosition(), %this.target.getPosition());//TODO?
	
	//echo("faceObject target rotation:    " @ %targetRotation);
	
	%this.owner.rotateTo(%targetRotation, %this.owner.turnSpeed);
}
