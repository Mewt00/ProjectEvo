//-----------------------------------------------------------------------------

if (!isObject(LungeBehavior))
{
	%template = new BehaviorTemplate(LungeBehavior);
	
	%template.friendlyName = "LungeBehavior";
	%template.behaviorType = "Movement Base";
	%template.description  = "Set the object to lunge";
	
	%template.addBehaviorField(friendlyName, "The name", string, "LungeBehavior");
	%template.addBehaviorField(target, "The object to lunge", object, "", t2dSceneObject);
	%template.addBehaviorField(number, "The number of behaviors", int, 0);
	%template.addBehaviorField(targetRotation, "Rotation to main target", float, 90.0);
	%template.addBehaviorField(forwardSpeed, "The speed", int, 0);
	%template.addBehaviorField(lungeCooledDown, "Able to lunge", bool, true);
	%template.addBehaviorField(lungeDone, "Done with lunge", bool, true);
}

function LungeBehavior::onBehaviorAdd(%this)
{
	//%this.owner.setUpdateCallback(true);
	%this.forwardSpeed = 30 * %this.number;
	echo("forwardSpeed   " @ %this.owner.forwardSpeed);
}

function LungeBehavior::onUpdate(%this)
{
	if (!isObject(%this.target))
		return;
	
	//if(!%this.lungeCooledDown)
	//	return;

	if(%this.lungeDone)
	{
		if(!%this.lungeCooledDown || Vector2Distance(%this.owner.getPosition(), %this.target.getPosition()) > 30)
			return;
		%this.targetRotation = Vector2AngleToPoint (%this.owner.getPosition(), %this.target.getPosition());
	
		//%lungeQuery = getRandom(50);
	}
	/*switch(%lungeQuery)
	{
		case 0:
			echo("LUNGE");
		default:
			//echo("default");
			return;
	}*/
	
	
	echo("target rotation:    " @ %this.targetRotation);
	%xPercent = mCos(%this.targetRotation);
	echo("xpercent:    " @ %xPercent);
    %yPercent = mSin(%this.targetRotation);
	echo("yPercent:    " @ %yPercent);
	
	%this.owner.tempLinearVelocityX += %xPercent * %this.forwardSpeed;
	%this.owner.tempLinearVelocityY += %yPercent * %this.forwardSpeed;
	
	//%this.owner.setLinearVelocityX(%this.owner.getLinearVelocityX() + %xPercent * %this.owner.walkspeed * %this.owner.forwardSpeed);
	//%this.owner.setLinearVelocityY(%this.owner.getLinearVelocityY() + %yPercent * %this.owner.walkspeed * %this.owner.forwardSpeed);
	//echo("forwardSpeed   " @ %this.owner.forwardSpeed);
	%this.lungeCooledDown = false;
	%this.lungeDone = false;
	%this.schedule(1000, "endLunge");
	%this.schedule(1600, "canLunge");
}

function LungeBehavior::endLunge(%this)
{
	echo("in canLunge");
	%this.lungeDone = true;
}

function LungeBehavior::canLunge(%this)
{
	echo("in canLunge");
	%this.lungeCooledDown = true;
}