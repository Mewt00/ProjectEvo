//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(WanderAroundBehavior))
{
  %template = new BehaviorTemplate(WanderAroundBehavior);
   
  %template.friendlyName = "Wander Around";
  %template.behaviorType = "AI";
  %template.description  = "Wander around the field randomly";

  %template.addBehaviorField(turnDelay, "When to change direction.(s)", int, 2);
  %template.addBehaviorField(numDires, "Number of directions allowed.", int, 4);
  %template.addBehaviorField(moveSpeed, "Mob move speed.", int, 5);
  %template.addBehaviorField(turnSpeed, "Mob turn speed.", int, 0);
  %template.addBehaviorField(honcho, "", bool, false);
  
  %template.addBehaviorField(specX, "", float, 0);
  %template.addBehaviorField(specY, "", float, 0);
}

function WanderAroundBehavior::onBehaviorAdd(%this)
{
  %this.owner.moveBehaviorCount++;
  %this.ticks = 0;

  if(%this.owner.moveBehaviorCount == 1)
    %this.honcho = true;
}

function WanderAroundBehavior::onUpdate(%this)
{
  if(%this.ticks < (%this.turnDelay * 30)){
    %this.ticks++;
    %this.owner.specialX = %this.owner.specialX + %this.specX;
    %this.owner.specialY = %this.owner.specialX - %this.specY;
    return;
  }else{
    %this.ticks = 0;
  }

  if(%this.honcho == true){
    %this.owner.specialX = 0;
    %this.owner.specialY = 0;
  }

  %startVelocity = %this.owner.specialX SPC %this.owner.specialY;
  %targetRotation = (getRandom(0, %this.numDires) * (360/%this.numDires));

  %this.specX = %this.moveSpeed * mSin(mDegToRad(%targetRotation)) / %this.owner.moveBehaviorCount;
  %this.specY = %this.moveSpeed * mCos(mDegToRad(%targetRotation)) / %this.owner.moveBehaviorCount;

  %this.owner.specialX = getWord(%startVelocity, 0) + %this.specX;
  %this.owner.specialY = getWord(%startVelocity, 1) - %this.specY;
}
