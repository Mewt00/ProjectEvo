//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(DealsDamageAdvBehavior))
{
   %template = new BehaviorTemplate(DealsDamageAdvBehavior);
   
   %template.friendlyName = "Deals Damage Advanced";
   %template.behaviorType = "Game";
   %template.description  = "Set the object to deal damage to TakesDamage objects it collides with";

   %template.addBehaviorField(strength, "The amount of damage the object deals", int, 10);
   %template.addBehaviorField(deleteOnHit, "Delete the object when it collides", bool, 1);
   %template.addBehaviorField(explodeEffect, "The particle effect to play on collision", object, "", t2dParticleEffect);
}

function DealsDamageAdvBehavior::onBehaviorAdd(%this)
{
   %this.owner.collisionCallback = true;
   %this.owner.collisionActiveSend = true;
}

function DealsDamageAdvBehavior::dealDamage(%this, %amount, %victim)
{
   %takesDamage = %victim.getBehavior("TakesDamageAdvBehavior");
   if (!isObject(%takesDamage))
      return; 
        
   %takesDamage.takeDamage(%amount, %this.owner);
}

function DealsDamageAdvBehavior::explode(%this, %position)
{
   if (isObject(%this.explodeEffect))
   {
      %explosion = %this.explodeEffect.cloneWithBehaviors();
      %explosion.position = %position;
      %explosion.setEffectLifeMode("Kill", 1.0);
      %explosion.playEffect();
   }
}

function DealsDamageAdvBehavior::onCollision(%this, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts)
{
   %this.dealDamage(%this.strength, %dstObj);
   
   if (%this.deleteOnHit)
   {
      %this.explode(getWords(%contacts, 0, 1));
      %this.owner.safeDelete();
   }
}
