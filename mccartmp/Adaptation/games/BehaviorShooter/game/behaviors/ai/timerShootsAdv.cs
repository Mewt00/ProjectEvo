//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(TimerShootsAdvBehavior))
{
   %template = new BehaviorTemplate(TimerShootsAdvBehavior);
   
   %template.friendlyName = "Timer Shoots Advanced";
   %template.behaviorType = "AI";
   %template.description  = "Set the object to shoot on a timer (with extra controls)";

   %template.addBehaviorField(projectile, "The projectile to clone and shoot", object, "", t2dSceneObject);
   %template.addBehaviorField(fireRate, "The rate to fire shots (seconds)", float, "0.25");
   %template.addBehaviorField(fireStartDelay, "Time before first shot (seconds)", float, "0.5");
   %template.addBehaviorField(fireRateVariance, "The variance in the fire rate (seconds)", float, "0.2");
   %template.addBehaviorField(projectileSpeed, "The speed of projectiles (world units per second)", float, "150");
   %template.addBehaviorField(fireAngleOffset, "Direction of motion angle offset relative to the owner's rotation (degrees)", float, "0");
   %template.addBehaviorField(rotationOffset, "Rotation offset of the projectile itself relative to the owner's rotation (degrees)", float, "0");
}

function TimerShootsAdvBehavior::onAddToScene(%this, %scenegraph)
{
   %this.schedule(%this.genDelay() + %this.fireStartDelay * 1000, "fire");
}

function TimerShootsAdvBehavior::fire(%this)
{
   if (!isObject(%this.projectile) || !%this.owner.enabled)
      return;
   
   %projectile = %this.projectile.cloneWithBehaviors();
   
   %projectile.setPosition(%this.owner.position);
   %projectile.setRotation(%this.owner.rotation + %this.rotationOffset);
   %projectile.setLinearVelocityPolar(%this.owner.rotation + %this.fireAngleOffset, %this.projectileSpeed);
   
   %this.schedule(%this.genDelay(), "fire");
      
}

function TimerShootsAdvBehavior::genDelay(%this)
{
   %min = (%this.fireRate - %this.fireRateVariance) * 1000;
   %max = (%this.fireRate + %this.fireRateVariance) * 1000;
   %random = getRandom(%min, %max);
   
   if( %random < 0 )
      %random = 55;   
   
   return %random;
}
