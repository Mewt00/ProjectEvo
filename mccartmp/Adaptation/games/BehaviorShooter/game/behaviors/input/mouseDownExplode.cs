//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(MouseDownExplodeBehavior))
{
   %template = new BehaviorTemplate(MouseDownExplodeBehavior);
   
   %template.friendlyName = "Mouse Down Explode";
   %template.behaviorType = "Input";
   %template.description  = "Clicking on the object causes it to explode";
   
   %template.addBehaviorField(particleEffect, "The particle effect object to use", object, "", "t2dParticleEffect");
   %template.addBehaviorField(explodeAtMouse, "Place the explosion at the mouse point rather than the object center", bool, false);
   %template.addBehaviorField(deleteOwner, "Deletes the object when the explosion occurs", bool, true);
}

function MouseDownExplodeBehavior::onBehaviorAdd(%this)
{
   %this.owner.setUseMouseEvents(true);
}

function MouseDownExplodeBehavior::onMouseDown(%this, %modifier, %worldPos)
{
   %position = %this.owner.position;
   if (%this.explodeAtMouse)
      %position = %worldPos;
   
   %this.explode(%position);
   
   if (%this.deleteOwner)
      %this.owner.safeDelete();
}

function MouseDownExplodeBehavior::explode(%this, %worldPos)
{
   if (!isObject(%this.particleEffect))
      return;
   
   %explosion = %this.particleEffect.cloneWithBehaviors();
   %explosion.position = %worldPos;
   %explosion.setEffectLifeMode("Kill", 1.0);
   %explosion.playEffect();
}
