//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(CameraMoveBehavior))
{
   %template = new BehaviorTemplate(CameraMoveBehavior);
   
   %template.friendlyName = "Camera Move";
   %template.behaviorType = "Effects";
   %template.description  = "Move the camera to this object's world limits when the object is clicked";

   %template.addBehaviorField(moveTime, "The amount of time for the move to take (seconds)", float, 1.0);
}

function CameraMoveBehavior::onBehaviorAdd(%this)
{
   %this.owner.setUseMouseEvents(true);
}

function CameraMoveBehavior::onMouseUp(%this, %modifier, %worldPos)
{
   if (!isObject(sceneWindow2d))
      return;
   
   sceneWindow2D.setTargetCameraArea(getWords(%this.owner.getWorldLimit(), 1, 4));
   sceneWindow2D.startCameraMove(%this.moveTime);
}
