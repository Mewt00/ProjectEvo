//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(MouseDownRotateBehavior))
{
   %template = new BehaviorTemplate(MouseDownRotateBehavior);
   
   %template.friendlyName = "Mouse Down Rotate";
   %template.behaviorType = "Input";
   %template.description  = "Rotate the object on mouse down";

   %template.addBehaviorField(rotateSpeed, "The speed at which the object will rotate (degrees per second)", float, 45.0);
   %template.addBehaviorField(resetOnMouseUp, "Reset the rotation when the mouse is released", bool, 0);
}

function MouseDownRotateBehavior::onBehaviorAdd(%this)
{
   %this.startRotation = %this.owner.rotation;
   %this.owner.setUseMouseEvents(true);
}

function MouseDownRotateBehavior::onMouseDown(%this, %modifier, %worldPos)
{
   %this.startRotation = %this.owner.rotation;
   %this.owner.setAngularVelocity(%this.rotateSpeed);
}

function MouseDownRotateBehavior::onMouseUp(%this,%modifier,%worldPos, %a1, %a2, %a3)
{
   %this.owner.setAngularVelocity(0);
   if(%this.resetOnMouseUp)
      %this.owner.rotation = %this.startRotation;
}

function MouseDownRotateBehavior::onMouseLeave(%this,%modifier,%worldPos, %a1, %a2, %a3)
{
   %this.owner.setAngularVelocity(0);
   if(%this.resetOnMouseUp)
      %this.owner.rotation = %this.startRotation;
}
