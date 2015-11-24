//-----------------------------------------------------------------------------

if (!isObject(ChaseBehavior))
{
	%template = new BehaviorTemplate(ChaseBehavior);
	
	%template.friendlyName = "Chase Object";
	%template.behaviorType = "Movement Base";
	%template.description  = "Set the object to chase";
	
	%template.addBehaviorField(target, "The object to chase", object, "", t2dSceneObject);
	%template.addBehaviorField(number, "The number of behaviors", int, 0);
	%template.addBehaviorInput(beingPaused, "Paused", "Allows a pause of this");
	%template.addBehaviorInput(beingUnpaused, "Paused", "Allows a pause of this");
	%template.addBehaviorField(isPaused, "", bool, false);

}

function ChaseBehavior::onBehaviorAdd(%this)
{
	%this.owner.setUpdateCallback(true);
	%this.owner.chaseSpeed = 15 * %this.number;
	//echo("walk speed   " @ %this.speed);
	%this.owner.turnSpeed = 60 * %this.number;
}

function ChaseBehavior::beingPaused(%this, %fromBehavior, %fromOutput)
{
	%this.isPaused = true;
	//echo("Being paused");
}

function ChaseBehavior::beingUnpaused(%this, %fromBehavior, %fromOutput)
{
	%this.isPaused = false;
	//echo("Being unpaused");
}

function ChaseBehavior::onUpdate(%this)
{
	if (!isObject(%this.target))
		return;
	
	if (%this.isPaused)
		return;
	//echo("owner position:    " @ %this.owner.getPosition());
	//echo("target position:    " @ %this.target.getPosition());
	
	%targetRotation = Vector2AngleToPoint (%this.owner.getPosition(), %this.target.getPosition());
	
	//echo("target rotation:    " @ %targetRotation);
	%xPercent = mCos(%targetRotation);
    %yPercent = mSin(%targetRotation);
	

	//echo("X:    " @ %xPercent);
	//echo("Y:     " @ %yPercent);
	//echo("X   :    " @ %this.owner.specialX);
	//echo("Y    :     " @ %this.owner.specialY);
	
	//%this.owner.specialX += %xcomponent;
    //%this.owner.specialY -= %ycomponent;
	
	echo("X:    " @ %xPercent);
	echo("Y:     " @ %yPercent);
	echo("walkspeed:     " @ %this.owner.walkspeed);

	
	%this.owner.tempLinearVelocityX += %xPercent * %this.owner.walkspeed;
	%this.owner.tempLinearVelocityY += %yPercent * %this.owner.walkspeed;
	//echo("chase speed is " @ %this.owner.getLinearVelocityX() @ " by " @ %this.owner.getLinearVelocityY());
	//%this.owner.setLinearVelocityX(%this.owner.getLinearVelocityX() + %xPercent * %this.owner.walkspeed);
	//%this.owner.setLinearVelocityY(%this.owner.getLinearVelocityY() + %yPercent * %this.owner.walkspeed);
	//echo("after chase speed is " @ %this.owner.getLinearVelocityX() @ " by " @ %this.owner.getLinearVelocityY());
	%this.owner.rotateTo(%targetRotation, %this.owner.turnSpeed);
}
