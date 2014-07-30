//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(FaceMouseBehavior))
{
   %template = new BehaviorTemplate(FaceMouseBehavior);
   
   %template.friendlyName = "Face mouse";
   %template.behaviorType = "Input";
   %template.description  = "Set the object to face the mouse";

   %template.addBehaviorField(rotationOffset, "The rotation offset (degrees)", float, 0.0);
}

function FaceMouseBehavior::onBehaviorAdd(%this)
{
   %this.owner.setUpdateCallback(true);
}

function FaceMouseBehavior::onUpdate(%this)
{
	if(! %this.owner.isDashing)
	{
		%targetRotation = %this.rotationOffset + Vector2AngleToPoint(%this.owner.getPosition(), mainWindow.getMousePosition());
		%this.owner.setAngle(%targetRotation);
	}
	else
	{
		%this.owner.setAngle(%this.owner.currDashDirection);
	}
}
