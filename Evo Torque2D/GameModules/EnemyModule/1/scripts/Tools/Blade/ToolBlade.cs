//-----------------------------------------------------------------------------
// PlayerModule: EnemyUnit class and functions
//-----------------------------------------------------------------------------

function ToolBlade::CreateInstance(%emyOwner, %type, %posX, %posY, %toolOrientation)  
{  
    %r = new SceneObject()  
	{  
		superclass = "ToolNode";
		class = "ToolBlade";   
		owner = %emyOwner;
		toolType = %type;
		bodyPosX = %posX;
		bodyPosY = %posY;
		orientation = %toolOrientation;
		stackLevel = 1;
		cooldownTime = 2.6*1000;
	};  
  
    return %r;  
}  

//-----------------------------------------------------------------------------

function ToolBlade::initialize(%this)
{	
	exec("./EnemyBladeStrike.cs");
	
	Parent::initialize(%this);
	
	%this.cooldownTime = %this.cooldownTime/%this.stackLevel;
	%this.bladeDamage = 15;
	
	//shot barrel offset (instead of bullet coming out of center of cannon)	
	%this.bladeXoffset = %this.myWidth*0.75;
	%this.bladeYoffset = 0;	
}

//-----------------------------------------------------------------------------

function ToolBlade::setupSprite( %this )
{
	%this.owner.addSprite(%this.getRelativePosistion());
	
	%this.owner.setSpriteImage("GameAssets:tool_blade_a", 0);
	%this.owner.setSpriteSize(64 * %this.owner.sizeRatio, 64 * %this.owner.sizeRatio);
	%this.sortLevel = 6;
	
	%this.owner.setSpriteAngle(%this.orientation);
}

//-----------------------------------------------------------------------------

function ToolBlade::setupBehaviors( %this )
{
	exec("./behaviors/bladeAI.cs");
	%baseAI = BladeToolBehavior.createInstance();
	%this.addBehavior(%baseAI);
}

//-----------------------------------------------------------------------------

function ToolBlade::attack( %this )
{
	%this.owner.bladeAttackNums++;
	
	// add a strike to the arena
	%newStrike = new CompositeSprite()
	{
		class = "EnemyBladeStrikeEffect";
		strikeDamage = %this.bladeDamage;
		strikeAngle = %this.owner.getAngle();
		owner = %this;
	};
	
	%this.owner.getMyScene().add( %newStrike );
	
	%newStrike.setPosition(%this.getWorldPosistion(%this.getOrientatedOffset(%this.bladeXoffset, %this.bladeYoffset)));
} 