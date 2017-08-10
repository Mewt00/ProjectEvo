//-----------------------------------------------------------------------------

if (!isObject(StrafeBehavior))
{
	%template = new BehaviorTemplate(StrafeBehavior);
	
	%template.friendlyName = "StrafeBehavior";
	%template.behaviorType = "Movement Base";
	%template.description  = "Set the object to strafe";
	
	%template.addBehaviorField(friendlyName, "The name", string, "StrafeBehavior");
	%template.addBehaviorField(target, "The object to strafe around", object, "", t2dSceneObject);
	%template.addBehaviorField(number, "The number of behaviors", int, 0);
	%template.addBehaviorField(targetRotation, "Rotation to main target", float, 90.0);
	%template.addBehaviorField(strafeSpeed, "The speed", int, 0);
}

function StrafeBehavior::onBehaviorAdd(%this)
{
	//%this.owner.setUpdateCallback(true);
	%this.strafeSpeed = %this.owner.walkspeed * %this.number;
}

function StrafeBehavior::onUpdate(%this)
{
	if (!isObject(%this.target))
		return;	
	
	%this.targetRotation = Vector2AngleToPoint (%this.owner.getPosition(), %this.target.getPosition()) + 90;
	%stayOnCourse = getRandom(50);
	//echo("stayOnCourse    " @ %stayOnCourse);
	switch(%stayOnCourse)
	{
		case 0:
			%this.targetRotation -= 180;
			echo("targetRotation    " @ %this.targetRotation);
		default:
			
	}
	
	
	//echo("target rotation:    " @ %this.targetRotation);
	%xPercent = mCos(%this.targetRotation);
    %yPercent = mSin(%this.targetRotation);
	
	%this.owner.tempLinearVelocityX += %xPercent * %this.strafeSpeed;
	%this.owner.tempLinearVelocityY += %yPercent * %this.strafeSpeed;
	
}