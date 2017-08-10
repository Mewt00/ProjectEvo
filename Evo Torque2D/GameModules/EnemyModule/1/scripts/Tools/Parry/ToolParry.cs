//-----------------------------------------------------------------------------
// Parry Tool
//-----------------------------------------------------------------------------

function ToolParry::CreateInstance(%emyOwner, %type, %posX, %posY, %toolOrientation)  
{  
    %r = new SceneObject()  
	{  
		superclass = "ToolNode";
		class = "ToolParry";   
		owner = %emyOwner;
		toolType = %type;
		bodyPosX = %posX;
		bodyPosY = %posY;
		orientation = %toolOrientation;
		stackLevel = 1;
	};  
  
    return %r;  
}  

//-----------------------------------------------------------------------------

function ToolParry::initialize(%this)
{	
	exec("./ParrySpark.cs");

	Parent::initialize(%this);
	
	//-Stats--
	%this.parryChanceBonus = 0.1;
	
	%this.owner.parryChance += %this.parryChanceBonus*%this.stackLevel;
	
	if( !isObject(%this.owner.parryTools))
	{
		%this.owner.parryTools = new SimSet();
	}
	
	%this.owner.parryTools.add(%this);
	
	%this.owner.addEnemyBehavior("BlinkBehavior", 1);
	/*for(%i = 0; %i < %this.owner.behaviorNamesSize; %i += 2)		
	{
		echo("%this.behaviorNames   " @ %this.owner.behaviorNames[%i] SPC %this.owner.behaviorNames[%i + 1]);
	}
	echo("Size: " SPC %this.owner.behaviorNamesSize);*/
}

//-----------------------------------------------------------------------------

function ToolParry::setupSprite( %this )
{
	%this.owner.addSprite(%this.getRelativePosistion());
	
	%this.owner.setSpriteImage("GameAssets:tool_parry_a", 0);
	%this.owner.setSpriteSize(64 * %this.owner.sizeRatio, 64 * %this.owner.sizeRatio);
	%this.sortLevel = 6;
	
	%this.owner.setSpriteAngle(%this.orientation);
}

//-----------------------------------------------------------------------------
//for EnemyUnit class ;)

function EnemyUnit::generateParrySpark( %this )
{
	%roomDmgSplash = new Sprite() { 
		class = "ToolParrySpark";
		initDamage = %dmgAmount;
		driftAngle = %this.getAngle();
	};
	%this.getScene().add( %roomDmgSplash ); 
	
	%pos = %this.parryTools.getObject(getRandom(0, %this.parryTools.getCount() - 1)).getWorldPosistion();
	%roomDmgSplash.setPosition(%pos);
}

//-----------------------------------------------------------------------------

function ToolParry::setupBehaviors( %this )
{

}
