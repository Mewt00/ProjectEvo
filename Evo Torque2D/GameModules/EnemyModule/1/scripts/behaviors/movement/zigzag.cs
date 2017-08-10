//-----------------------------------------------------------------------------

if (!isObject(ZigZagBehavior))
{
	%template = new BehaviorTemplate(ZigZagBehavior);
	
	%template.friendlyName = "ZigZagBehavior";
	%template.behaviorType = "Movement Base";
	%template.description  = "Set the object to zig or zag";
	
	%template.addBehaviorField(friendlyName, "The name", string, "ZigZagBehavior");
	%template.addBehaviorField(target, "The object to zig/zag", object, "", t2dSceneObject);
	%template.addBehaviorField(number, "The number of behaviors", int, 0);
	%template.addBehaviorField(targetRotation, "Rotation to main target", float, 90.0);
	%template.addBehaviorField(sideSpeed, "The speed", int, 0);
	//%template.addBehaviorField(isMoving, "", bool, false);
	%template.addBehaviorField(zigZagCooledDown, "Able to zig or zag", bool, true);
	%template.addBehaviorField(zigZagDone, "Done with zig or zag", bool, true);
	//%template.addBehaviorOutput(pauseForZigZag, "Pause", "Allows a pause of movements");
	//%template.addBehaviorOutput(unpauseForZigZag, "unpause", "Allows an unpause of movements");
	
}

function ZigZagBehavior::onBehaviorAdd(%this)
{
	//%this.owner.setUpdateCallback(true);
	%this.sideSpeed = 40 * %this.number;
	//echo("walk speed   " @ %this.speed);
}

/*function ZigZagBehavior::pauseForZigZag(%this)
{
	//echo("zigzag pause");
	%this.isMoving = true;
	//echo("before raise2");
	%this.owner.Raise(%this, unpauseForZigZag, 100);
	//echo("after raise2");
}*/

/*function ZigZagBehavior::unpauseForZigZag(%this)
{
	//echo("zigzag unpause");
	%this.isMoving = false;
	//%this.owner.setLinearVelocityX(0);
	//%this.owner.setLinearVelocityY(0);	
	
}*/

function ZigZagBehavior::onUpdate(%this)
{
	if (!isObject(%this.target))
		return;
	
	//if(%this.isMoving)
	//	return;
	
	if(%this.zigZagDone)
	{
		if(!%this.zigZagCooledDown)
			return;
		%this.targetRotation = Vector2AngleToPoint (%this.owner.getPosition(), %this.target.getPosition());
	
		%leftRight = getRandom(100);
	
		switch(%leftRight)
		{
			case 0:
				%this.targetRotation += 90;
			case 1:
				%this.targetRotation -= 90;
			default:
				//echo("default");
				return;
		}
	}
	
	%xPercent = mCos(%this.targetRotation);
    %yPercent = mSin(%this.targetRotation);
	
	//%this.owner.Raise(%this, pauseForZigZag);
	
	//echo("after raise");
	
	%this.owner.tempLinearVelocityX += %xPercent * %this.sideSpeed;
	%this.owner.tempLinearVelocityY += %yPercent * %this.sideSpeed;
	//echo("chase speed is " @ %this.owner.getLinearVelocityX() @ " by " @ %this.owner.getLinearVelocityY());
	
	//%this.owner.setLinearVelocityX(%this.owner.getLinearVelocityX() + %xPercent * %this.owner.walkspeed * %this.owner.sideSpeed);
	//%this.owner.setLinearVelocityY(%this.owner.getLinearVelocityY() + %yPercent * %this.owner.walkspeed * %this.owner.sideSpeed);
	
	%this.zigZagCooledDown = false;
	%this.zigZagDone = false;
	%this.schedule(500, "endZigZag");
	%this.schedule(1600, "canZigZag");
}

function ZigZagBehavior::endZigZag(%this)
{
	%this.zigZagDone = true;
}

function ZigZagBehavior::canZigZag(%this)
{
	%this.zigZagCooledDown = true;
}
