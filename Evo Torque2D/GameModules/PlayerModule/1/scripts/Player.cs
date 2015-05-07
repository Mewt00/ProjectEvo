//-----------------------------------------------------------------------------
// PlayerModule: player class and functions
//-----------------------------------------------------------------------------

function Player::onAdd( %this )
{

}

//-----------------------------------------------------------------------------

function Player::initialize(%this)
{
	exec("./playerBullet/PlayerBullet.cs");
	exec("./playerBullet/PlayerBulletMuzzleFlash.cs");
	exec("./playerBullet/PlayerBulletHit.cs");
	exec("./playerStrike/PlayerStrike.cs");
	exec("./playerDash/PlayerDash.cs");
	exec("./playerBlock/PlayerBlock.cs");
	
	%this.setSceneGroup(Utility.getCollisionGroup("Player"));			//5: Player sceneGroup
	%this.setSceneLayer(5);
	%this.fixedAngle = true;
	
	//Stats
	%this.fullHealth = 150;
	%this.health = %this.fullHealth;
	%this.sizeRatio = $pixelToWorldRatio;
	%this.myWidth = 210 * %this.sizeRatio;
	%this.myHeight = 169 * %this.sizeRatio;
	
	%this.baseWalkSpeed = 40;
	%this.walkSpeed = %this.baseWalkSpeed;
	%this.tarred = false;
	%this.setPosition(0, 25);
	
	%this.myShotsContainer = new SimSet();		//tracker for shot objects on screen
	
	
	%this.base_fireRate = 3;
	%this.fireRate = %this.base_fireRate;					//per second
	%this.fireCooldown = 1000/%this.fireRate;				//ms
	
	%this.strikeRate = 2;									//per second
	%this.strikeCooldown = 1000/%this.strikeRate;			//
	
	%this.blockTickRate = 2;								//per second
	%this.blockTickTime = 1000/%this.strikeRate;			//ms
	
	//Dash
	%this.isDashing = false;
	%this.currDashDirection = 0;
	%this.dashLength = 0.3*1000;		//ms
	%this.dashSpeed = 120;
	
	//Block
	%this.blockCooldown = 0;
	%this.blockCooldownRate = 0.25;
	%this.blockCooldownRefresh = 20;
	
	//Algorithm Counters
	%this.rangedCount = 0;
	%this.meleeCount = 0;
	%this.blockCount = 0;
	%this.dashCount = 0;
	
    %this.setUpdateCallback(true);
	
	%this.setupSprite();
	%this.setupControls();
	
	%this.setupCollisionShape();
	
	%this.myHealthbar = new CompositeSprite()
	{
		class = "Healthbar";
		owner = %this;
		xOffset = 0;
		yOffset = 150*%this.sizeRatio;
		curved = true;
	};
	
	%this.getScene().add( %this.myHealthbar );
	
	echo("Player.initialize(): id: " SPC %this.getId());
}

//-----------------------------------------------------------------------------

function Player::addMyHealthbar( %this )
{
}

//-----------------------------------------------------------------------------

function Player::setupCollisionShape( %this )
{
	%offsetX = %this.myWidth*(5/12);
	%offsetY = %this.myHeight*(5/12);
	
	%boxSizeRatio = 0.75;
	%shapePoints = 
		0 SPC %offsetY*%boxSizeRatio SPC 
		%offsetX*%boxSizeRatio - %this.myWidth*(1/10) SPC 0 SPC 
		0 SPC - %offsetY*%boxSizeRatio SPC 
		-%offsetX*%boxSizeRatio SPC 0;	
	
	%this.createPolygonCollisionShape(%shapePoints);
	
    %this.setCollisionGroups( Utility.getCollisionGroup("Enemies") SPC Utility.getCollisionGroup("Wall") );

	%this.setCollisionCallback(true);
}
//-----------------------------------------------------------------------------

function Player::setupSprite( %this )
{
	%this.clearSprites();
	%this.addSprite("0 0");
	%this.setSpriteAnimation("GameAssets:playerbaseAnim", 0);
	%this.setSpriteName("BodyAnim");
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
}

//-----------------------------------------------------------------------------

function Player::setupControls( %this )
{
	exec("./behaviors/controls/PlayerMovementControls.cs");
	exec("./behaviors/controls/PlayerAimControls.cs");
	//exec("./behaviors/controls/alignToJoystick.cs");
	
	echo("Player controls");
	new ActionMap(controlActionMap);
 	%controls = PlayerMovementControlsBehavior.createInstance();
	%this.addBehavior(%controls);
	
 	%aimer = PlayerAimControlsBehavior.createInstance();
	%this.addBehavior(%aimer);
	
	/*
 	%xBoxControl = AlignToJoystickBehavior.createInstance();
	%xBoxControl.xAxis = "joystick0 xaxis";
	%xBoxControl.yAxis = "joystick0 yaxis";
	%xBoxControl.rotationOffset = 90;
	%this.addBehavior(%xBoxControl); 
	*/
	
	/*
	exec("./behaviors/controls/faceMouse.cs");
	
	%faceMse = FaceMouseBehavior.createInstance();
	%faceMse.object = mainPlayer;
	%faceMse.rotationOffset = -90;
	%this.addBehavior(%faceMse);
	*/
	/*
	exec("./behaviors/controls/mouseInput.cs");
	
	%mouseInput = PlayerMouseInputBehavior.createInstance();
	%this.addBehavior(%mouseInput);
	*/
}

//-----------------------------------------------------------------------------

function Player::onUpdate( %this )
{
	if(%this.blockCooldown > 0)
	{
		%this.blockCooldown = %this.blockCooldown - %this.blockCooldownRate;
	}
	
	if(%this.tarred)
	{
		%slowedRatio = %this.walkSpeed / %this.baseWalkSpeed;
		%this.setSpriteBlendColor(%slowedRatio, %slowedRatio, %slowedRatio);
	}
}

//-----------------------------------------------------------------------------
// receive damage

function Player::hit(%this, %damage, %dmgObject)
{
	if(isObject(%this.blocker))			//if shield is up
	{
		%this.blocker.takeDamage(%damage);	//shield absorbs damage
		//TODO: currently no damage carry over, 1 hp of shield could block 80 incoming damage
		return 0;
	}
	else
	{
		return %this.takeDamage(%damage, %dmgObject);
	}
}
	
//-----------------------------------------------------------------------------
//subtract health, check for death and kill

function Player::takeDamage( %this, %dmgAmount, %dmgObject )
{
	if(%this.isDashing)
	{
		return 0;
	}

	%this.health -= %dmgAmount;
	%this.splashScreenDamage(%dmgAmount);
	
	if( %this.health <= 0)
	{
		%this.kill(%dmgObject);
	}
	else if( %this.health > %this.fullHealth)
	{
		%this.health = %this.fullHealth;
	}
	

	%this.myHealthbar.assessDamage();
	return %dmgAmount;
}

//-----------------------------------------------------------------------------

function Player::splashScreenDamage( %this, %dmgAmount )
{
	if(%dmgAmount >= 0)
	{
		%roomDmgSplash = new Sprite() { 
			class = "DamageSplashScreen";
			initDamage = %dmgAmount;
		};
		%this.getScene().add( %roomDmgSplash ); 
	}
}

//-----------------------------------------------------------------------------

function Player::tar( %this, %slowEffect, %duration )
{
	if(!tarred)
	{
		//%this.addSprite(1, 0);
		//%this.setSpriteImage("GameAssets:playertarred", 0);
		//%this.setSpriteSize(184 * %this.sizeRatio, 127 * %this.sizeRatio);
		
		%this.tarred = true;
	}
	
	%this.walkSpeed -= %slowEffect;
	
	
	if(%this.walkSpeed < %this.baseWalkSpeed/3)
	{
		%this.walkSpeed = %this.baseWalkSpeed/3;
	}
	
	//schedule(%duration, 0, "Player::restoreSpeed", %this, %slowEffect);
	%this.schedule(%duration, "restoreSpeed", %slowEffect);
}

//-----------------------------------------------------------------------------

function Player::restoreSpeed( %this, %amt )
{
	%this.walkSpeed += %amt;
	
	if(%this.walkSpeed >= %this.baseWalkSpeed )
	{
		%this.tarred = false;
		%this.walkSpeed = %this.baseWalkSpeed;
	}
}

//-----------------------------------------------------------------------------

function Player::updateFireRate( %this, %rate )
{
	%this.fireRate = %rate;
	%this.fireCooldown = 1000/%this.fireRate;			//ms
}


//-----------------------------------------------------------------------------
// Player has been defeated, end level trigger

function Player::kill( %this, %murderer )
{
	%this.myArena.playerDied(%murderer);
	%this.safeDelete();
}

//-----------------------------------------------------------------------------

function Player::onRemove( %this )
{
	echo("before:");
	echo(isObject(controlActionMap));
	controlActionMap.delete();
	echo("after delete?");
	echo(isObject(controlActionMap));
	%this.clearBehaviors();
	//%this.safeDelete();
}
