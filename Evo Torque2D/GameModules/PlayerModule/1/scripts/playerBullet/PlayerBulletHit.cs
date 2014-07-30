//-----------------------------------------------------------------------------

function PlayerBulletHit::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function PlayerBulletHit::initialize(%this)
{
	%this.setSceneLayer(4);
	%this.fixedAngle = true;
		
	%this.sizeRatio = $pixelToWorldRatio;
	%this.myWidth = 18 * %this.sizeRatio * 2.5;
	%this.myHeight = 11 * %this.sizeRatio * 2.5;
	
	%this.setLifetime(0.25);
	
	%this.setupSprite();
}

//-----------------------------------------------------------------------------

function PlayerBulletHit::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteAnimation("GameAssets:playershotHitAnim", 0);
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
	%this.setAngle(%this.fireAngle - 90 );
}