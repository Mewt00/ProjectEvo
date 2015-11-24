//-----------------------------------------------------------------------------
// Slick on ground, that when touched- slows the player
//-----------------------------------------------------------------------------

function EnemyTarSlick::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function EnemyTarSlick::initialize(%this)
{
	%this.setSceneGroup(Utility.getCollisionGroup("EnemyAttacks"));
	%this.setSceneLayer(29);
	%this.fixedAngle = true;
		
	%this.sizeRatio = $pixelsToWorldUnits;
	%this.myWidth = 128 * %this.sizeRatio;
	%this.myHeight = 128 * %this.sizeRatio;
	
	%this.fresh = true;
	%this.lifeSpan = 5.0 * 1000;	//ms
	%this.fadeSpan = %this.lifeSpan/6;
	
	%this.setupSprite();
		
    %this.setUpdateCallback(true);
	
    //%this.createPolygonBoxCollisionShape(%this.myWidth, %this.myHeight);
    %this.createCircleCollisionShape(%this.myWidth/2, 0, 0);
    %this.setCollisionShapeIsSensor(0, true);
	
    //%this.setCollisionGroups($UtilityObj.getCollisionGroup("Player") SPC $UtilityObj.getCollisionGroup("Walls"));
    %this.setCollisionGroups( Utility.getCollisionGroup("Player") );
	%this.setCollisionCallback(true);
	
	%this.mySchedule = schedule(%this.lifeSpan, 0, "EnemyTarSlick::deleteThis", %this);
}

//-----------------------------------------------------------------------------

function EnemyTarSlick::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteImage("GameAssets:tarSlick", 0);
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
	%this.setAngle(getRandom(360));
}

//-----------------------------------------------------------------------------

function EnemyTarSlick::onCollision(%this, %object, %collisionDetails)
{
	if(%this.fresh)
	{
		if(%object.getSceneGroup() == Utility.getCollisionGroup("Player"))
		{
			%object.tar(%this.slowEffect, %this.duration);
			
			%this.fresh = false;
			
			/*
			%newPlayerTarred = new CompositeSprite()
			{
				class = "PlayerTarred";
				owner = %object;
				lifeSpan = %this.duration;
			};
			
			%this.owner.owner.getMyScene().add( %newPlayerTarred );
			*/
		}
	}
}

//-----------------------------------------------------------------------------

function EnemyTarSlick::onUpdate(%this)
{
	if(getEventTimeLeft(%this.mySchedule) < %this.fadeSpan)
	{
		%this.setSpriteBlendAlpha(getEventTimeLeft(%this.mySchedule) / %this.fadeSpan);
	}
}

//-----------------------------------------------------------------------------

function EnemyTarSlick::deleteThis( %this )
{
	%this.safeDelete();
}


//-----------------------------------------------------------------------------

function EnemyTarSlick::onRemove( %this )
{
}
