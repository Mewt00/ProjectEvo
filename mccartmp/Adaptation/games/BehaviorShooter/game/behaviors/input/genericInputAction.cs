//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(GenericInputActionBehavior))
{
   %template = new BehaviorTemplate(GenericInputActionBehavior);
   
   %template.friendlyName = "Generic Input Action";
   %template.behaviorType = "Input";
   %template.description = "Executes a command when the specified key is pressed";
   
   %template.addBehaviorField(actionKey, "The key that triggers the command", keybind, "keyboard enter");
   %template.addBehaviorField(command, "The Torque Script function to call", string, "");
}

function GenericInputActionBehavior::onAddToScene(%this, %scenegraph)
{
   if (!isObject(moveMap))
      return;
   
   moveMap.bind(getWord(%this.actionKey, 0), getWord(%this.actionKey, 1), %this.command);
}
