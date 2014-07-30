//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(TankControlsBehavior))
{
   %template = new BehaviorTemplate(TankControlsBehavior);
   
   %template.friendlyName = "Tank Controls";
   %template.behaviorType = "Input";
   %template.description  = "Tank style movement control";
   
   %template.addBehaviorField(upKey, "Key to bind to forward movement", keybind, "keyboard up");
   %template.addBehaviorField(downKey, "Key to bind to backward movement", keybind, "keyboard down");
   %template.addBehaviorField(leftKey, "Key to bind to rotate left", keybind, "keyboard left");
   %template.addBehaviorField(rightKey, "Key to bind to rotate right", keybind, "keyboard right");
   
   %template.addBehaviorField(moveSpeed, "Forward acceleration", float, 15.0);
   %template.addBehaviorField(turnSpeed, "Angular velocity", float, 45.0);
}

function TankControlsBehavior::onBehaviorAdd(%this)
{
   if (!isObject(moveMap))
      return;
   
   %this.owner.enableUpdateCallback();
   
   moveMap.bindObj(getWord(%this.upKey, 0), getWord(%this.upKey, 1), "moveUp", %this);
   moveMap.bindObj(getWord(%this.downKey, 0), getWord(%this.downKey, 1), "moveDown", %this);
   moveMap.bindObj(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1), "moveLeft", %this);
   moveMap.bindObj(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1), "moveRight", %this);
   
   %this.up = 0;
   %this.down = 0;
   %this.left = 0;
   %this.right = 0;
}

function TankControlsBehavior::onBehaviorRemove(%this)
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

function TankControlsBehavior::onUpdate(%this)
{
   %this.owner.setLinearVelocityPolar(%this.owner.rotation, (%this.up - %this.down) * %this.moveSpeed);
   %this.owner.setAngularVelocity((%this.right - %this.left) * %this.turnSpeed);
}

function TankControlsBehavior::moveUp(%this, %val)
{
   %this.up = %val;
}

function TankControlsBehavior::moveDown(%this, %val)
{
   %this.down = %val;
}

function TankControlsBehavior::moveLeft(%this, %val)
{
   %this.left = %val;
}

function TankControlsBehavior::moveRight(%this, %val)
{
   %this.right = %val;
}
