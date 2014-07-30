//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(MouseDownDamageBehavior))
{
   %template = new BehaviorTemplate(MouseDownDamageBehavior);
   
   %template.friendlyName = "Mouse Down Damage";
   %template.behaviorType = "Game";
   %template.description  = "Inflict damage when the mouse is clicked";

   %template.addBehaviorField(damage, "The amount of damage to apply", int, 10);
}
function MouseDownDamageBehavior::onBehaviorAdd(%this)
{
   %this.owner.setUseMouseEvents(true);
}

function MouseDownDamageBehavior::onMouseDown(%this, %modifier, %worldPos)
{
   %takesDamage = %this.owner.getBehavior("TakesDamageBehavior");
   if (!isObject(%takesDamage))
      return;
   
   %takesDamage.takeDamage(%this.damage, %this);
}
