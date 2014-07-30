//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(TimerShootsBehavior))
{
   %template = new BehaviorTemplate(TimerShootsBehavior);
   
   %template.friendlyName = "Timer Shoots";
   %template.behaviorType = "AI";
   %template.description  = "Set the object to shoot on a timer";

   %template.addBehaviorField(projectile, "The projectile to clone and shoot", object, "", t2dSceneObject);
   %template.addBehaviorField(fireRate, "The rate to fire shots (seconds)", float, "0.25");
   %template.addBehaviorField(fireRateVariance, "The variance in the fire rate (seconds)", float, "0.2");
   %template.addBehaviorField(projectileSpeed, "The speed of projectiles (world units per second)", float, "150");
}

function TimerShootsBehavior::onAddToScene(%this, %scenegraph)
{
   %this.schedule(%this.fireRate, "fire");
}

function TimerShootsBehavior::fire(%this)
{
   if (!isObject(%this.projectile))
      return;
   
   %projectile = %this.projectile.cloneWithBehaviors();
   
   %projectile.setPosition(%this.owner.position);
   %projectile.setRotation(%this.owner.rotation);
   %projectile.setLinearVelocityPolar(%this.owner.rotation, %this.projectileSpeed);
   
   %min = (%this.fireRate - %this.fireRateVariance) * 1000;
   %max = (%this.fireRate + %this.fireRateVariance) * 1000;
   %random = getRandom(%min, %max);
   %this.schedule(%random, "fire");
}
