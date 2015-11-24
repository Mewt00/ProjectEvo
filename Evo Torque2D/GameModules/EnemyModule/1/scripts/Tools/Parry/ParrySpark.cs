//-----------------------------------------------------------------------------
// PlayerModule: playerBullet class and functions
//-----------------------------------------------------------------------------

function ToolParrySpark::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function ToolParrySpark::initialize(%this)
{		
	%this.setSceneLayer(4);
	%this.fixedAngle = true;

	//-Stats---
	%this.driftSpeed = 8;	
	%this.spinSpeed = 180;	
	%this.lifeSpan = 0.4 * 1000;	//ms
	
	//-Info---
	%this.sizeRatio = $pixelsToWorldUnits;
	%this.myWidth = 43 * %this.sizeRatio;
	%this.myHeight = 42 * %this.sizeRatio;
	
	%this.setupSprite();
		
	%this.setLinearVelocityPolar(%this.driftAngle+20, %this.driftSpeed);		//0 degrees is down
	%this.setAngularVelocity(%this.spinSpeed);

	%this.setCollisionSuppress();
    %this.setUpdateCallback(true);
	%this.setSceneGroup( Utility.getCollisionGroup("") );
	
	%this.mySchedule = schedule(%this.lifeSpan, 0, "ToolParrySpark::deleteThis", %this);
}

//-----------------------------------------------------------------------------

function ToolParrySpark::setupSprite( %this )
{
	%this.setImage( "GameAssets:parrySpark" );
	%this.setSize( %this.myWidth, %this.myHeight );
	
	%this.setSceneLayer(2);
}

//-----------------------------------------------------------------------------

function ToolParrySpark::onUpdate(%this)
{
	%this.setBlendAlpha(getEventTimeLeft(%this.mySchedule) / %this.lifeSpan);
}

//-----------------------------------------------------------------------------

function ToolParrySpark::deleteThis( %this )
{
	%this.safeDelete();
}

//-----------------------------------------------------------------------------

function ToolParrySpark::onRemove( %this )
{
}
