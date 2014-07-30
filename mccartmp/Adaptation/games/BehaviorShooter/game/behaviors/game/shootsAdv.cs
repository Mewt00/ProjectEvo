//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(ShootsBehaviorAdv))
{
   %template = new BehaviorTemplate(ShootsBehaviorAdv);
   
   %template.friendlyName = "Shoots Advanced";
   %template.behaviorType = "Game";
   %template.description  = "Shoots an object (with more controls)";

   %template.addBehaviorField(projectile, "The projectile to clone and shoot", object, "", t2dSceneObject);
   %template.addBehaviorField(fireKey, "The key to fire the projectile with", keybind, "keyboard space");
   %template.addBehaviorField(fireRate, "The rate to fire when the fire key is held down (seconds)", float, "0.25");
   %template.addBehaviorField(projectileSpeed, "The speed of projectiles (world units per second)", float, "150");
   %template.addBehaviorField(fireAngleOffset, "Direction of motion angle offset relative to the owner's rotation (degrees)", float, "0");
   %template.addBehaviorField(rotationOffset, "Rotation offset of the projectile itself relative to the owner's rotation (degrees)", float, "0");
}

function ShootsBehaviorAdv::onBehaviorAdd(%this)
{
   %this.tryingToFire = false;
   
   if (isObject(moveMap))
      moveMap.bindObj(getWord(%this.fireKey, 0), getWord(%this.fireKey, 1), "fire", %this);
}

function ShootsBehaviorAdv::onBehaviorRemove(%this)
{
   if (isObject(moveMap))
      moveMap.unbindObj(getWord(%this.fireKey, 0), getWord(%this.fireKey, 1), %this);
}

function ShootsBehaviorAdv::fire(%this, %val)
{
   //This can be a bit confusing, but it's to ensure that the firing mechanism
   // works as expected, so that the player cannot fire more bullets by tapping the
   // key really fast (the waitSchedule ensures that) - when the wait finishes, it
   // uses the fireCond method to see if the key was pressed again during the wait.
   %this.tryingToFire = %val;
   
   if (%val == 0) 
   {
      cancel(%this.fireSchedule);
      if( !isEventPending(%this.waitSchedule))
         %this.waitSchedule = %this.schedule( %this.fireRate * 1000, "fireCond");
      return;
   }
   
   if (isEventPending(%this.fireSchedule) || isEventPending( %this.waitSchedule))
      return;
   
   if (!isObject(%this.projectile) || !%this.owner.enabled || !%this.owner.getVisible())
      return;
   
   %projectile = %this.projectile.cloneWithBehaviors();
   
   %projectile.setPosition(%this.owner.position);
   %projectile.setRotation(%this.owner.rotation + %this.rotationOffset);
   %projectile.setLinearVelocityPolar(%this.owner.rotation + %this.fireAngleOffset, %this.projectileSpeed);
   
   if (!isEventPending(%this.fireSchedule))
      %this.fireSchedule = %this.schedule(%this.fireRate * 1000, "fire", 1);
}

function ShootsBehaviorAdv::fireCond(%this)
{
   if( %this.tryingToFire )
      %this.fire( %this.tryingToFire );
      
}
