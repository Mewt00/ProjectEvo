//-----------------------------------------------------------------------------

if (!isObject(BlinkBehavior))
{
	%template = new BehaviorTemplate(BlinkBehavior);
	
	%template.friendlyName = "BlinkBehavior";
	%template.behaviorType = "Movement Base";
	%template.description  = "Set the object to blink";
	
	%template.addBehaviorField(friendlyName, "The name", string, "BlinkBehavior");
	%template.addBehaviorField(target, "The object to lunge", object, "", t2dSceneObject);
	%template.addBehaviorField(number, "The number of behaviors", int, 0);
	%template.addBehaviorField(blinkFrequency, "The frequency of blinks", int, 0);
	%template.addBehaviorField(blinkCooledDown, "Able to blink", bool, true);
	%template.addBehaviorField(blinkDone, "Done with blink", bool, true);
	//%template.addBehaviorField(targetRotation, "Rotation to main target", float, 90.0);
}

function BlinkBehavior::onBehaviorAdd(%this)
{
	//%this.owner.setUpdateCallback(true);
	%this.blinkFrequency = mFloor(250/%this.number);
}

function BlinkBehavior::onUpdate(%this)
{
	echo("Blink update");
	if (!isObject(%this.target))
		return;

	if(%this.blinkDone)
	{
		if(!%this.blinkCooledDown)
			return;
	
		%blinkChance = getRandom(%this.blinkFrequency);
		
		if(%blinkChance == 1)
		{
			%this.owner.setEnabled(false);
			%this.owner.myHealthbar.setVisible(false);
			//%this.owner.setVisible(false);
			
			%offsetX = (60 * $pixelsToWorldUnits) - $roomWidth/2;
			%offsetY = (60 * $pixelsToWorldUnits) - $roomHeight/2;
			%blinkXLocation = getRandom($roomWidth - (4 * 30 * $pixelsToWorldUnits));
			%blinkYLocation = getRandom($roomHeight - (4 * 30 * $pixelsToWorldUnits));
			
			%this.owner.setPosition(%blinkXLocation + %offsetX, %blinkYLocation + %offsetY);
			echo("BLINK");
			echo("Location" @ %this.owner.getPosition());
			%this.blinkCooledDown = false;
			//%this.blinkDone = false;
			%this.schedule(320, "blink");
			%this.schedule(1600, "canBlink");
		}
	}
	
	
	/*echo("target rotation:    " @ %this.targetRotation);
	%xPercent = mCos(%this.targetRotation);
	echo("xpercent:    " @ %xPercent);
    %yPercent = mSin(%this.targetRotation);
	echo("yPercent:    " @ %yPercent);
	
	%this.owner.tempLinearVelocityX += %xPercent  * %this.owner.walkspeed * %this.owner.forwardSpeed;
	%this.owner.tempLinearVelocityY += %yPercent  * %this.owner.walkspeed * %this.owner.forwardSpeed;
	
	//%this.owner.setLinearVelocityX(%this.owner.getLinearVelocityX() + %xPercent * %this.owner.walkspeed * %this.owner.forwardSpeed);
	//%this.owner.setLinearVelocityY(%this.owner.getLinearVelocityY() + %yPercent * %this.owner.walkspeed * %this.owner.forwardSpeed);
	echo("forwardSpeed   " @ %this.owner.forwardSpeed);*/
	
}

function BlinkBehavior::blink(%this)
{
	/*%offsetX = (30 * $pixelsToWorldUnits) - $roomWidth/2;
	%offsetY = (30 * $pixelsToWorldUnits) - $roomHeight/2;
	%blinkXLocation = getRandom($roomWidth - (2 * 30 * $pixelsToWorldUnits));
	%blinkYLocation = getRandom($roomHeight - (2 * 30 * $pixelsToWorldUnits));
	
	%this.owner.setPosition(%blinkXLocation + %offsetX, %blinkYLocation + %offsetY);*/
	%this.owner.setEnabled(true);
	%this.owner.myHealthbar.setVisible(true);
	//%this.owner.setVisible(true);
	echo("BLINK");
	echo("Location" @ %this.owner.getPosition());
	%this.blinkDone = true;
}

function BlinkBehavior::canBlink(%this)
{
	echo("in canLunge");
	%this.blinkCooledDown = true;
}