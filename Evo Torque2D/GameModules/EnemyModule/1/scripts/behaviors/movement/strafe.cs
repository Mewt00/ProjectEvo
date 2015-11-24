//-----------------------------------------------------------------------------

if (!isObject(StrafeBehavior))
{
	%template = new BehaviorTemplate(StrafeBehavior);
	
	%template.friendlyName = "Zigzag Object";
	%template.behaviorType = "Movement Base";
	%template.description  = "Set the object to zig or zag";
	
	%template.addBehaviorField(target, "The object to strafe around", object, "", t2dSceneObject);
	%template.addBehaviorField(number, "The number of behaviors", int, 0);
}

function StrafeBehavior::onBehaviorAdd(%this)
{
	%this.owner.setUpdateCallback(true);
	//%this.owner.sideSpeed = 5 * %this.number;
	//echo("walk speed   " @ %this.speed);
}

function StrafeBehavior::onUpdate(%this)
{
	if (!isObject(%this.target))
		return;
	//echo("owner position:    " @ %this.owner.getPosition());
	//echo("target position:    " @ %this.target.getPosition());
	
	
	%targetRotation = Vector2AngleToPoint (%this.owner.getPosition(), %this.target.getPosition()) + 90;
	%stayOnCourse = getRandom(50);
	//echo("stayOnCourse    " @ %stayOnCourse);
	switch(%stayOnCourse)
	{
		case 0:
			%targetRotation -= 180;
			echo("targetRotation    " @ %targetRotation);
		default:
			//echo("default");
			//return;
	}
	
	
	//echo("target rotation:    " @ %targetRotation);
	%xPercent = mCos(%targetRotation);
    %yPercent = mSin(%targetRotation);
	
	%this.owner.tempLinearVelocityX += %xPercent * %this.owner.walkspeed * %this.number;
	%this.owner.tempLinearVelocityY += %yPercent * %this.owner.walkspeed * %this.number;
	//echo("chase speed is " @ %this.owner.getLinearVelocityX() @ " by " @ %this.owner.getLinearVelocityY());
	
	//%this.owner.setLinearVelocityX(%this.owner.getLinearVelocityX() + %xPercent * %this.owner.walkspeed * %this.owner.sideSpeed);
	//%this.owner.setLinearVelocityY(%this.owner.getLinearVelocityY() + %yPercent * %this.owner.walkspeed * %this.owner.sideSpeed);
	
}

/*if (!isObject(StrafeBehavior))
{
  %template = new BehaviorTemplate(StrafeBehavior);

  %template.friendlyName = "Maximum Distance";
  %template.behaviorType = "AI";
  %template.description  = "Set the object to face another object";

  %template.addBehaviorField(distance, "", int, 70);
  //%template.addBehaviorField(distance, "", int, 50);
  %template.addBehaviorField(scaled, "", bool, false);
  %template.addBehaviorField(moveSpeed, "", int, 10);
  %template.addBehaviorField(honcho, "", bool, false);
}

function StrafeBehavior::onBehaviorAdd(%this)
{
  %this.owner.moveBehaviorCount++;
  if(%this.owner.moveBehaviorCount == 1)
    %this.honcho = true;
  %this.owner.setUpdateCallback(true);
}

function StrafeBehavior::onUpdate( %this )
{
	//echo("strafe update");
	if(%this.honcho == true)
	{
		%this.owner.specialX = 0;
		%this.owner.specialY = 0;
	}

	if(!isObject(%this.owner))
	{
		%this.safeDelete();
	}
	else
	{
    %startVelocity = %this.owner.specialX SPC %this.owner.specialY;
    %targetRotation = Vector2AngleToPoint (%this.owner.getPosition(), %this.owner.mainTarget.getPosition());//+90 not needed?

    %xcomponent = %this.moveSpeed * mSin(mDegToRad(%targetRotation)) / %this.owner.moveBehaviorCount;
    %ycomponent = %this.moveSpeed * mCos(mDegToRad(%targetRotation)) / %this.owner.moveBehaviorCount;

    %this.owner.specialX = getWord(%startVelocity, 0) + %xcomponent;
    %this.owner.specialY = getWord(%startVelocity, 1) - %ycomponent;
	}
}
*/