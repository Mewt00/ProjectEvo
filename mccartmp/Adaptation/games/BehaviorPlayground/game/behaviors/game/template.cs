//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(TemplateBehavior))
{
   %template = new BehaviorTemplate(TemplateBehavior);
   
   %template.friendlyName = "Template";
   %template.behaviorType = "Game";
   %template.description  = "Disables an object unless it is a clone";
}

function TemplateBehavior::onLevelLoaded(%this, %scenegraph)
{
   %this.owner.enabled = false;
}
