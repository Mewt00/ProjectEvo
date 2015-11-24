//-----------------------------------------------------------------------------
// Basic player controls & behaviors: Joystick maps to 8 keys, for 8 cardinal directions
//-----------------------------------------------------------------------------

if (!isObject(PlayerMovementControlsBehavior))
{
	echo("controlActionMap exec");
	
    %template = new BehaviorTemplate(PlayerMovementControlsBehavior);
	
	/*if(isObject(controlActionMap))
	{
		echo("controlActionMap being popped and deleted");
		controlActionMap.pop();
		controlActionMap.delete();
	}*/
	
	//new ActionMap(controlActionMap);
	
	%template.friendlyName = "Shooter Controls";
	%template.behaviorType = "Input";
	%template.description  = "Shooter style movement control";

	%template.addBehaviorField(upKey, "Key to bind to upward movement", keybind, "keyboard W");
	%template.addBehaviorField(downKey, "Key to bind to downward movement", keybind, "keyboard S");
	%template.addBehaviorField(leftKey, "Key to bind to left movement", keybind, "keyboard A");
	%template.addBehaviorField(rightKey, "Key to bind to right movement", keybind, "keyboard D");
	
	%template.addBehaviorField(blockKey, "", keybind, "keyboard F");
	%template.addBehaviorField(dashKey, "", keybind, "keyboard space");
	
	//controlActionMap.push();
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::onBehaviorAdd(%this)
{
	echo("ActionMap isObject:");
	echo(isObject(controlActionMap));
    if (!isObject(controlActionMap))
       return;

	controlActionMap.bindObj(getWord(%this.upKey, 0), getWord(%this.upKey, 1), "moveUp", %this);
	controlActionMap.bindObj(getWord(%this.downKey, 0), getWord(%this.downKey, 1), "moveDown", %this);
	controlActionMap.bindObj(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1), "moveLeft", %this);
	controlActionMap.bindObj(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1), "moveRight", %this);
	
	controlActionMap.bindObj("keyboard", %this.blockKey, "pressBlock", %this);
	controlActionMap.bindObj("keyboard", %this.dashKey, "pressDash", %this);

	//Xbox XInput mapping
	controlActionMap.bindObj("gamepad0", "thumblx", "LAnalogX", %this);
	controlActionMap.bindObj("gamepad0", "thumbly", "LAnalogY", %this);	
	
	controlActionMap.bindObj("gamepad0", "triggerl", "pressBlock", %this);
	controlActionMap.bindObj("gamepad0", "btn_l", "pressDash", %this);

	%this.up = 0;
	%this.down = 0;
	%this.left = 0;
	%this.right = 0;
	
	%this.moveAngle = 0;
	
	%this.setUpdateCallback(true);
	
	%this.wallCheckDist = 2;
	
	controlActionMap.push();
	
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

	
	
	//TODO::Are the Unbindings needed?

	controlActionMap.unbindObj(getWord(%this.upKey, 0), getWord(%this.upKey, 1), %this);
	controlActionMap.unbindObj(getWord(%this.downKey, 0), getWord(%this.downKey, 1), %this);
	controlActionMap.unbindObj(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1), %this);
	controlActionMap.unbindObj(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1), %this);
	
	controlActionMap.unbindObj("keyboard", %this.blockKey, %this);

	%this.up = 0;
	%this.down = 0;
	%this.left = 0;
	%this.right = 0;
	
	//controlActionMap.delete();
}

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::onCollision(%this, %object, %collisionDetails)
{

}

//-----------------------------------------------------------------------------

function PlayerMovementControlsBehavior::onUpdate(%this)
{
	%this.updateMovement();
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
			
		/*if(%this.upRight == 1)
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
		}*/
		
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
		%this.down = 0;
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
		%this.up = 0;
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
		%this.right = 0;
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
		%this.left = 0;
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
  
/*function PlayerMovementControlsBehavior::moveUpRight(%this, %val)
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
}*/

//------------------------------------------------------------------------------------

function PlayerMovementControlsBehavior::pressBlock(%this, %val)
{
	if(%val == 1)
	{
		%this.owner.blocker = new CompositeSprite()
		{
			class = "PlayerBlock";
			//blockAngle = %this.owner.getAngle();
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

function PlayerMovementControlsBehavior::pressDash(%this, %val)
{
	if(%val == 1)
	{
		if((! %this.owner.isDashing) && (! %this.owner.tarred))
		{
			%this.findDashAngle();
			
			%newDashTrail = new CompositeSprite()
			{
				class = "PlayerDash";
				dashAngle = %this.moveAngle;
			};
				
			%this.owner.getScene().add( %newDashTrail );
			%newDashTrail.setPosition(%this.owner.getWorldPoint(-20 * $pixelsToWorldUnits, 0) );
			
			
			%this.owner.isDashing = true;
			%this.owner.currDashDirection = %this.moveAngle;
			%this.owner.setLinearVelocityPolar(%this.moveAngle, %this.owner.dashSpeed);	
			
			%this.dashSchedule = schedule(%this.owner.dashLength, 0, "PlayerAimControlsBehavior::endDash", %this);
			
			%this.owner.dashCount++;
		
			%this.owner.setSpriteBlendAlpha(0.8);
		}
	}
} 

function PlayerMovementControlsBehavior::findDashAngle(%this)
{
	
	if(%this.up && %this.left)
	{
		%this.moveAngle = 135;
	}
	if(%this.up && %this.right)
	{
		%this.moveAngle = 45;
	}
	if(%this.down && %this.left)
	{
		%this.moveAngle = 225;
	}
	if(%this.down && %this.right)
	{
		%this.moveAngle = 315;
	}
	if(%this.up && !%this.left && !%this.right)
	{
		%this.moveAngle = 90;
	}
	if(%this.down && !%this.left && !%this.right)
	{
		%this.moveAngle = 270;
	}
	if(%this.left && !%this.up && !%this.down)
	{
		%this.moveAngle = 180;
	}
	if(%this.right && !%this.up && !%this.down)
	{
		%this.moveAngle = 0;
	}
	
}

function PlayerMovementControlsBehavior::endDash(%this)
{
	%this.owner.isDashing = false;	
	%this.owner.setLinearVelocityPolar(0, 0);	
	%this.owner.setSpriteBlendAlpha(1);
}

//------------------------------------------------------------------------------------

/*function PlayerMovementControlsBehavior::doNothing(%this)
{
	//Empty function for timer calls
}*/
