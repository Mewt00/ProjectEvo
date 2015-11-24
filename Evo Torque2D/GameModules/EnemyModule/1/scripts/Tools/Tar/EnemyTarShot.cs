//-----------------------------------------------------------------------------
// 
//-----------------------------------------------------------------------------

function EnemyTarShot::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function EnemyTarShot::initialize(%this)
{
	%this.setSceneGroup(Utility.getCollisionGroup("EnemyAttacks"));
	%this.setSceneLayer(3);
	%this.fixedAngle = true;
	
	%this.shotSpeed = 55;		
	%this.setBullet(true);
	%this.sizeRatio = $pixelsToWorldUnits;
	%this.myWidth = 38 * %this.sizeRatio;
	%this.myHeight = 24 * %this.sizeRatio;
	%this.travelTime = (%this.distTotal/%this.shotSpeed)*1000;
	%this.travelSched = %this.schedule(%this.travelTime, "spawnSlick");
	
	%this.setupSprite();
		
	%this.setLinearVelocityPolar(%this.fireAngle, %this.shotSpeed);		//0 degrees is down
	
    %this.setUpdateCallback(true);
	
    %this.createCircleCollisionShape(%this.myWidth/2, 0, 0);
    %this.setCollisionShapeIsSensor(0, true);
	
    //%this.setCollisionGroups($UtilityObj.getCollisionGroup("Player") SPC $UtilityObj.getCollisionGroup("Walls"));
    %this.setCollisionGroups( Utility.getCollisionGroup("Player") SPC Utility.getCollisionGroup("PlayerBlock") SPC Utility.getCollisionGroup("Wall") );
	%this.setCollisionCallback(true);
}

//-----------------------------------------------------------------------------

function EnemyTarShot::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteImage("GameAssets:tarShot", 0);
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
	%this.setAngle(%this.fireAngle);
}
//-----------------------------------------------------------------------------

function EnemyTarShot::onUpdate( %this )
{
	%timeTraveled = getEventTimeLeft(%this.travelSched);
	
	%time = ((%this.travelTime/2) - %timeTraveled)/(%this.travelTime/2);
	
	%sizeRatio = 2 - (%time*%time);
	
	if(%sizeRatio < 1)
		%sizeRatio = 1;
	
	%this.setSpriteSize(%this.myWidth*%sizeRatio, %this.myHeight*%sizeRatio);	
	
}
//-----------------------------------------------------------------------------

function EnemyTarShot::onCollision(%this, %object, %collisionDetails)
{
	if(%object.getSceneGroup() == Utility.getCollisionGroup("Player"))
	{
		%this.spawnSlick();
	}
	else if(%object.getSceneGroup() == Utility.getCollisionGroup("PlayerBlock"))
	{
		%this.spawnSlick();
	}
	else if(%object.getSceneGroup() == Utility.getCollisionGroup("Wall"))
	{
		%this.spawnSlick();
	}
}

//-----------------------------------------------------------------------------

function EnemyTarShot::spawnSlick( %this )
{
	%newSlick = new CompositeSprite()
	{
		class = "EnemyTarSlick";
		slowEffect = %this.slowEffect;
		duration = %this.duration;
		owner = %this.owner;
	};
	
	%this.owner.owner.getMyScene().add( %newSlick );
	
	%newSlick.setPosition(%this.getPosition());

	%this.safeDelete();
}

//-----------------------------------------------------------------------------

function EnemyTarShot::onRemove( %this )
{
}
