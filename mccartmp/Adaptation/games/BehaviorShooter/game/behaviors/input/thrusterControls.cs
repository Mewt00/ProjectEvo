//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(ThrusterControlsBehavior))
{
   %template = new BehaviorTemplate(ThrusterControlsBehavior);
   
   %template.friendlyName = "Thruster Controls";
   %template.behaviorType = "Input";
   %template.description  = "Thruster style movement control";
   
   %template.addBehaviorField(upKey, "Key to bind to vertical thrust", keybind, "keyboard up");
   %template.addBehaviorField(downKey, "Key to bind to vertical thrust", keybind, "keyboard down");
   %template.addBehaviorField(leftKey, "Key to bind to horizontal thrust", keybind, "keyboard left");
   %template.addBehaviorField(rightKey, "Key to bind to horizontal thrust", keybind, "keyboard right");
   
   %template.addBehaviorField(horizontalThrust, "Horizontal thrust force", float, 200.0);
   %template.addBehaviorField(verticalThrust, "Vertical thrust force", float, 200.0);
   %template.addBehaviorField(damping, "Amount to damp movement (percentage of velocity per second)", float, 3.0);
}

function ThrusterControlsBehavior::onBehaviorAdd(%this)
{
   if (!isObject(moveMap))
      return;
   
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

function ThrusterControlsBehavior::onBehaviorRemove(%this)
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

function ThrusterControlsBehavior::updateMovement(%this)
{
   %this.owner.setConstantForceX((%this.right - %this.left) * %this.horizontalThrust);
   %this.owner.setConstantForceY((%this.down - %this.up) * %this.verticalThrust);
}

function ThrusterControlsBehavior::moveUp(%this, %val)
{
   %this.up = %val;
   %this.updateMovement();
}

function ThrusterControlsBehavior::moveDown(%this, %val)
{
   %this.down = %val;
   %this.updateMovement();
}

function ThrusterControlsBehavior::moveLeft(%this, %val)
{
   %this.left = %val;
   %this.updateMovement();
}

function ThrusterControlsBehavior::moveRight(%this, %val)
{
   %this.right = %val;
   %this.updateMovement();
}
