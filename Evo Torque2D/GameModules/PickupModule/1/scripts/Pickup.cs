//-----------------------------------------------------------------------------
// PickupModule:
// "Was intended to be the parent class, but instead wound up as simply the health pickup" -Mike
//-----------------------------------------------------------------------------

function Pickup::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function Pickup::initialize(%this)
{
	%this.setSceneGroup(Utility.getCollisionGroup("Pickups"));
	%this.setSceneLayer(28);
	%this.fixedAngle = true;
	
	//-Stats---
	%this.healthPickup = 50;
	%this.spinSpeed = 30;	
	
	//-Info---
	%this.sizeRatio = $pixelsToWorldUnits;
	%this.myWidth = 1.5 * 32 * %this.sizeRatio;
	%this.myHeight = 1.5 * 32 * %this.sizeRatio;
	
	%this.setAngularVelocity(%this.spinSpeed);
	
	%this.setupSprite();
	
	%this.setupCollisionShape();

}

//-----------------------------------------------------------------------------

function Pickup::setupCollisionShape( %this )
{	
	%boxSizeRatio = 0.75;
	
    %this.createCircleCollisionShape(%this.myWidth/2, 0, 0);
    %this.setCollisionShapeIsSensor(0, true);
    %this.setCollisionGroups( Utility.getCollisionGroup("Player") );
	%this.setCollisionCallback(true);
}
//-----------------------------------------------------------------------------

function Pickup::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteImage("GameAssets:pickup_health", 0);
	%this.setSpriteName("BodyAnim");
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
}

//-----------------------------------------------------------------------------

function Pickup::onCollision(%this, %object, %collisionDetails)
{
	if(%object.getSceneGroup() == Utility.getCollisionGroup("Player"))
	{
		%this.pickupAction(%object);
	}
}

//-----------------------------------------------------------------------------
//Health (negative damage) player

function Pickup::pickupAction( %this, %playerObj )
{
	%playerObj.takeDamage(-%this.healthPickup);
	%this.safeDelete();
}


//-----------------------------------------------------------------------------

function Pickup::onRemove( %this )
{
}
