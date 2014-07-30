//-----------------------------------------------------------------------------
// PlayerModule: EnemyUnit class and functions
//-----------------------------------------------------------------------------


function ToolAcidTurret::onAdd( %this )
{
	%this.initialize();
}
//-----------------------------------------------------------------------------

function ToolAcidTurret::initialize(%this)
{	
	%this.setSceneLayer(9);
	
	%this.myWidth = 108 * %this.owner.owner.sizeRatio;
	%this.myHeight = 64 * %this.owner.owner.sizeRatio;
	
	%this.setupSprite();
	
    %this.setUpdateCallback(true);
}

//-----------------------------------------------------------------------------

function ToolAcidTurret::setupSprite( %this )
{
	%this.addSprite();
	
	%this.setSpriteImage("GameAssets:tool_acid_turret_a", 0);
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
}

//-----------------------------------------------------------------------------

function ToolAcidTurret::onUpdate( %this )
{
	if(!isObject(%this.owner))
	{
		%this.safeDelete();
	}
	else
	{
		if(isObject(%this.owner.owner.mainTarget))
		{
			%targetRotation = Vector2AngleToPoint(%this.owner.getWorldPosistion(), %this.owner.owner.mainTarget.getPosition()) - 90;
			%this.rotateTo(%targetRotation, %this.turnSpeed);
		}
		%this.setPosition(%this.owner.getWorldPosistion());
	}
}
