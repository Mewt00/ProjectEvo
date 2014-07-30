//-----------------------------------------------------------------------------
// PlayerModule: EnemyUnit class and functions
//-----------------------------------------------------------------------------

function ToolTar::CreateInstance(%emyOwner, %type, %posX, %posY, %toolOrientation)  
{  
    %r = new SceneObject()  
	{  
		superclass = "ToolNode";
		class = "ToolTar";   
		owner = %emyOwner;
		toolType = %type;
		bodyPosX = %posX;
		bodyPosY = %posY;
		orientation = %toolOrientation;
		stackLevel = 1;
		reloadTime = 6.0 * 1000;
	};  
  
    return %r;  
}  

//-----------------------------------------------------------------------------

function ToolTar::initialize(%this)
{	
	exec("./EnemyTarShot.cs");
	exec("./EnemyTarSlick.cs");
	exec("./PlayerTarred.cs");
	
	Parent::initialize(%this);
	
	%this.reloadTime = %this.reloadTime/%this.stackLevel;
	%this.slowEffect = 10;
	%this.slowEffectDuration = 10 * 1000;
	
	//shot barrel offset (instead of bullet coming out of center of cannon)	
	%this.barrelXoffset = 55*%this.owner.sizeRatio;
	%this.barrelYoffset = 0*%this.owner.sizeRatio;	
}

//-----------------------------------------------------------------------------

function ToolTar::setupSprite( %this )
{
	%this.owner.addSprite(%this.getRelativePosistion());
	
	%this.owner.setSpriteImage("GameAssets:tool_tar_a", 0);
	%this.owner.setSpriteSize(64 * %this.owner.sizeRatio, 64 * %this.owner.sizeRatio);
	%this.sortLevel = 6;
	
	%this.owner.setSpriteAngle(%this.orientation);
}

//-----------------------------------------------------------------------------

function ToolTar::setupBehaviors( %this )
{
	exec("./behaviors/tarAI.cs");
	%baseAI = TarToolBehavior.createInstance();
	%this.addBehavior(%baseAI);

}

//-----------------------------------------------------------------------------

function ToolTar::shoot( %this )
{
	// add a bullet to the arena
	%newBullet = new CompositeSprite()
	{
		class = "EnemyTarShot";
		fireAngle = %this.owner.getAngle();
		slowEffect = %this.slowEffect;
		duration = %this.slowEffectDuration;
		targetLocation = %this.owner.mainTarget.getPosition();
		owner = %this;
		distTotal = (Vector2Distance(%this.getWorldPosistion(), %this.owner.mainTarget.getPosition()));
	};
	
	%newBullet.setPosition(%this.getWorldPosistion());
	%this.owner.getMyScene().add( %newBullet );
	
	
}