//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(MouseDownConstantForceBehavior))
{
   %template = new BehaviorTemplate(MouseDownConstantForceBehavior);
   
   %template.friendlyName = "Mouse Down Constant Force";
   %template.behaviorType = "Input";
   %template.description  = "Clicking on the object applies a constant force to it";
   
   %template.addBehaviorField(xForce, "Force to apply in the x direction", float, 10.0);
   %template.addBehaviorField(yForce, "Force to apply in the y direction", float, 10.0);
}

function MouseDownConstantForceBehavior::onBehaviorAdd(%this)
{
   %this.owner.setUseMouseEvents(true);
}

function MouseDownConstantForceBehavior::onMouseDown(%this, %modifier, %worldPos)
{
   %this.owner.setConstantForce(%this.xForce, %this.yForce);
}
