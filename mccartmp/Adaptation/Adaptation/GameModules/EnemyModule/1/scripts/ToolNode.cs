//-----------------------------------------------------------------------------
// PlayerModule: EnemyUnit class and functions
//-----------------------------------------------------------------------------

function ToolNode::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function ToolNode::initialize(%this)
{	
	%this.setSceneGroup(10);		//Enemy Unit sceneGroup

	%this.myWidth = 64 * %this.owner.sizeRatio;
	%this.myHeight = 64 * %this.owner.sizeRatio;

	%this.setupBehaviors();
	%this.setupCollisionShape();

	%this.setCollisionCallback(false);
	
}

//-----------------------------------------------------------------------------

function ToolNode::setupCollisionShape( %this )
{
	%shapePoints = %this.bodyPosX*%this.myWidth SPC %this.bodyPosY*%this.myHeight 
		SPC (%this.bodyPosX + 1)*%this.myWidth SPC %this.bodyPosY*%this.myHeight 
		SPC (%this.bodyPosX + 1)*%this.myWidth SPC (%this.bodyPosY + 1)*%this.myHeight
		SPC %this.bodyPosX*%this.myWidth SPC (%this.bodyPosY + 1)*%this.myHeight;
		
	%this.owner.createPolygonCollisionShape(%shapePoints);
}

//-----------------------------------------------------------------------------

function ToolNode::setupSprites( %this )
{
	//echo(%this.getId() SPC %this.bodyPosX*%this.myWidth SPC %this.bodyPosY*%this.myHeight);
	
	%this.owner.addSprite(%this.bodyPosX*%this.myWidth SPC %this.bodyPosY*%this.myHeight);
	
	//switch...case
	if(%this.toolType $= "Blob") {
		%this.setupSpriteBlob();
	}
	else if(%this.toolType $= "Shooter"){
		%this.setupSpriteShooter();
	}
	else if(%this.toolType $= "Blade"){
		%this.setupSpriteBlade();
	}
	else if(%this.toolType $= "Armor"){
		%this.setupSpriteArmor();
	}
	
	%this.owner.setSpriteAngle(%this.orientation);
}

//-----------------------------------------------------------------------------

function ToolNode::setupSpriteBlob( %this )
{
	%this.owner.setSpriteImage("GameAssets:tool_blob1x1_a", 0);
	%this.owner.setSpriteSize(88 * %this.owner.sizeRatio, 88 * %this.owner.sizeRatio);
	%this.sortLevel = 2;
}

//-----------------------------------------------------------------------------

function ToolNode::setupSpriteShooter( %this )
{
	%this.owner.setSpriteImage("GameAssets:tool_shooter_a", 0);
	%this.owner.setSpriteSize(64 * %this.owner.sizeRatio, 64 * %this.owner.sizeRatio);
	%this.sortLevel = 6;
}

//-----------------------------------------------------------------------------

function ToolNode::setupSpriteBlade( %this )
{
	%this.owner.setSpriteImage("GameAssets:tool_blade_a", 0);
	%this.owner.setSpriteSize(64 * %this.owner.sizeRatio, 64 * %this.owner.sizeRatio);
	%this.sortLevel = 5;
}

//-----------------------------------------------------------------------------

function ToolNode::setupSpriteArmor( %this )
{
	%this.owner.setSpriteImage("GameAssets:tool_armor_a", 0);
	%this.owner.setSpriteSize(80 * %this.owner.sizeRatio, 144 * %this.owner.sizeRatio);
	%this.sortLevel = 1;
}

//-----------------------------------------------------------------------------

function ToolNode::setupBehaviors( %this )
{
	exec("./behaviors/movement/Drift.cs");
	%driftMove = DriftBehavior.createInstance();
	%this.addBehavior(%driftMove);
}

//-----------------------------------------------------------------------------

function ToolNode::getOpenSlots( %this )
{
	if(%this.toolType !$= "Blob")
		return "";
		
		
	%down = (%this.bodyPosX) SPC (%this.bodyPosY - 1);
	%right = (%this.bodyPosX + 1) SPC (%this.bodyPosY);
	%up = (%this.bodyPosX) SPC (%this.bodyPosY + 1);
	%left = (%this.bodyPosX - 1) SPC (%this.bodyPosY);
	
	return %down SPC %right SPC %up SPC %left;
}
//-----------------------------------------------------------------------------

function ToolNode::getBodyPosistion( %this )
{
	return (%this.bodyPosX) SPC (%this.bodyPosY);
}

//-----------------------------------------------------------------------------

function ToolNode::destroy( %this )
{
}
