//-----------------------------------------------------------------------------
// Basic player aim conrols: joystick corresponds to 8 keys, for each cardinal direction
//-----------------------------------------------------------------------------

if (!isObject(PlayerAimControlsBehavior))
{
    %template = new BehaviorTemplate(PlayerAimControlsBehavior);

	%template.friendlyName = "Shooter Aiming";
	%template.behaviorType = "Input";
	%template.description  = "Shooter style aiming control";

	%template.addBehaviorField(upFaceKey, "", keybind, "keyboard up");
	%template.addBehaviorField(downFaceKey, "", keybind, "keyboard down");
	%template.addBehaviorField(leftFaceKey, "", keybind, "keyboard left");
	%template.addBehaviorField(rightFaceKey, "", keybind, "keyboard right");
  
	/*%template.addBehaviorField(ulKey, "", keybind, "keyboard I");
	%template.addBehaviorField(urKey, "", keybind, "keyboard O");
	%template.addBehaviorField(drKey, "", keybind, "keyboard Comma");
	//%template.addBehaviorField(dlKey, "", keybind, "keyboard M");*/

  	//%template.addBehaviorField(fireKey, "", keybind, "keyboard space");
	%template.addBehaviorField(switchKey, "", keybind, "keyboard E");
	//%template.addBehaviorField(dashKey, "", keybind, "keyboard space");
}

function PlayerAimControlsBehavior::onBehaviorAdd(%this)
{
    if (!isObject(aimControlActionMap))
       return;
	   
	//cardinal
    aimControlActionMap.bindObj(getWord(%this.upFaceKey, 0), getWord(%this.upFaceKey, 1), "faceUp", %this);
    aimControlActionMap.bindObj(getWord(%this.rightFaceKey, 0), getWord(%this.rightFaceKey, 1), "faceRight", %this);
    aimControlActionMap.bindObj(getWord(%this.downFaceKey, 0), getWord(%this.downFaceKey, 1), "faceDown", %this);
    aimControlActionMap.bindObj(getWord(%this.leftFaceKey, 0), getWord(%this.leftFaceKey, 1), "faceLeft", %this);
	
	//aimControlActionMap.bindObj(getWord(%this.fireKey, 0), getWord(%this.fireKey, 1), "pressFire", %this);
	aimControlActionMap.bindObj("keyboard", %this.switchKey, "switchAttackMode", %this);
	//aimControlActionMap.bindObj("keyboard", %this.dashKey, "pressDash", %this);
	
	/*//diagonal
    GlobalActionMap.bindObj(getWord(%this.ulKey, 0), getWord(%this.ulKey, 1), "faceUL", %this);
    GlobalActionMap.bindObj(getWord(%this.urKey, 0), getWord(%this.urKey, 1), "faceUR", %this);
    GlobalActionMap.bindObj(getWord(%this.drKey, 0), getWord(%this.drKey, 1), "faceDR", %this);
    //GlobalActionMap.bindObj(getWord(%this.dlKey, 0), getWord(%this.dlKey, 1), "faceDL", %this);*/
	
	controlActionMap.bindObj("gamepad0", "connect", "isConnected", %this);	
	
	//aimControlActionMap.bindObj("gamepad0", "triggerr", "pressFire", %this);
	controlActionMap.bindObj("gamepad0", "thumbrx", "RAnalogX", %this);
	controlActionMap.bindObj("gamepad0", "thumbry", "RAnalogY", %this);	
	
	aimControlActionMap.bindObj("gamepad0", "btn_r", "switchAttackMode", %this);
	aimControlActionMap.bindObj("gamepad0", "btn_l", "pressDash", %this);
	
	%this.upFace = 0;
	%this.downFace = 0;
	%this.leftFace = 0;
	%this.rightFace = 0;
	
	%this.xFace = 0;
	%this.yFace = 0;
	
	%this.rotation = 90;
	
	//shot barrel offset (instead of bullet coming out of center of player)	
	%this.barrelXoffset = 60 * $pixelsToWorldUnits;//*%this.owner.myWidth;
	%this.barrelYoffset = -40 * $pixelsToWorldUnits;//*%this.owner.myHeight;
	
	//blade offset (instead of strike effect coming out of center of player)	
	%this.bladeXoffset = 75 * $pixelsToWorldUnits;
	%this.bladeYoffset = 30 * $pixelsToWorldUnits;
	
	%this.attackMode = "shoot";
	
	%this.fireHeld = false;
	%this.strikeHeld = false;
	%this.strikeCooledDown = true;
	%this.fireCooledDown = true;
	
	aimControlActionMap.push();
}

function PlayerAimControlsBehavior::onBehaviorRemove(%this)
{
    if (!isObject(aimControlActionMap))
       return;

    aimControlActionMap.unbindObj("keyboard", %this.upFaceKey, %this);
    aimControlActionMap.unbindObj("keyboard", %this.righFacetKey, %this);
    aimControlActionMap.unbindObj("keyboard", %this.downFaceKey, %this);
    aimControlActionMap.unbindObj("keyboard", %this.leftFaceKey, %this);

    /*GlobalActionMap.unbindObj("keyboard", %this.ulKey, %this);
    GlobalActionMap.unbindObj("keyboard", %this.urKey, %this);
    GlobalActionMap.unbindObj("keyboard", %this.drKey, %this);
    //GlobalActionMap.unbindObj("keyboard", %this.dlKey, %this);*/
	
	//aimControlActionMap.unbindObj("keyboard", %this.fireKey, %this);
	aimControlActionMap.unbindObj("keyboard", %this.switchKey, %this);
	aimControlActionMap.unbindObj("keyboard", %this.dashKey, %this);
	
	%this.upFace = 0;
	%this.downFace = 0;
	%this.leftFace = 0;
	%this.rightFace = 0;
	
	%this.xFace = 0;
	%this.yFace = 0;
	
	%this.rotation = 0;
	
	//aimControlActionMap.delete();
}

//------------------------------------------------------------------------------------
  
function PlayerAimControlsBehavior::onUpdate(%this)
{
	//echo(" x , y    " @ %this.owner.getPosition());
	%this.updateFacingDirection();
	
	if(%this.upFace || %this.leftFace || %this.downFace || %this.rightFace || mAbs(%this.xFace) + mAbs(%this.yFace) > 0)
		%this.tryAttack();
	
	/*if(%this.fireHeld)
	{
		%this.tryFire();
	}
	
	if(%this.strikeHeld)
	{
		%this.tryMelee();
	}*/
}

//------------------------------------------------------------------------------------
  
/*function PlayerAimControlsBehavior::updateFacingDirection(%this)
{
	%this.rotation = %this.getRotation();
	echo("Rotation: " @ %this.rotation);
	%this.owner.setAngle(%this.rotation);
}*/


function PlayerAimControlsBehavior::updateFacingDirection(%this)
{
	//%tempUp = %this.upFace;
	//%tempDown = %this.downFace;
	//%tempLeft = %this.leftFace;
	//%tempRight = %this.rightFace;	
	
	/*echo("up, down, left, right");
	echo(%this.upFace);
	echo(%this.downFace);
	echo(%this.leftFace);
	echo(%this.rightFace);
	echo("sticks: " @ mATan(%this.xFace, %this.yFace));*/
	
	if(%this.upFace && %this.leftFace)
	{
		%this.rotation = 135;
	}
	if(%this.upFace && %this.rightFace)
	{
		%this.rotation = 45;
	}
	if(%this.downFace && %this.leftFace)
	{
		%this.rotation = 225;
	}
	if(%this.downFace && %this.rightFace)
	{
		%this.rotation = 315;
	}
	if(%this.upFace && !%this.leftFace && !%this.rightFace)
	{
		%this.rotation = 90;
	}
	if(%this.downFace && !%this.leftFace && !%this.rightFace)
	{
		%this.rotation = 270;
	}
	if(%this.leftFace && !%this.upFace && !%this.downFace)
	{
		%this.rotation = 180;
	}
	if(%this.rightFace && !%this.upFace && !%this.downFace)
	{
		%this.rotation = 0;
	}
	if(mAbs(%this.xFace) + mAbs(%this.yFace) > 0)
	{
		%this.rotation = mATan(%this.xFace, %this.yFace);
		if(%this.rotation < 0)
			%this.rotation =  360 + %this.rotation;
	}
	
	/*if(%this.upFace)
	{
		if(%this.rightFace)
		{
			echo("upright");
			%this.rotation =  45;
		}
		else
		{
			if(%this.leftFace)
			{
					echo("upleft");
					%this.rotation =  135;
			}
			else
			{
				echo("up");
				%this.rotation =  90;
			}
		}
	}
	else
	{
		if(%this.downFace)	
		{
			if(%this.rightFace)
			{
				echo("downright");
				%this.rotation =  315;
			}
			else 
			{
				if(%this.leftFace)
				{
					echo("downleft");
					%this.rotation =  225;
				}
				else
				{
					echo("down");
					%this.rotation =  270;
				}
			}
		}
		else
		{
			if(%this.rightFace)
			{
				echo("right");
				%this.rotation =  0;
			}
			else 
			{
				if(%this.leftFace)
				{
					echo("left");
					%this.rotation =  180;
				}
			}
		}
	}*/
	
	//echo("Rotation: " @ %this.rotation);
	%this.owner.setAngle(%this.rotation);
}

function PlayerAimControlsBehavior::isConnected(%this, %val)
{
	echo("CONNECTED???  " @ %val);
}

function PlayerAimControlsBehavior::faceUp(%this, %val)
{	
	//%targetRotation = 90;
	
	if(%val == 1)
	{
		%this.upFace = 1;
		%this.downFace = 0;
		//%this.fireHeld = 1;
	}
	else
	{
		%this.upFace = 0;
		//%this.fireHeld = 0;
	}
}

function PlayerAimControlsBehavior::faceDown(%this, %val)
{	
	//%targetRotation = 270;
	
    if(%val == 1)
	{
		%this.downFace = 1;
		%this.upFace = 0;
		//%this.fireHeld = 1;
	}
	else
	{
		%this.downFace = 0;
		//%this.fireHeld = 0;
	}
}

function PlayerAimControlsBehavior::RAnalogY(%this, %val)
{
	
	%this.yFace = %val;
}

function PlayerAimControlsBehavior::faceLeft(%this, %val)
{
	//%targetRotation = 180;
	
    if(%val == 1)
	{
		%this.leftFace = 1;
		%this.rightFace = 0;
		//%this.fireHeld = 1;
	}
	else
	{
		%this.leftFace = 0;
		//%this.fireHeld = 0;
	}
}

function PlayerAimControlsBehavior::faceRight(%this, %val)
{
	//%targetRotation = 0;
	
    if(%val == 1)
	{
		%this.rightFace = 1;
		%this.leftFace = 0;
		//%this.fireHeld = 1;
	}
	else
	{
		%this.rightFace = 0;
		//%this.fireHeld = 0;
	}
}

function PlayerAimControlsBehavior::RAnalogX(%this, %val)
{
	%this.xFace = %val;
}

/*//------------------------------------------------------------------------------------
function PlayerAimControlsBehavior::faceUR(%this, %val)
{	
	%targetRotation = 45;
	
	if(%val == 1)
		%this.updateFacingDirection(%targetRotation);
}

function PlayerAimControlsBehavior::faceDR(%this, %val)
{	
	%targetRotation = 315;
	
    if(%val == 1)
		%this.updateFacingDirection(%targetRotation);
}

function PlayerAimControlsBehavior::faceDL(%this, %val)
{
	%targetRotation = 225;
	
    if(%val == 1)
		%this.updateFacingDirection(%targetRotation);
}

function PlayerAimControlsBehavior::faceUL(%this, %val)
{
	%targetRotation = 135;
	
    if(%val == 1)
		%this.updateFacingDirection(%targetRotation);
}*/

function PlayerAimControlsBehavior::switchAttackMode(%this, %val)
{
	echo("inside of press fire with val: " @ %val);
	if(%val == 1)
	{
		//if (!isEventPending(%this.modeChangeSchedule))  %this.modeChangeSchedule = %this.schedule(5000, "switchAttackMode", 1);
		if(%this.attackMode $= "shoot")
		{
			%this.attackMode = "strike";
		}
		else
		{
			if(%this.attackMode $= "strike")
			{
				%this.attackMode = "shoot";
			}
		}
	}
} 

//------------------------------------------------------------------------------------

function PlayerAimControlsBehavior::tryAttack(%this)
{
	if(%this.attackMode $= "shoot")
	{
		%this.tryFire();
	}
	else
	{
		if(%this.attackMode $= "strike")
		{
			%this.tryMelee();
		}
	}
} 

function PlayerAimControlsBehavior::tryFire(%this)
{
	//echo("inside of try fire");
	//if(getEventTimeLeft(%this.fireCooldownTime) <= 0)
	if(%this.fireCooledDown)
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
		%newFlash.setPosition(%this.owner.getWorldPoint(%this.barrelXoffset, %this.barrelYoffset) );
		
		%this.fireCooledDown = false;
		schedule(%this.owner.fireCooldown, 0, "PlayerAimControlsBehavior::fireCooldownReached", %this);
		//%this.fireCooldownTime = schedule(%this.owner.fireCooldown, 0, "PlayerAimControlsBehavior::doNothing", %this);
	}
}

//------------------------------------------------------------------------------------

function PlayerAimControlsBehavior::fireCooldownReached(%this)
{
	%this.fireCooledDown = true;
}

//------------------------------------------------------------------------------------

function PlayerAimControlsBehavior::pressMelee(%this, %val)
{
	//echo("inside of press melee with val: " @ %val);
	if(%val == 1)
	{
		%this.strikeHeld = true;
	}
	else
	{
		%this.strikeHeld = false;
	}
} 

function PlayerAimControlsBehavior::tryMelee(%this)
{
	//echo("inside of try melee");
	//if(getEventTimeLeft(%this.strikeCooldownTime) <= 0)
	if(%this.strikeCooledDown)
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
		%this.owner.setSpriteSize(320 * $pixelsToWorldUnits, 164 * $pixelsToWorldUnits);
		%this.owner.setSpriteLocalPosition(0, 17 * $pixelsToWorldUnits);
		%this.owner.schedule(250, "setupSprite");
		
		%this.strikeCooledDown = false;
		schedule(%this.owner.strikeCooldown, 0, "PlayerAimControlsBehavior::strikeCooldownReached", %this);
		//%this.strikeCooldownTime = schedule(%this.owner.strikeCooldown, 0, "PlayerAimControlsBehavior::doNothing", %this);
	}
}

//------------------------------------------------------------------------------------

function PlayerAimControlsBehavior::strikeCooldownReached(%this)
{
	%this.strikeCooledDown = true;
}

//------------------------------------------------------------------------------------

function PlayerAimControlsBehavior::pressDash(%this, %val)
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
			%newDashTrail.setPosition(%this.owner.getWorldPoint(-20 * $pixelsToWorldUnits, 0) );
			
			
			%this.owner.isDashing = true;
			%this.owner.currDashDirection = %this.owner.getAngle();
			%this.owner.setLinearVelocityPolar(%this.owner.getAngle(), %this.owner.dashSpeed);	
			
			%this.dashSchedule = schedule(%this.owner.dashLength, 0, "PlayerAimControlsBehavior::endDash", %this);
			
			%this.owner.dashCount++;
		
			%this.owner.setSpriteBlendAlpha(0.8);
		}
	}
} 

function PlayerAimControlsBehavior::endDash(%this)
{
	%this.owner.isDashing = false;	
	%this.owner.setLinearVelocityPolar(0, 0);	
	%this.owner.setSpriteBlendAlpha(1);
}

//------------------------------------------------------------------------------------