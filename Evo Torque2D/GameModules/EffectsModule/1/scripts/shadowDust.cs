//-----------------------------------------------------------------------------\

function shadowDust::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function shadowDust::initialize(%this)
{
	%this.setupSprite();
	%this.setUpdateCallback(true);
	%this.setCollisionSuppress();
	%this.setSceneGroup( Utility.getCollisionGroup("") );
	
	%this.setUpdateCallback(true);
	
	%this.setLifetime(%this.lifeSpan);
	
	//%this.setAngle(getRandom(360));
	//%this.setAngularVelocity(getRandom(90, 180));
}
//-----------------------------------------------------------------------------

function shadowDust::setupSprite( %this )
{

	%this.addSprite("0 0");
	%this.setSpriteImage("GameAssets:shadowDust", 0);
	%this.setSpriteSize(134*$pixelToWorldRatio, 134*$pixelToWorldRatio);
	%this.setSceneLayer(2);
	%this.setSpriteFlipX(getRandom(0, 1));
	//%this.setSpriteFlipY(getRandom(0, 1));
}

//-----------------------------------------------------------------------------

function shadowDust::onUpdate(%this)
{
	%ratio = (%this.getLifetime() / %this.lifeSpan);
	%this.setSpriteBlendAlpha(1 - %ratio);
	%this.setSpriteSize(134*$pixelToWorldRatio*(1 - %ratio), 134*$pixelToWorldRatio*(1 - %ratio));
}

//-----------------------------------------------------------------------------

function shadowDust::onRemove( %this )
{
}
