
if (!isObject(MaxDistanceBehavior))
{
	%template = new BehaviorTemplate(MaxDistanceBehavior);

	%template.friendlyName = "MaxDistanceBehavior";
	%template.behaviorType = "Movement Base";
	%template.description  = "Set a maximum distance to maintain";

	//%template.addBehaviorField(scaled, "", bool, false);
	//%template.addBehaviorField(honcho, "", bool, false);
	%template.addBehaviorField(friendlyName, "The name", string, "MaxDistanceBehavior");
	%template.addBehaviorField(target, "The object to chase", object, "", t2dSceneObject);
	%template.addBehaviorField(number, "The number of behaviors", int, 0);
	%template.addBehaviorField(targetRotation, "Rotation to main target", float, 90.0);
	%template.addBehaviorField(moveSpeed, "The speed", int, 0);
	%template.addBehaviorField(distance, "Default distance to attempt to keep", int, 80);
	%template.addBehaviorField(extra, "Distance input", int, -9999);
}

function MaxDistanceBehavior::onBehaviorAdd(%this)
{
	//%this.owner.setUpdateCallback(true);
	%this.moveSpeed = 15 * %this.number;
	if(%this.extra != -9999)
		%this.distance = %this.extra;
}

function MaxDistanceBehavior::onUpdate(%this)
{
	if (!isObject(%this.target))
		return;
	
	if(VectorDist(%this.owner.getPosition(), %this.target.getPosition()) > %this.distance)
	{
		%this.targetRotation = Vector2AngleToPoint (%this.owner.getPosition(), %this.target.getPosition());
	
		%this.targetRotation += 180;
		//echo("target rotation:    " @ %this.targetRotation);
		%xPercent = mCos(%this.targetRotation);
		%yPercent = mSin(%this.targetRotation);
		
		echo("X:    " @ %xPercent);
		echo("Y:     " @ %yPercent);
		echo("walkspeed:     " @ %this.owner.walkspeed);

		
		%this.owner.tempLinearVelocityX += %xPercent * %this.moveSpeed;
		%this.owner.tempLinearVelocityY += %yPercent * %this.moveSpeed;
		
	}
}
