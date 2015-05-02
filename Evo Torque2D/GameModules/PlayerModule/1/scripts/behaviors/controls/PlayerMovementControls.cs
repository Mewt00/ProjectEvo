//-----------------------------------------------------------------------------
// Basic player controls & behaviors: Joystick maps to 8 keys, for 8 cardinal directions
//-----------------------------------------------------------------------------

if (!isObject(PlayerMovementControlsBehavior))
{
    %template = new BehaviorTemplate(PlayerMovementControlsBehavior);
	
	echo("ActionMap1:");
	echo(isObject(controlActionMap));
	
	if(isObject(controlActionMap))
	{
		controlActionMap.pop();
		controlActionMap.delete();
	}
	
	echo("ActionMap2:");
	echo(isObject(controlActionMap));
	
	new ActionMap(controlActionMap);
	
	echo("ActionMap3:");
	echo(isObject(controlActionMap));

	%template.friendlyName = "Shooter Controls";
	%template.behaviorType = "Input";
	%template.description  = "Shooter style movement control";

	%template.addBehaviorField(upKey, "Key to bind to upward movement", keybind, "keyboard W");
	%template.addBehaviorField(downKey, "Key to bind to downward movement", keybind, "keyboard S");
	%template.addBehaviorField(leftKey, "Key to bind to left movement", keybind, "keyboard A");
	%template.addBehaviorField(rightKey, "Key to bind to right movement", keybind, "keyboard D");
	
	%template.addBehaviorField(upRightKey, "Key to bind for up-right movement", keybind, "keyboard U");	//diagonal (for joy to key)
	%template.addBehaviorField(upLeftKey, "Key to bind to up-left movement", keybind, "keyboard Y");
	%template.addBehaviorField(downLeftKey, "Key to bind to down-left movement", keybind, "keyboard B");
	%template.addBehaviorField(downRightKey, "Key to bind to down-right movement", keybind, "keyboard N");

	%template.addBehaviorField(fireKey, "", keybind, "keyboard space");
	%template.addBehaviorField(meleeKey, "", keybind, "keyboard E");
	%template.addBehaviorField(dashKey, "", keybind, "keyboard Q");
	%template.addBehaviorField(blockKey, "", keybind, "keyboard F");
	
	controlActionMap.push();
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::onBehaviorAdd(%this)
{
	echo("ActionMap:");
	echo(isObject(controlActionMap));
    if (!isObject(controlActionMap))
       return;

	controlActionMap.bindObj(getWord(%this.upKey, 0), getWord(%this.upKey, 1), "moveUp", %this);
	echo("echo here");
	echo(getWord(%this.upKey, 0));
	echo(getWord(%this.upKey, 1));
	controlActionMap.bindObj(getWord(%this.downKey, 0), getWord(%this.downKey, 1), "moveDown", %this);
	controlActionMap.bindObj(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1), "moveLeft", %this);
	controlActionMap.bindObj(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1), "moveRight", %this);
	
	controlActionMap.bindObj(getWord(%this.upRightKey, 0), getWord(%this.upRightKey, 1), "moveUpRight", %this);
	controlActionMap.bindObj(getWord(%this.upLeftKey, 0), getWord(%this.upLeftKey, 1), "moveUpLeft", %this);
	controlActionMap.bindObj(getWord(%this.downLeftKey, 0), getWord(%this.downLeftKey, 1), "moveDownLeft", %this);
	controlActionMap.bindObj(getWord(%this.downRightKey, 0), getWord(%this.downRightKey, 1), "moveDownRight", %this);

	controlActionMap.bindObj("keyboard", %this.fireKey, "pressFire", %this);
	controlActionMap.bindObj("keyboard", %this.meleeKey, "pressMelee", %this);
	controlActionMap.bindObj("keyboard", %this.dashKey, "pressDash", %this);
	controlActionMap.bindObj("keyboard", %this.blockKey, "pressBlock", %this);
	
	//Xbox XInput mapping
	controlActionMap.bindObj("gamepad0", "thumblx", "LAnalogX", %this);
	controlActionMap.bindObj("gamepad0", "thumbly", "LAnalogY", %this);
	/*controlActionMap.bindObj("gamepad0", getWord(%this.downKey, 1), "moveDown", %this);
	controlActionMap.bindObj("gamepad0", getWord(%this.leftKey, 1), "moveLeft", %this);
	controlActionMap.bindObj("gamepad0", getWord(%this.rightKey, 1), "moveRight", %this);*/
	
		
	controlActionMap.bindObj("gamepad0", "triggerr", "pressFire", %this);

	%this.up = 0;
	%this.down = 0;
	%this.left = 0;
	%this.right = 0;
	
	%this.upRight = 0;
	%this.upLeft = 0;
	%this.downLeft = 0;
	%this.downRight = 0;
	
	
   // %this.setUpdateCallback(true);
	
	//shot barrel offset (instead of bullet coming out of center of player)	
	%this.barrelXoffset = 55*%this.owner.sizeRatio;
	%this.barrelYoffset = -42*%this.owner.sizeRatio;
	
	//blade offset (instead of strike effect coming out of center of player)	
	%this.bladeXoffset = 100*%this.owner.sizeRatio;
	%this.bladeYoffset = 30*%this.owner.sizeRatio;
	
	%this.wallCheckDist = 2;
	
	%this.fireHeld = false;
	%this.strikeHeld = false;
	
	/*
	//Print directional input
	%this.fontUp = new ImageFont();
	%this.fontUp.Image = "GameAssets:font";
	%this.fontUp.Text = %this.up;
	%this.fontUp.FontSize = "3 3";
	%this.fontUp.setPosition("0 16");
	%this.fontUp.TextAlignment = "Center";
	%this.fontUp.setBlendColor("1 1 0");
	%this.owner.getScene().add( %this.fontUp ); 
	
	%this.fontRight = new ImageFont();
	%this.fontRight.Image = "GameAssets:font";
	%this.fontRight.Text = %this.right;
	%this.fontRight.FontSize = "3 3";
	%this.fontRight.setPosition("16 0");
	%this.fontRight.TextAlignment = "Center";
	%this.fontRight.setBlendColor("1 1 0");
	%this.owner.getScene().add( %this.fontRight );
	
	%this.fontDown = new ImageFont();
	%this.fontDown.Image = "GameAssets:font";
	%this.fontDown.Text = %this.down;
	%this.fontDown.FontSize = "3 3";
	%this.fontDown.setPosition("0 -16");
	%this.fontDown.TextAlignment = "Center";
	%this.fontDown.setBlendColor("1 1 0");
	%this.owner.getScene().add( %this.fontDown );
	
	%this.fontLeft = new ImageFont();
	%this.fontLeft.Image = "GameAssets:font";
	%this.fontLeft.Text = %this.left;
	%this.fontLeft.FontSize = "3 3";
	%this.fontLeft.setPosition("-16 0");
	%this.fontLeft.TextAlignment = "Center";
	%this.fontLeft.setBlendColor("1 1 0");
	%this.owner.getScene().add( %this.fontLeft );
	*/
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::onBehaviorRemove(%this)
{
    if (!isObject(controlActionMap))
       return;

	//%this.owner.setUpdateCallback(true);
	
	//TODO::Are the Unbindings needed?

	controlActionMap.unbindObj(getWord(%this.upKey, 0), getWord(%this.upKey, 1), %this);
	controlActionMap.unbindObj(getWord(%this.downKey, 0), getWord(%this.downKey, 1), %this);
	controlActionMap.unbindObj(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1), %this);
	controlActionMap.unbindObj(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1), %this);
	
	controlActionMap.unbindObj(getWord(%this.upRightKey, 0), getWord(%this.upRightKey, 1), %this);
	controlActionMap.unbindObj(getWord(%this.upLeftKey, 0), getWord(%this.upLeftKey, 1), %this);
	controlActionMap.unbindObj(getWord(%this.downLeftKey, 0), getWord(%this.downLeftKey, 1), %this);
	controlActionMap.unbindObj(getWord(%this.downRightKey, 0), getWord(%this.downRightKey, 1), %this);

	controlActionMap.unbindObj("keyboard", %this.fireKey, %this);
	controlActionMap.unbindObj("keyboard", %this.meleeKey, %this);
	controlActionMap.unbindObj("keyboard", %this.dashKey, %this);
	controlActionMap.unbindObj("keyboard", %this.blockKey, %this);

	%this.up = 0;
	%this.down = 0;
	%this.left = 0;
	%this.right = 0;
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::onCollision(%this, %object, %collisionDetails)
{

}

//-----------------------------------------------------------------------------

function PlayerMovementControlsBehavior::onUpdate(%this)
{
	%this.updateMovement();
	
	if(%this.fireHeld)
	{
		%this.tryFire();
	}
	
	if(%this.strikeHeld)
	{
		%this.tryMelee();
	}
}

//------------------------------------------------------------------------------------
  
function PlayerMovementControlsBehavior::updateMovement(%this)
{	 
	if(! %this.owner.isDashing)
	{	
		
		%tempUp = %this.up;
		%tempDown = %this.down;
		%tempLeft = %this.left;
		%tempRight = %this.right;
			
		if(%this.upRight == 1)
		{
			%tempUp = 1;
			%tempDown = 0;
			%tempLeft = 0;
			%tempRight = 1;
		}
		else if(%this.upLeft == 1)
		{
			%tempUp = 1;
			%tempDown = 0;
			%tempLeft = 1;
			%tempRight = 0;
		}
		else if(%this.downLeft == 1)
		{
			%tempUp = 0;
			%tempDown = 1;
			%tempLeft = 1;
			%tempRight = 0;
		}
		else if(%this.downRight == 1)
		{
			%tempUp = 0;
			%tempDown = 1;
			%tempLeft = 0;
			%tempRight = 1;
		}
		
		if((mAbs(%tempRight - %tempLeft) + mAbs(%tempUp - %tempDown)) >= 2)		//combine (angle) movement scaled so its not faster
		{
			%cosRatio = 0.707;
			%goSpeed = %this.owner.walkSpeed * %cosRatio;
		}
		else
		{
			%goSpeed = %this.owner.walkSpeed;
		}
		
		if(isObject(%this.owner.blocker))
		{
			%goSpeed = %goSpeed*0.5;
		}
	
		%this.owner.setLinearVelocityX((%tempRight - %tempLeft) * %goSpeed);
		%this.owner.setLinearVelocityY((%tempUp - %tempDown) * %goSpeed);
	}	
	
	/*
	%this.fontUp.Text = %this.up;
	%this.fontRight.Text = %this.right;
	%this.fontDown.Text = %this.down;
	%this.fontLeft.Text = %this.left;
	*/
	
}

//------------------------------------------------------------------------------------
  
function PlayerMovementControlsBehavior::moveUp(%this, %val)
{
	if(%val == 1)
	{
		%this.up = 1;
	}
	else
	{
		%this.up = 0;
	}
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::moveDown(%this, %val)
{
    if(%val == 1)
	{
		%this.down = 1;
	}
	else
	{
		%this.down = 0;
	}
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::LAnalogY(%this, %val)
{
	//echo("Left analog stick --> up/down" @ %val);
	if(%val < -0.25)
	{
		%this.down = 1;
		//%this.up = 0;
	}
	else if(%val > 0.25)
	{
		%this.up = 1;	
		//%this.down = 0;
	}
	else
	{
		%this.up = 0;	
		%this.down = 0;
	}
	
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::moveLeft(%this, %val)
{
    if(%val == 1)
	{
		%this.left = 1;
	}
	else
	{
		%this.left = 0;
	}
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::moveRight(%this, %val)
{
    if(%val == 1)
	{
		%this.right = 1;
	}
	else
	{
		%this.right = 0;
	}
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::LAnalogX(%this, %val)
{
	//echo("Left analog stick --> right/left" @ %val);
	if(%val < -0.25)
	{
		%this.left = 1;
		//%this.right = 0;
	}
	else if(%val > 0.25)
	{
		%this.right = 1;
		//%this.left = 0;		
	}
	else
	{
		%this.right = 0;	
		%this.left = 0;
	}
	
}

//------------------------------------------------------------------------------------
  
function PlayerMovementControlsBehavior::moveUpRight(%this, %val)
{
	if(%val == 1)
	{
		%this.upRight = 1;
	}
	else
	{
		%this.upRight = 0;
	}
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::moveUpLeft(%this, %val)
{
    if(%val == 1)
	{
		%this.upLeft = 1;
	}
	else
	{
		%this.upLeft = 0;
	}
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::moveDownLeft(%this, %val)
{
    if(%val == 1)
	{
		%this.downLeft = 1;
	}
	else
	{
		%this.downLeft = 0;
	}
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::moveDownRight(%this, %val)
{
    if(%val == 1)
	{
		%this.downRight = 1;
	}
	else
	{
		%this.downRight = 0;
	}
}

//------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::pressFire(%this, %val)
{
	if(%val == 1)
	{
		%this.fireHeld = true;
	}
	else
	{
		%this.fireHeld = false;
	}
} 

function PlayerMovementControlsBehavior::tryFire(%this)
{
	if(getEventTimeLeft(%this.fireCooldownTime) <= 0)
	{
		// add a bullet to the arena
		%newBullet = new CompositeSprite()
		{
			class = "PlayerBullet";
			fireAngle = %this.owner.getAngle();
			owner = %this.owner;
		};
		
		%this.owner.rangedCount++;
		%this.owner.getScene().add( %newBullet );
		%this.owner.myShotsContainer.add( %newBullet );
		%newBullet.setPosition(%this.owner.getWorldPoint(%this.barrelXoffset, %this.barrelYoffset) );
		
		
		// add a muzzle flash to the arena
		%newFlash = new CompositeSprite()
		{
			class = "PlayerBulletMuzzleFlash";
			fireAngle = %this.owner.getAngle();
			owner = %this;
		};
		
		%this.owner.getScene().add( %newFlash );
		%newFlash.setPosition(%this.owner.getWorldPoint(%this.barrelXoffset + 23*%this.owner.sizeRatio, %this.barrelYoffset - 6*%this.owner.sizeRatio) );
		
		%this.fireCooldownTime = schedule(%this.owner.fireCooldown, 0, "PlayerMovementControlsBehavior::doNothing", %this);
	}
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::pressMelee(%this, %val)
{
	if(%val == 1)
	{
		%this.strikeHeld = true;
	}
	else
	{
		%this.strikeHeld = false;
	}
} 

function PlayerMovementControlsBehavior::tryMelee(%this)
{
	if(getEventTimeLeft(%this.strikeCooldownTime) <= 0)
	{
		// add a strike effect to the arena
		%newStriker = new CompositeSprite()
		{
			class = "PlayerStrike";
			strikeAngle = %this.owner.getAngle();
		};
			
		%this.owner.meleeCount++;
		%this.owner.getScene().add( %newStriker );
		
		%newStriker.setPosition(%this.owner.getWorldPoint(%this.bladeXoffset, %this.bladeYoffset) );
		
		%this.owner.setSpriteAnimation("GameAssets:playerStrikingAnim", 0);
		%this.owner.setSpriteSize(320*%this.owner.sizeRatio, 164*%this.owner.sizeRatio);
		%this.owner.setSpriteLocalPosition(0, 17*%this.owner.sizeRatio);
		%this.owner.schedule(250, "setupSprite");
		
		%this.strikeCooldownTime = schedule(%this.owner.strikeCooldown, 0, "PlayerMovementControlsBehavior::doNothing", %this);
	}
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::pressDash(%this, %val)
{
	if(%val == 1)
	{
		if((! %this.owner.isDashing) && (! %this.owner.tarred))
		{
			
			%newDashTrail = new CompositeSprite()
			{
				class = "PlayerDash";
				dashAngle = %this.owner.getAngle();
			};
				
			%this.owner.getScene().add( %newDashTrail );
			%newDashTrail.setPosition(%this.owner.getWorldPoint(-20*%this.owner.sizeRatio, 0) );
			
			
			%this.owner.isDashing = true;
			%this.owner.currDashDirection = %this.owner.getAngle();
			%this.owner.setLinearVelocityPolar(%this.owner.getAngle(), %this.owner.dashSpeed);	
			
			%this.dashSchedule = schedule(%this.owner.dashLength, 0, "PlayerMovementControlsBehavior::endDash", %this);
			
			%this.owner.dashCount++;
		
			%this.owner.setSpriteBlendAlpha(0.8);
		}
	}
} 

function PlayerMovementControlsBehavior::endDash(%this)
{
	%this.owner.isDashing = false;	
	%this.owner.setLinearVelocityPolar(0, 0);	
	%this.owner.setSpriteBlendAlpha(1);
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::pressBlock(%this, %val)
{
	if(%val == 1)
	{
		%this.owner.blocker = new CompositeSprite()
		{
			class = "PlayerBlock";
			blockAngle = %this.owner.getAngle();
			owner = %this.owner;
		};
		
		if(%this.owner.blockCooldown < %this.owner.blocker.maxDamage - %this.owner.blockCooldownRefresh )
		{
			%this.owner.blockCount++;
			%this.owner.blocker.setPosition(%this.owner.getWorldPoint(0, 0) );
			// add a block effect to the arena
			%this.owner.getScene().add( %this.owner.blocker );
			
			%this.owner.blocker.takeDamage( %this.owner.blockCooldown );
			
			%this.blockTickSchedule = schedule(%this.owner.blockTickTime, 0, "PlayerMovementControlsBehavior::blockTick", %this);
			
		}
		else
		{
			if(isObject(%this.owner.blocker))
			{
				%this.owner.blocker.takeDamage( %this.owner.blockCooldown );
				%this.owner.blocker.safeDelete();
			}
		}
	}
	else if(%val == 0)
	{
		if(isObject(%this.owner.blocker))
		{
			%this.owner.blocker.safeDelete();
		}
	}
} 

function PlayerMovementControlsBehavior::blockTick(%this)
{
	if(isObject(%this.owner.blocker))
	{
		%this.owner.blockCount++;
		%this.blockTickSchedule = schedule(%this.owner.blockTickTime, 0, "PlayerMovementControlsBehavior::blockTick", %this);
	}
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::doNothing(%this)
{
	//Empty function for timer calls
}
