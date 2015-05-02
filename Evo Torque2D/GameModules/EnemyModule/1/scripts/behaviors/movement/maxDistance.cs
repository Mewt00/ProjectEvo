//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(MaxDistanceBehavior))
{
  %template = new BehaviorTemplate(MaxDistanceBehavior);

  %template.friendlyName = "Maximum Distance";
  %template.behaviorType = "AI";
  %template.description  = "Set the object to face another object";

  %template.addBehaviorField(distance, "", int, 70);
  //%template.addBehaviorField(distance, "", int, 50);
  %template.addBehaviorField(scaled, "", bool, false);
  %template.addBehaviorField(moveSpeed, "", int, 10);
  %template.addBehaviorField(honcho, "", bool, false);
}

function MaxDistanceBehavior::onBehaviorAdd(%this)
{
  %this.owner.moveBehaviorCount++;
  if(%this.owner.moveBehaviorCount == 1)
    %this.honcho = true;
}

function MaxDistanceBehavior::onUpdate(%this)
{
  if(%this.honcho == true){
    %this.owner.specialX = 0;
    %this.owner.specialY = 0;
  }

	if(!isObject(%this.owner))
	{
		%this.safeDelete();
	}
	else
	{
    if(VectorDist(%this.owner.getPosition(), %this.owner.mainTarget.getPosition()) > %this.distance){
      %startVelocity = %this.owner.specialX SPC %this.owner.specialY;
      %targetRotation = Vector2AngleToPoint (%this.owner.getPosition(), %this.owner.mainTarget.getPosition());//TODO?
      
      %xcomponent = %this.moveSpeed * mSin(mDegToRad(%targetRotation)) / %this.owner.moveBehaviorCount;
      %ycomponent = %this.moveSpeed * mCos(mDegToRad(%targetRotation)) / %this.owner.moveBehaviorCount;

      %this.owner.specialX = getWord(%startVelocity, 0) + %xcomponent;
      %this.owner.specialY = getWord(%startVelocity, 1) - %ycomponent;
    }
	}
}
