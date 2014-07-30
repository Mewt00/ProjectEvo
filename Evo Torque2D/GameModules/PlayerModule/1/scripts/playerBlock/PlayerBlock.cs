//-----------------------------------------------------------------------------
// PlayerModule: playerBullet class and functions
//-----------------------------------------------------------------------------

function PlayerBlock::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function PlayerBlock::initialize(%this)
{		
	%this.setSceneGroup(Utility.getCollisionGroup("PlayerBlock"));
	%this.setSceneLayer(4);
	%this.fixedAngle = true;
	
	%this.maxDamage = 100;
	%this.damage = 0;
	
	%this.sizeRatio = $pixelToWorldRatio;
	%this.myWidth = 210 * %this.sizeRatio;
	%this.myHeight = 210 * %this.sizeRatio;
	
	%this.setupSprite();
	
    %this.createCircleCollisionShape(%this.myWidth/2, 0, 0);
    %this.setCollisionShapeIsSensor(0, true);
	
	//%this.setCollisionCallback(true);
	
    %this.setUpdateCallback(true);
	
	//%this.mySchedule = schedule(%this.lifeSpan, 0, "PlayerStrike::deleteThis", %this);
}

//-----------------------------------------------------------------------------

function PlayerBlock::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteImage("GameAssets:playerblock", 0);
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
	%this.setAngle(%this.strikeAngle);
}

//-----------------------------------------------------------------------------

function PlayerBlock::onCollision(%this, %object, %collisionDetails)
{

}

//-----------------------------------------------------------------------------

function PlayerBlock::onUpdate(%this)
{
	%this.setPosition(%this.owner.getPosition());
	%this.setAngle(%this.owner.getAngle());
}

//-----------------------------------------------------------------------------

function PlayerBlock::takeDamage( %this, %dmgAmount )
{
	%this.damage = %this.damage + %dmgAmount;
	
	if( %this.damage >= %this.maxDamage)
	{
		%this.safeDelete();
	}
	else
	{
		%dmgRatio = (%this.damage/%this.maxDamage);
		%this.setSpriteBlendColor(1, 1 - %dmgRatio, 1 - %dmgRatio, 1 - %dmgRatio/2);
	}
}

//-----------------------------------------------------------------------------

function PlayerBlock::deleteThis( %this )
{
	%this.safeDelete();
}

//-----------------------------------------------------------------------------

function PlayerBlock::onRemove( %this )
{
	%this.owner.blockCooldown = %this.damage;
}

