//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(AlignToJoystickBehavior))
{
   %template = new BehaviorTemplate(AlignToJoystickBehavior);
   
   %template.friendlyName = "Align To Joystick";
   %template.behaviorType = "Input";
   %template.description  = "Face the object to the direction a joystick is pointing";

   %template.addBehaviorField(xAxis, "The joystick's x-axis", keybind, "joystick0 rxaxis");
   %template.addBehaviorField(yAxis, "The joystick's y-axis", keybind, "joystick0 ryaxis");
   %template.addBehaviorField(turnSpeed, "The speed to rotate at (degrees per second). Use 0 to snap", float, 0.0);
   %template.addBehaviorField(rotationOffset, "The rotation offset (degrees)", float, 0.0);
}

function AlignToJoystickBehavior::onBehaviorAdd(%this)
{
   if (!isObject(moveMap))
      return;
   
   moveMap.bindObj(getWord(%this.xAxis, 0), getWord(%this.xAxis, 1), "moveX", %this);
   moveMap.bindObj(getWord(%this.yAxis, 0), getWord(%this.yAxis, 1), "moveY", %this);
   
   %this.xVal = 0;
   %this.yVal = 0;
}

function AlignToJoystickBehavior::moveX(%this, %val)
{
   %this.xVal = %val;
   %this.updateMovement();
}

function AlignToJoystickBehavior::moveY(%this, %val)
{
   %this.yVal = %val;
   %this.updateMovement();
}

function AlignToJoystickBehavior::updateMovement(%this)
{
   %targetRotation = mRadToDeg(mAtan(%this.yVal, %this.xVal)) + %this.rotationOffset;
   if (%this.turnSpeed == 0)
      %this.owner.rotation = %targetRotation;
   else
      %this.owner.rotateTo(%targetRotation, %this.turnSpeed);
}
