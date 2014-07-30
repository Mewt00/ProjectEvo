//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(CollisionParticlesBehavior))
{
   %template = new BehaviorTemplate(CollisionParticlesBehavior);
   
   %template.friendlyName = "Collision Particles";
   %template.behaviorType = "Effects";
   %template.description  = "Play a particle effect when the object collides";

   %template.addBehaviorField(particleEffect, "The effect to use", object, "", t2dParticleEffect);
}

function CollisionParticlesBehavior::onBehaviorAdd(%this)
{
   %this.owner.collisionCallback = true;
}

function CollisionParticlesBehavior::onCollision(%this, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts)
{
   %this.explode(getWords(%contacts, 0, 1));
}

function CollisionParticlesBehavior::explode(%this, %position)
{
   if (isObject(%this.particleEffect))
   {
      %explosion = %this.particleEffect.cloneWithBehaviors();
      %explosion.position = %position;
      %explosion.setEffectLifeMode("Kill", 1.0);
      %explosion.playEffect();
   }
}
