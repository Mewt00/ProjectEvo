//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(WorldLimitParticlesBehavior))
{
   %template = new BehaviorTemplate(WorldLimitParticlesBehavior);
   
   %template.friendlyName = "World Limit Particles";
   %template.behaviorType = "Effects";
   %template.description  = "Play a particle effect when the object hits its world limit";

   %template.addBehaviorField(particleEffect, "The effect to use", object, "", t2dParticleEffect);
}

function WorldLimitParticlesBehavior::onBehaviorAdd(%this)
{
   %this.owner.worldLimitCallback = true;
}

function WorldLimitParticlesBehavior::onWorldLimit(%this, %limitMode, %limit)
{
   switch$ (%limit)
   {
      case "left":
         %this.explode(getWord(%this.owner.getWorldLimit(), 1) SPC %this.owner.position.y);
         
      case "right":
         %this.explode(getWord(%this.owner.getWorldLimit(), 3) SPC %this.owner.position.y);

      case "top":
         %this.explode(%this.owner.position.x SPC getWord(%this.owner.getWorldLimit(), 2));

      case "bottom":
         %this.explode(%this.owner.position.x SPC getWord(%this.owner.getWorldLimit(), 4));
   }
}

function WorldLimitParticlesBehavior::explode(%this, %position)
{
   if (isObject(%this.particleEffect))
   {
      %explosion = %this.particleEffect.cloneWithBehaviors();
      %explosion.position = %position;
      %explosion.setEffectLifeMode("Kill", 1.0);
      %explosion.playEffect();
   }
}
