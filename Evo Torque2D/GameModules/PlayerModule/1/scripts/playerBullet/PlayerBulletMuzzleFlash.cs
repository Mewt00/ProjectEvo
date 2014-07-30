//-----------------------------------------------------------------------------
function PlayerBulletMuzzleFlash::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function PlayerBulletMuzzleFlash::initialize(%this)
{
	%this.setSceneLayer(4);
	%this.fixedAngle = true;
		
	%this.sizeRatio = $pixelToWorldRatio;
	%this.myWidth = 28 * %this.sizeRatio * 1.5;
	%this.myHeight = 38 * %this.sizeRatio * 1.5;
	
	%this.schedule(200, "safeDelete");
	
	%this.setupSprite();
	
	%this.setUpdateCallback(true);
}

//-----------------------------------------------------------------------------

function PlayerBulletMuzzleFlash::onUpdate( %this )
{
	%this.setPosition(%this.owner.owner.getWorldPoint(%this.owner.barrelXoffset + 27*%this.owner.owner.sizeRatio, %this.owner.barrelYoffset - 6*%this.owner.owner.sizeRatio) );
}

//-----------------------------------------------------------------------------

function PlayerBulletMuzzleFlash::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteAnimation("GameAssets:playershotMuzzleFlashAnim", 0);
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
	%this.setAngle(%this.fireAngle);
}