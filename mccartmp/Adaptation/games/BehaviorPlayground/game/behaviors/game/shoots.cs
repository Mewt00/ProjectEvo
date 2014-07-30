//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(ShootsBehavior))
{
   %template = new BehaviorTemplate(ShootsBehavior);
   
   %template.friendlyName = "Shoots";
   %template.behaviorType = "Game";
   %template.description  = "Shoots an object";

   %template.addBehaviorField(projectile, "The projectile to clone and shoot", object, "", t2dSceneObject);
   %template.addBehaviorField(fireKey, "The key to fire the projectile with", keybind, "keyboard space");
   %template.addBehaviorField(fireRate, "The rate to fire when the fire key is held down (seconds)", float, "0.25");
   %template.addBehaviorField(projectileSpeed, "The speed of projectiles (world units per second)", float, "150");
}

function ShootsBehavior::onBehaviorAdd(%this)
{
   if (isObject(moveMap))
      moveMap.bindObj(getWord(%this.fireKey, 0), getWord(%this.fireKey, 1), "fire", %this);
}

function ShootsBehavior::onBehaviorRemove(%this)
{
   if (isObject(moveMap))
      moveMap.unbindObj(getWord(%this.fireKey, 0), getWord(%this.fireKey, 1), %this);
}

function ShootsBehavior::fire(%this, %val)
{
   if (%val == 0)
   {
      cancel(%this.fireSchedule);
      return;
   }
   
   if (!isObject(%this.projectile))
      return;
   
   %projectile = %this.projectile.cloneWithBehaviors();
   
   %projectile.setPosition(%this.owner.position);
   %projectile.setRotation(%this.owner.rotation);
   %projectile.setLinearVelocityPolar(%this.owner.rotation, %this.projectileSpeed);
   
   if (!isEventPending(%this.fireSchedule))
      %this.fireSchedule = %this.schedule(%this.fireRate * 1000, "fire", 1);
}
