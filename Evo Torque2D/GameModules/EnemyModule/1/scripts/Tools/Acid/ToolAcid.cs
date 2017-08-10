//-----------------------------------------------------------------------------
// PlayerModule: EnemyUnit class and functions
//-----------------------------------------------------------------------------

function ToolAcid::CreateInstance(%emyOwner, %type, %posX, %posY, %toolOrientation)  
{  
    %r = new SceneObject()  
	{  
		superclass = "ToolNode";
		class = "ToolAcid";   
		owner = %emyOwner;
		toolType = %type;
		bodyPosX = %posX;
		bodyPosY = %posY;
		orientation = %toolOrientation;
		stackLevel = 1;
		reloadTime = 2.5*1000;
		acidDamage = 5;
		acidDamageToShield = 20;			//bonus damage to Player shield
	};  
  
    return %r;  
}  

//-----------------------------------------------------------------------------

function ToolAcid::initialize(%this)
{	
	exec("./EnemyAcidSpray.cs");
	exec("./ToolAcidTurret.cs");
	
	Parent::initialize(%this);
	
	%this.reloadTime = %this.reloadTime/%this.stackLevel;
	%this.turnSpeed = 120;
	
	//shot barrel offset (instead of spray coming out of center of cannon)	
	%this.barrelXoffset = 126*%this.owner.sizeRatio;
	%this.barrelYoffset = 0*%this.owner.sizeRatio;	
	
	%this.addTurret();
	
	%this.owner.addEnemyBehavior("ChaseBehavior", 1);
	/*for(%i = 0; %i < %this.owner.behaviorNamesSize; %i += 2)		
	{
		echo("%this.behaviorNames   " @ %this.owner.behaviorNames[%i] SPC %this.owner.behaviorNames[%i + 1]);
	}
	echo("Size: " SPC %this.owner.behaviorNamesSize);*/
}

//-----------------------------------------------------------------------------

function ToolAcid::addTurret( %this )
{
	// add a turret to the arena
	%this.myTurret = new CompositeSprite()
	{
		class = "ToolAcidTurret";
		owner = %this;
		turnSpeed = %this.turnSpeed;
	};
	
	%this.owner.getMyScene().add( %this.myTurret );
	
	%this.myTurret.setPosition(%this.getWorldPosistion());
}

//-----------------------------------------------------------------------------

function ToolAcid::setupSprite( %this )
{
	%this.owner.addSprite(%this.getRelativePosistion());
	
	%this.owner.setSpriteImage("GameAssets:tool_acid_a", 0);
	%this.owner.setSpriteSize(64 * %this.owner.sizeRatio, 64 * %this.owner.sizeRatio);
	%this.sortLevel = 6;
	
	%this.owner.setSpriteAngle(%this.orientation);
}

//-----------------------------------------------------------------------------

function ToolAcid::setupBehaviors( %this )
{
	exec("./behaviors/acidAI.cs");
	%baseAI = AcidToolBehavior.createInstance();
	%this.addBehavior(%baseAI);
}

//-----------------------------------------------------------------------------

function ToolAcid::shoot( %this )
{
	// add a spray to the arena
	%newSpray = new CompositeSprite()
	{
		class = "EnemyAcidSpray";
		fireAngle = %this.owner.getAngle();
		sprayDamage = %this.acidDamage;
		sprayDamageToShield = %this.acidDamageToShield;
		owner = %this;
	};
	
	%this.owner.getMyScene().add( %newSpray );
	
	%newSpray.setPosition(%this.getWorldPosistion());	//%this.myWidth, 0
		
	//%this.mySchedule = schedule(%this.reloadTime, 0, "ToolShooter::shoot", %this);
	
} 

//-----------------------------------------------------------------------------

function ToolAcid::onRemove( %this )
{
	if(isObject(%this.myTurret))
	{
		%this.myTurret.safeDelete();
	}
}
