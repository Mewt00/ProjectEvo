//-----------------------------------------------------------------------------
// Shooter Tool
//-----------------------------------------------------------------------------

function ToolShooter::CreateInstance(%emyOwner, %type, %posX, %posY, %toolOrientation)  
{  
    %r = new SceneObject()  
	{  
		superclass = "ToolNode";
		class = "ToolShooter";   
		owner = %emyOwner;
		toolType = %type;
		bodyPosX = %posX;
		bodyPosY = %posY;
		orientation = %toolOrientation;
		stackLevel = 1;
		reloadTime = 2.0*1000;
	};  
  
    return %r;  
}  

//-----------------------------------------------------------------------------

function ToolShooter::initialize(%this)
{	
	exec("./EnemyShooterBullet.cs");
	exec("./ToolShooterTurret.cs");
	
	Parent::initialize(%this);
	
	%this.reloadTime = %this.reloadTime/%this.stackLevel;
	%this.shooterDamage = 15;
	%this.turnSpeed = 120;
	
	//shot barrel offset (instead of bullet coming out of center of cannon)	
	%this.barrelXoffset = 55*%this.owner.sizeRatio;
	%this.barrelYoffset = 0*%this.owner.sizeRatio;	
	
	%this.addTurret();
	
    %this.setUpdateCallback(true);
	
	%this.owner.addEnemyBehavior("StrafeBehavior", 1);
	%this.owner.addEnemyBehavior("MinDistanceBehavior", 1);
	/*for(%i = 0; %i < %this.owner.behaviorNamesSize; %i += 2)		
	{
		echo("%this.behaviorNames   " @ %this.owner.behaviorNames[%i] SPC %this.owner.behaviorNames[%i + 1]);
	}
	echo("Size: " SPC %this.owner.behaviorNamesSize);*/
}

//-----------------------------------------------------------------------------

function ToolShooter::setupCollisionShape( %this )
{
	%offsetX = %this.myWidth/2;
	%offsetY = %this.myHeight/2;
	
	%pAx = %this.bodyPosX*%this.myWidth - %offsetX;
	%pAy = %this.bodyPosY*%this.myHeight - %offsetY;
	
	%pBx = %this.bodyPosX*%this.myWidth + %offsetX;
	%pBy = %this.bodyPosY*%this.myHeight - %offsetY;
	
	%pCx = %this.bodyPosX*%this.myWidth + %offsetX;
	%pCy = %this.bodyPosY*%this.myHeight + %offsetY;
	
	%pDx = %this.bodyPosX*%this.myWidth - %offsetX;
	%pDy = %this.bodyPosY*%this.myHeight + %offsetY;
	
	
	%shapePoints = 
		%pAx SPC %pAy SPC 
		%pBx SPC %pBy SPC 
		%pCx SPC %pCy SPC 
		%pDx SPC %pDy;	
		
	%this.owner.createPolygonCollisionShape(%shapePoints);
}

//-----------------------------------------------------------------------------

function ToolShooter::addTurret( %this )
{
	// add a turret to the arena
	%this.myTurret = new CompositeSprite()
	{
		class = "ToolShooterTurret";
		owner = %this;
		turnSpeed = %this.turnSpeed;
	};
	
	%this.myTurret.setPosition(%this.getWorldPosistion());
	%this.owner.getMyScene().add( %this.myTurret );
	
}

//-----------------------------------------------------------------------------

function ToolShooter::setupSprite( %this )
{
	%this.owner.addSprite(%this.getRelativePosistion());
	
	%this.owner.setSpriteImage("GameAssets:tool_shooter_a", 0);
	%this.owner.setSpriteSize(64 * %this.owner.sizeRatio, 64 * %this.owner.sizeRatio);
	%this.sortLevel = 6;
	
	%this.owner.setSpriteAngle(%this.orientation);
}

//-----------------------------------------------------------------------------

function ToolShooter::setupBehaviors( %this )
{
	exec("./behaviors/shooterAI.cs");
	%baseAI = ShooterToolBehavior.createInstance();
	%this.addBehavior(%baseAI);
}

//-----------------------------------------------------------------------------

function ToolShooter::onUpdate( %this )
{
}

//-----------------------------------------------------------------------------

function ToolShooter::shoot( %this )
{
	%this.owner.shooterShotsFired++;

	// add a bullet to the arena
	%newBullet = new CompositeSprite()
	{
		class = "EnemyShooterBullet";
		fireAngle = %this.myTurret.getAngle();
		shotDamage = %this.shooterDamage;
		owner = %this;
	};
	
	%this.owner.getMyScene().add( %newBullet );
	
	%newBullet.setPosition(%this.getWorldPosistion());
		
	//%this.mySchedule = schedule(%this.reloadTime, 0, "ToolShooter::shoot", %this);
	
} 
//-----------------------------------------------------------------------------

function ToolShooter::onRemove( %this )
{
	if(isObject(%this.myTurret))
	{
		%this.myTurret.safeDelete();
	}
}