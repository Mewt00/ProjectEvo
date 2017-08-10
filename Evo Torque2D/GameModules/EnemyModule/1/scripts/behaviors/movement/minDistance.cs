
if (!isObject(MinDistanceBehavior))
{
	%template = new BehaviorTemplate(MinDistanceBehavior);

	%template.friendlyName = "MinDistanceBehavior";
	%template.behaviorType = "Movement Base";
	%template.description  = "Set a minimum distance to maintain";

	//%template.addBehaviorField(scaled, "", bool, false);
	//%template.addBehaviorField(honcho, "", bool, false);
	%template.addBehaviorField(friendlyName, "The name", string, "MinDistanceBehavior");
  	%template.addBehaviorField(target, "The object to chase", object, "", t2dSceneObject);
	%template.addBehaviorField(number, "The number of behaviors", int, 0);
	%template.addBehaviorField(targetRotation, "Rotation to main target", float, 90.0);
	%template.addBehaviorField(moveSpeed, "The speed", int, 0);
	%template.addBehaviorField(distance, "Distance to attempt to keep", int, 40);
}

function MinDistanceBehavior::onBehaviorAdd(%this)
{
	//%this.owner.setUpdateCallback(true);
	%this.moveSpeed = 15 * %this.number;
}

function MinDistanceBehavior::onUpdate(%this)
{
	if (!isObject(%this.target))
		return;
	
	if(VectorDist(%this.owner.getPosition(), %this.target.getPosition()) < %this.distance)
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
