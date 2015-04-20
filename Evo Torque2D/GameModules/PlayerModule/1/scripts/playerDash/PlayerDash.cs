//-----------------------------------------------------------------------------
// Player Dash: speed effect behind play, deal small damage to enemies hit
//-----------------------------------------------------------------------------

function PlayerDash::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function PlayerDash::initialize(%this)
{		
	%this.setSceneGroup(4);
	%this.setSceneLayer(4);
	%this.fixedAngle = true;
	
	%this.driftSpeed = 3;	
	%this.lifeSpan = 0.2 * 1000;	//ms
	
	%this.sizeRatio = $pixelToWorldRatio;
	%this.myWidth = 219 * %this.sizeRatio;
	%this.myHeight = 137 * %this.sizeRatio;
	
	%this.setupSprite();
		
	%this.setLinearVelocityPolar(%this.dashAngle + 180, %this.driftSpeed);		//0 degrees is down
	
   // %this.createPolygonBoxCollisionShape(%this.myWidth, %this.myHeight);
   // %this.setCollisionShapeIsSensor(0, true);
	//%this.setCollisionCallback(true);
	
    %this.setUpdateCallback(true);
	
	%this.mySchedule = schedule(%this.lifeSpan, 0, "PlayerDash::deleteThis", %this);
}

//-----------------------------------------------------------------------------

function PlayerDash::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteImage("GameAssets:playerdash", 0);
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
	%this.setAngle(%this.dashAngle);
}

//-----------------------------------------------------------------------------

function PlayerDash::onCollision(%this, %object, %collisionDetails)
{
	if(%this.fresh)
	{
		if(%object.class $= "EnemyUnit")
		{
			//%object.recycle(%object.side);
			
			%object.takeDamage(%this.strikeDamage);
			%this.fresh = false;
		}
	}
}

//-----------------------------------------------------------------------------

function PlayerDash::onUpdate(%this)
{
	%this.setSpriteBlendAlpha(getEventTimeLeft(%this.mySchedule) / %this.lifeSpan);
}

//-----------------------------------------------------------------------------

function PlayerDash::deleteThis( %this )
{
	%this.safeDelete();
}

//-----------------------------------------------------------------------------

function PlayerDash::destroy( %this )
{
}
