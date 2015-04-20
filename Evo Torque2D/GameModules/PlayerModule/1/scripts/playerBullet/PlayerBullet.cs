//-----------------------------------------------------------------------------
// Player bullet/shot
//-----------------------------------------------------------------------------

function PlayerBullet::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function PlayerBullet::initialize(%this)
{
	%this.setSceneGroup(Utility.getCollisionGroup("PlayerAttacks"));
	%this.setSceneLayer(6);
	%this.fixedAngle = true;
	
	%this.shotDamage = 10;
	%this.shotSpeed = 75;		
	%this.setBullet(true);
	%this.sizeRatio = $pixelToWorldRatio;
	%this.myWidth = 39 * %this.sizeRatio;
	%this.myHeight = 24 * %this.sizeRatio;
	
	%this.setupSprite();
		
	%this.setLinearVelocityPolar(%this.fireAngle+90, %this.shotSpeed);		//0 degrees is down
	
    %this.createPolygonBoxCollisionShape(%this.myWidth, %this.myHeight);
    %this.setCollisionShapeIsSensor(0, true);
    %this.setCollisionGroups( Utility.getCollisionGroup("Enemies") SPC Utility.getCollisionGroup("Wall") );
	%this.setCollisionCallback(true);
}

//-----------------------------------------------------------------------------

function PlayerBullet::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteImage("GameAssets:playershot", 0);
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
	%this.setAngle(%this.fireAngle);
}

//-----------------------------------------------------------------------------

function PlayerBullet::onCollision(%this, %object, %collisionDetails)
{
	if(%object.getSceneGroup() == Utility.getCollisionGroup("Enemies"))
	{
		%object.takeDamage(%this.shotDamage, "Ranged");
		
		// add a PlayerBulletHit to the arena
		%newHit = new CompositeSprite()
		{
			class = "PlayerBulletHit";
			fireAngle = %this.fireAngle;
			owner = %this.owner;
		};
		
		%this.owner.getScene().add( %newHit );
		%newHit.setPosition(%this.getPosition());
		
		
		%this.safeDelete();
	}
	else if(%object.getSceneGroup() == Utility.getCollisionGroup("Wall"))
	{
		%this.safeDelete();
	}
}

//-----------------------------------------------------------------------------

function PlayerBullet::destroy( %this )
{
}
