//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(AsteroidsControlsBehavior))
{
   %template = new BehaviorTemplate(AsteroidsControlsBehavior);
   
   %template.friendlyName = "Asteroids Controls";
   %template.behaviorType = "Input";
   %template.description  = "Asteroids style movement control";
   
   %template.addBehaviorField(upKey, "Key to bind to acceleration", keybind, "keyboard up");
   %template.addBehaviorField(downKey, "Key to bind to deceleration", keybind, "keyboard down");
   %template.addBehaviorField(leftKey, "Key to bind to rotate left", keybind, "keyboard left");
   %template.addBehaviorField(rightKey, "Key to bind to rotate right", keybind, "keyboard right");
   
   %template.addBehaviorField(acceleration, "Forward acceleration (world units per second)", float, 2.0);
   %template.addBehaviorField(turnSpeed, "Velocity of turning (degrees per second)", float, 120.0);
   %template.addBehaviorField(damping, "Amount to damp movement (percentage of velocity per second)", float, 1.0);
}

function AsteroidsControlsBehavior::onBehaviorAdd(%this)
{
   if (!isObject(moveMap))
      return;
   
   %this.owner.enableUpdateCallback();
   %this.owner.setDamping(%this.damping);
   
   moveMap.bindObj(getWord(%this.upKey, 0), getWord(%this.upKey, 1), "moveUp", %this);
   moveMap.bindObj(getWord(%this.downKey, 0), getWord(%this.downKey, 1), "moveDown", %this);
   moveMap.bindObj(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1), "moveLeft", %this);
   moveMap.bindObj(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1), "moveRight", %this);
   
   %this.up = 0;
   %this.down = 0;
   %this.left = 0;
   %this.right = 0;
}

function AsteroidsControlsBehavior::onBehaviorRemove(%this)
{
   if (!isObject(moveMap))
      return;

   %this.owner.disableUpdateCallback();
   
   moveMap.unbindObj(getWord(%this.upKey, 0), getWord(%this.upKey, 1), %this);
   moveMap.unbindObj(getWord(%this.downKey, 0), getWord(%this.downKey, 1), %this);
   moveMap.unbindObj(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1), %this);
   moveMap.unbindObj(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1), %this);
   
   %this.up = 0;
   %this.down = 0;
   %this.left = 0;
   %this.right = 0;
}

function AsteroidsControlsBehavior::onUpdate(%this)
{
   %this.owner.setImpulseForcePolar(%this.owner.rotation, (%this.up - %this.down) * %this.acceleration);
	%this.owner.setAngularVelocity((%this.right - %this.left) * %this.turnSpeed);
}

function AsteroidsControlsBehavior::moveUp(%this, %val)
{
   %this.up = %val;
}

function AsteroidsControlsBehavior::moveDown(%this, %val)
{
   %this.down = %val;
}

function AsteroidsControlsBehavior::moveLeft(%this, %val)
{
   %this.left = %val;
}

function AsteroidsControlsBehavior::moveRight(%this, %val)
{
   %this.right = %val;
}
