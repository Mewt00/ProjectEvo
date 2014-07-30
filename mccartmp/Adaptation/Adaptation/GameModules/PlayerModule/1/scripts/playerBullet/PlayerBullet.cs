//-----------------------------------------------------------------------------
// PlayerModule: playerBullet class and functions
//-----------------------------------------------------------------------------

function PlayerBullet::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function PlayerBullet::initialize(%this)
{
	%this.setSceneGroup(6);
	%this.setSceneLayer(6);
	%this.fixedAngle = true;
	
	%this.shotSpeed = 1000;		
	%this.setBullet(true);
	
	%this.setupSprite();
		
	%this.setLinearVelocityPolar(%this.fireAngle, -%this.shotSpeed);
	
	echo(%this.getLinearVelocity());
	
    %this.createPolygonBoxCollisionShape(%this.getWidth(), %this.getHeight());
    %this.setCollisionShapeIsSensor(0, true);
    %this.setCollisionGroups( "10 15" );
	%this.setCollisionCallback(true);
}

//-----------------------------------------------------------------------------

function PlayerBullet::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteImage("GameAssets:playershot", 0);
	%this.setSpriteSize(24, 39);
	%this.setAngle(%this.fireAngle);
}

//-----------------------------------------------------------------------------

function PlayerBullet::onCollision(%this, %object, %collisionDetails)
{
	if(%object.class $= "EnemyUnit")
	{
		%object.recycle(%object.side);
		%this.safeDelete();
	}
}

//-----------------------------------------------------------------------------

function PlayerBullet::destroy( %this )
{
}
