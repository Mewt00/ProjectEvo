//-----------------------------------------------------------------------------
// 
//-----------------------------------------------------------------------------

function EnemyAcidSpray::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function EnemyAcidSpray::initialize(%this)
{
	%this.setSceneGroup(Utility.getCollisionGroup("EnemyAttacks"));
	%this.setSceneLayer(4);
	%this.fixedAngle = true;
	
	%this.fresh = true;
	%this.refresh = false;
	
	//-Stats--
	%this.shotSpeed = 0;	
	%this.lifeSpan = 1.5 * 1000;	//ms	1.5s
	%this.refreshRate = %this.lifeSpan/3;	
	
	
	%this.setBullet(true);
	%this.sizeRatio = $pixelToWorldRatio;
	%this.myWidth = 192 * %this.sizeRatio;
	%this.myHeight = 64 * %this.sizeRatio;
	
	%this.setupSprite();
		
	%this.setLinearVelocityPolar(%this.fireAngle+90, %this.shotSpeed);		//0 degrees is down
	
    //%this.createPolygonBoxCollisionShape(%this.myWidth, %this.myHeight, %this.myWidth/2, 0);
    %this.setCollisionShapeIsSensor(0, true);
	
    %this.setUpdateCallback(true);
	%this.setGatherContacts(true);
	
    //%this.setCollisionGroups($UtilityObj.getCollisionGroup("Player") SPC $UtilityObj.getCollisionGroup("Walls"));
    %this.setCollisionGroups( Utility.getCollisionGroup("Player") SPC Utility.getCollisionGroup("PlayerBlock") );
	%this.setCollisionCallback(true);
	
	
	%this.mySchedule = schedule(%this.lifeSpan, 0, "EnemyAcidSpray::deleteThis", %this);
}

//-----------------------------------------------------------------------------

function EnemyAcidSpray::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteAnimation("GameAssets:acidSprayAnim", 0);
	%this.setSpriteSize(%this.myWidth*2, %this.myHeight);
	
	//%this.setPosition(-192*%this.sizeRatio,0);
	
	//%this.setSpriteAngle(%this.fireAngle);
}

//-----------------------------------------------------------------------------

function EnemyAcidSpray::onCollision(%this, %object, %collisionDetails)
{
	if(%this.fresh)
	{
		if(%object.getSceneGroup() == Utility.getCollisionGroup("Player"))
		{
			%this.hit(%object);
			%this.fresh = false;
			//%this.safeDelete();
		}
	}
}

//-----------------------------------------------------------------------------

function EnemyAcidSpray::hit(%this, %player)
{
	if(isObject(%player.blocker))
	{
		%player.blocker.takeDamage(%this.sprayDamageToShield);
	}
	else
	{
		%player.takeDamage(%this.sprayDamage, %this.owner.owner);
	}
	%this.refresh = false;
	%this.myRefreshSchedule = schedule(%this.refreshRate, 0, "EnemyAcidSpray::refreshThis", %this);
}

//-----------------------------------------------------------------------------

function EnemyAcidSpray::onUpdate(%this)
{
	if(isObject(%this.owner))
	{
		%this.setPosition(%this.owner.getWorldPosistion());
		%this.setAngle(%this.owner.myTurret.getAngle() );
		
		%this.clearCollisionShapes();
		%collShapeWidth = %this.myWidth * (1 - ((getEventTimeLeft(%this.mySchedule) / %this.lifeSpan)/1.5)*((getEventTimeLeft(%this.mySchedule) / %this.lifeSpan)/1.5));
		
		%this.createPolygonBoxCollisionShape(%collShapeWidth, %this.myHeight, %collShapeWidth/2, 0);
		
		%this.setCollisionShapeIsSensor(0, true);
		
		if((!%this.fresh) && (%this.refresh))
		{
			if( %this.getContactCount() > 0)
			{
				%this.hit(%this.owner.owner.myArena.player);
			}
			
		}
	}
}

//-----------------------------------------------------------------------------

function EnemyAcidSpray::refreshThis( %this )
{
	%this.refresh = true;
}

//-----------------------------------------------------------------------------

function EnemyAcidSpray::deleteThis( %this )
{
	%this.safeDelete();
}

//-----------------------------------------------------------------------------

function EnemyAcidSpray::onRemove( %this )
{
}
