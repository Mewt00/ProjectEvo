//-----------------------------------------------------------------------------
// Effects: Health pickup. a copy of Pickup.cs?
// TODO: I believe this was a preliminary to the "PickupModule/1/scripts/Pickup.cs", most likely delete this file.
//-----------------------------------------------------------------------------

function Effects::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function Effects::initialize(%this)
{
	//exec("./x/x.cs");
	
	%this.setSceneGroup(Utility.getCollisionGroup("Pickups"));
	%this.setSceneLayer(29);
	%this.fixedAngle = true;
	
	//Stats
	%this.sizeRatio = $pixelToWorldRatio;
	%this.myWidth = 1.5 * 32 * %this.sizeRatio;
	%this.myHeight = 1.5 * 32 * %this.sizeRatio;
	%this.healthPickup = 50;
	
	%this.setPosition(30, -30);
	
	%this.setupSprite();
	
	%this.setupCollisionShape();

}

//-----------------------------------------------------------------------------

function Effects::setupCollisionShape( %this )
{	
	%boxSizeRatio = 0.75;
	
    %this.createCircleCollisionShape(%this.myWidth/2, 0, 0);
    %this.setCollisionShapeIsSensor(0, true);
    %this.setCollisionGroups( Utility.getCollisionGroup("Player") );
	%this.setCollisionCallback(true);
}
//-----------------------------------------------------------------------------

function Effects::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteImage("GameAssets:pickup_health", 0);
	%this.setSpriteName("BodyAnim");
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
}

//-----------------------------------------------------------------------------

function Effects::onCollision(%this, %object, %collisionDetails)
{
	if(%object.getSceneGroup() == Utility.getCollisionGroup("Player"))
	{
		%this.pickupAction(%object);
	}
}

//-----------------------------------------------------------------------------

function Effects::pickupAction( %this, %playerObj )
{
	%playerObj.takeDamage(-%this.healthPickup);
	%this.safeDelete();
}


//-----------------------------------------------------------------------------

function Effects::onRemove( %this )
{
}
