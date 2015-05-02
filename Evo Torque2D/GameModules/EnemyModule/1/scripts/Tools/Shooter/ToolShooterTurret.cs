//-----------------------------------------------------------------------------

function ToolShooterTurret::onAdd( %this )
{
	%this.initialize();
}
//-----------------------------------------------------------------------------

function ToolShooterTurret::initialize(%this)
{	
	%this.setSceneLayer(9);
	
	%this.myWidth = 104 * %this.owner.owner.sizeRatio;
	%this.myHeight = 64 * %this.owner.owner.sizeRatio;
	
	%this.setupSprite();
	
    %this.setUpdateCallback(true);
}

//-----------------------------------------------------------------------------

function ToolShooterTurret::setupSprite( %this )
{
	%this.addSprite();
	
	%this.setSpriteImage("GameAssets:tool_shooter_turret_a", 0);
	
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
}

//-----------------------------------------------------------------------------

function ToolShooterTurret::onUpdate( %this )
{
	if(!isObject(%this.owner))
	{
		%this.safeDelete();
	}
	else
	{
		if(isObject(%this.owner.owner.mainTarget))
		{
			%targetRotation = Vector2AngleToPoint(%this.owner.getWorldPosistion(), %this.owner.owner.mainTarget.getPosition());
			%this.rotateTo(%targetRotation, %this.turnSpeed);
		}
		%this.setPosition(%this.owner.getWorldPosistion());
	}
}
