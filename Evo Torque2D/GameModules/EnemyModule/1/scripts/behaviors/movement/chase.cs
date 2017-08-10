//-----------------------------------------------------------------------------

if (!isObject(ChaseBehavior))
{
	%template = new BehaviorTemplate(ChaseBehavior);
	
	%template.friendlyName = "ChaseBehavior";
	%template.behaviorType = "Movement Base";
	%template.description  = "Set the object to chase";
	
	%template.addBehaviorField(friendlyName, "The name", string, "ChaseBehavior");
	%template.addBehaviorField(target, "The object to chase", object, "", t2dSceneObject);
	%template.addBehaviorField(number, "The number of behaviors", int, 0);
	%template.addBehaviorField(targetRotation, "Rotation to main target", float, 90.0);
	%template.addBehaviorField(chaseSpeed, "The speed", int, 0);
	/*%template.addBehaviorInput(beingPaused, "Paused", "Allows a pause of this");
	%template.addBehaviorInput(beingUnpaused, "Paused", "Allows a pause of this");
	%template.addBehaviorField(isPaused, "", bool, false);*/

}

function ChaseBehavior::onBehaviorAdd(%this)
{
	//%this.owner.setUpdateCallback(true);
	%this.chaseSpeed = 15 * %this.number;
	//echo("walk speed   " @ %this.speed);
	%this.owner.turnSpeed = 60 * %this.number;
	echo("ADDED");
}

/*function ChaseBehavior::beingPaused(%this, %fromBehavior, %fromOutput)
{
	%this.isPaused = true;
	//echo("Being paused");
}

function ChaseBehavior::beingUnpaused(%this, %fromBehavior, %fromOutput)
{
	%this.isPaused = false;
	//echo("Being unpaused");
}*/

function ChaseBehavior::onUpdate(%this)
{
	echo("UPDATE");
	if (!isObject(%this.target))
		return;
	
	if (%this.isPaused)
		return;
	
	%this.targetRotation = Vector2AngleToPoint (%this.owner.getPosition(), %this.target.getPosition());
	
	//echo("target rotation:    " @ %this.targetRotation);
	%xPercent = mCos(%this.targetRotation);
    %yPercent = mSin(%this.targetRotation);
	
	echo("X:    " @ %xPercent);
	echo("Y:     " @ %yPercent);
	echo("walkspeed:     " @ %this.owner.walkspeed);

	
	%this.owner.tempLinearVelocityX += %xPercent * %this.chaseSpeed;
	%this.owner.tempLinearVelocityY += %yPercent * %this.chaseSpeed;
	//echo("chase speed is " @ %this.owner.getLinearVelocityX() @ " by " @ %this.owner.getLinearVelocityY());
	//%this.owner.setLinearVelocityX(%this.owner.getLinearVelocityX() + %xPercent * %this.owner.walkspeed);
	//%this.owner.setLinearVelocityY(%this.owner.getLinearVelocityY() + %yPercent * %this.owner.walkspeed);
	//echo("after chase speed is " @ %this.owner.getLinearVelocityX() @ " by " @ %this.owner.getLinearVelocityY());
	%this.owner.rotateTo(%this.targetRotation, %this.owner.turnSpeed);
}
