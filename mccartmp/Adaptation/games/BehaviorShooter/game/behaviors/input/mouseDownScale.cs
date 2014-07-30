//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(MouseDownScaleBehavior))
{
   %template = new BehaviorTemplate(MouseDownScaleBehavior);
   
   %template.friendlyName = "Mouse Down Scale";
   %template.behaviorType = "Input";
   %template.description  = "Scale the object on mouse down";

   %template.addBehaviorField(xScaleSpeed, "The speed at which the object will scale horizontally (world units per second)", float, 5.0);
   %template.addBehaviorField(yScaleSpeed, "The speed at which the object will scale vertically (world units per second)", float, 5.0);
   %template.addBehaviorField(resetOnMouseUp, "Reset the scale when the mouse is released", bool, 0);
}

function MouseDownScaleBehavior::onBehaviorAdd(%this)
{
   %this.startSize = %this.owner.size;
   %this.owner.setUseMouseEvents(true);
}

function MouseDownScaleBehavior::onMouseDown(%this, %modifier, %worldPos)
{
   %this.startSize = %this.owner.size;
   %this.owner.enableUpdateCallback();
}

function MouseDownScaleBehavior::onMouseUp(%this,%modifier,%worldPos, %a1, %a2, %a3)
{
   %this.owner.disableUpdateCallback();
   if(%this.resetOnMouseUp)
      %this.owner.size = %this.startSize;
}

function MouseDownScaleBehavior::onMouseLeave(%this,%modifier,%worldPos, %a1, %a2, %a3)
{
   %this.owner.disableUpdateCallback();
   if(%this.resetOnMouseUp)
      %this.owner.size = %this.startSize;
}

function MouseDownScaleBehavior::onUpdate(%this)
{
   %newX = %this.owner.size.x + (%this.xScaleSpeed * 0.032);
   %newY = %this.owner.size.y + (%this.yScaleSpeed * 0.032);
   %this.owner.size = %newX SPC %newY;
}
