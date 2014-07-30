//-----------------------------------------------------------------------------
// PlayerModule: EnemyUnit class and functions
//-----------------------------------------------------------------------------

function ToolArmor::CreateInstance(%emyOwner, %type, %posX, %posY, %toolOrientation)  
{  
    %r = new SceneObject()  
	{  
		superclass = "ToolNode";
		class = "ToolArmor";   
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

function ToolArmor::initialize(%this)
{	
	Parent::initialize(%this);
	
	//-Stats--
	%this.armorRating = 2;
	
	%this.owner.armorValue += %this.armorRating*%this.stackLevel;
}

//-----------------------------------------------------------------------------

function ToolArmor::setupSprite( %this )
{
	%this.owner.addSprite(%this.getRelativePosistion());
	
	%this.owner.setSpriteImage("GameAssets:tool_armor_a", 0);
	%this.owner.setSpriteSize(144 * %this.owner.sizeRatio, 80 * %this.owner.sizeRatio);
	%this.sortLevel = 1;
	
	%this.owner.setSpriteAngle(%this.orientation);
}

//-----------------------------------------------------------------------------

function ToolArmor::setupBehaviors( %this )
{

}
