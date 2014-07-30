//-----------------------------------------------------------------------------
// PickupModule: player class and functions
//-----------------------------------------------------------------------------

function SpeedShotPickup::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function SpeedShotPickup::initialize(%this)
{
	//exec("./x/x.cs");
	
	%this.setSceneGroup(Utility.getCollisionGroup("Pickups"));
	%this.setSceneLayer(28);
	%this.fixedAngle = true;
	
	//-Stats---
	%this.modifier = 2.0;
	%this.duration = 5*1000;
	%this.spinSpeed = 30;	
	
	//-Info---
	%this.sizeRatio = $pixelToWorldRatio;
	%this.myWidth = 1.5 * 32 * %this.sizeRatio;
	%this.myHeight = 1.5 * 32 * %this.sizeRatio;
	
	%this.setAngularVelocity(%this.spinSpeed);
	
	%this.setupSprite();
	
	%this.setupCollisionShape();

}

//-----------------------------------------------------------------------------

function SpeedShotPickup::setupCollisionShape( %this )
{	
	%boxSizeRatio = 0.75;
	
    %this.createCircleCollisionShape(%this.myWidth/2, 0, 0);
    %this.setCollisionShapeIsSensor(0, true);
    %this.setCollisionGroups( Utility.getCollisionGroup("Player") );
	%this.setCollisionCallback(true);
}
//-----------------------------------------------------------------------------

function SpeedShotPickup::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteImage("GameAssets:pickup_speedshot", 0);
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
}

//-----------------------------------------------------------------------------

function SpeedShotPickup::onCollision(%this, %object, %collisionDetails)
{
	if(%object.getSceneGroup() == Utility.getCollisionGroup("Player"))
	{
		%this.pickupAction(%object);
	}
}

//-----------------------------------------------------------------------------

function SpeedShotPickup::pickupAction( %this, %playerObj )
{
	%playerObj.updateFireRate(%playerObj.base_fireRate*%this.modifier);
	%playerObj.schedule(%this.duration, "updateFireRate", %playerObj.base_fireRate);
	%this.safeDelete();
}


//-----------------------------------------------------------------------------

function SpeedShotPickup::onRemove( %this )
{
}
