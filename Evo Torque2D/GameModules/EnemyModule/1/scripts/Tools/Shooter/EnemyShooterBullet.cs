//-----------------------------------------------------------------------------
function EnemyShooterBullet::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function EnemyShooterBullet::initialize(%this)
{
	%this.setSceneGroup(Utility.getCollisionGroup("EnemyAttacks"));
	%this.setSceneLayer(9);
	%this.fixedAngle = true;
	
	%this.shotSpeed = 75;		
	%this.setBullet(true);
	%this.sizeRatio = $pixelToWorldRatio;
	%this.myWidth = 30 * %this.sizeRatio;
	%this.myHeight = 8 * %this.sizeRatio;
	
	%this.setupSprite();
		
	%this.setLinearVelocityPolar(%this.fireAngle+90, %this.shotSpeed);		//0 degrees is down
	
    %this.createPolygonBoxCollisionShape(%this.myWidth, %this.myHeight);
    %this.setCollisionShapeIsSensor(0, true);
	
    //%this.setCollisionGroups($UtilityObj.getCollisionGroup("Player") SPC $UtilityObj.getCollisionGroup("Walls"));
    %this.setCollisionGroups( Utility.getCollisionGroup("Player") SPC Utility.getCollisionGroup("PlayerBlock") SPC Utility.getCollisionGroup("Wall") );
	%this.setCollisionCallback(true);
}

//-----------------------------------------------------------------------------

function EnemyShooterBullet::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteImage("GameAssets:shootershot", 0);
	%this.setSpriteSize(2*%this.myWidth, 2*%this.myHeight);
	%this.setAngle(%this.fireAngle);
	%this.setVisible(false);
	%this.mySchedule = schedule(110, 0, "EnemyShooterBullet::becomeVisible", %this);		//"comes out of barrel"
}

//-----------------------------------------------------------------------------

function EnemyShooterBullet::onCollision(%this, %object, %collisionDetails)
{
	if(%object.getSceneGroup() == Utility.getCollisionGroup("Player"))
	{
		%this.owner.owner.shooterDamage += %object.hit(%this.shotDamage, %this.owner.owner);
		%this.safeDelete();
	}
	else if(%object.getSceneGroup() == Utility.getCollisionGroup("PlayerBlock"))
	{
		%object.takeDamage(%this.shotDamage);
		%this.safeDelete();
	}
	else if(%object.getSceneGroup() == Utility.getCollisionGroup("Wall"))
	{
		%this.safeDelete();
	}
}

//-----------------------------------------------------------------------------

function EnemyShooterBullet::becomeVisible( %this )
{
	if(isObject(%this))
		%this.setVisible(true);
}

//-----------------------------------------------------------------------------

function EnemyShooterBullet::destroy( %this )
{
}
