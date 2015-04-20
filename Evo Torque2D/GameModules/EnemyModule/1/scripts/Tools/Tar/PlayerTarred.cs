//-----------------------------------------------------------------------------
// Object that appears on top of player that shows they are tarred. 
// "Might not have been used. Instead placed by a simple coloring of the player" -Mike
//-----------------------------------------------------------------------------

function PlayerTarred::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function PlayerTarred::initialize(%this)
{	
	if(!isObject(%this.owner))			//no owner, no service
	{
		%this.safeDelete();
		return;
	}

	%this.setSceneLayer(4);
	
	%this.fadeSpan = %this.lifeSpan/6;
	
	%this.sizeRatio = $pixelToWorldRatio;
	%this.myWidth = 210*%this.sizeRatio;
	%this.myHeight = 164*%this.sizeRatio;
	%this.setupSprite();
	
    %this.setUpdateCallback(true);
	
	%this.mySchedule = schedule(%this.lifeSpan, 0, "PlayerTarred::deleteThis", %this);
}

//-----------------------------------------------------------------------------

function PlayerTarred::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteAnimation("GameAssets:playertarredAnim", 0);
	%this.setSpriteSize(%this.myWidth, %this.myHeight);
	%this.setAngle(%this.owner.getAngle());
}

//-----------------------------------------------------------------------------

function PlayerTarred::onUpdate(%this)
{
	if(!isObject(%this.owner))
	{
		%this.safeDelete();
		return;
	}
	%this.setPosition(%this.owner.getPosition());
	%this.setAngle(%this.owner.getAngle());
	
	
	if(getEventTimeLeft(%this.mySchedule) < %this.fadeSpan)
	{
		%this.setSpriteBlendAlpha(getEventTimeLeft(%this.mySchedule) / %this.fadeSpan);
	}
	
}

//-----------------------------------------------------------------------------

function PlayerTarred::deleteThis( %this )
{
	%this.safeDelete();
}

//-----------------------------------------------------------------------------

function PlayerTarred::onRemove( %this )
{
}
