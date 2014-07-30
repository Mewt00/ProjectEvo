//-----------------------------------------------------------------------------
// Healthbar: Healthbar class and functions
//-----------------------------------------------------------------------------

function Healthbar::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function Healthbar::initialize(%this)
{	
	if(!isObject(%this.owner))			//no owner, no service
	{
		%this.safeDelete();
		return;
	}
		
	%this.setSceneLayer(1);
	
    %this.setUpdateCallback(true);
	
	%this.sizeRatio = $pixelToWorldRatio;
	
	%this.segmentWidth = 12 * %this.sizeRatio;
	%this.segmentHeight = 12 * %this.sizeRatio;
	
	%this.assessDamage( );		//go to draw for first time
}

//-----------------------------------------------------------------------------

function Healthbar::drawHealthbar(%this, %health)
{
	%numSegs = %health/10;
	
	%currX = -(%numSegs*%this.segmentWidth)/2;
	for(%i = 0; %i < %numSegs; %i++)
	{
		%this.addSprite(%currX SPC "0");
		%this.setSpriteImage("GameAssets:healthBarSegment", 0);
		%this.setSpriteSize(%this.segmentWidth, %this.segmentHeight);
		
		%currX += %this.segmentWidth;
	}
}

//-----------------------------------------------------------------------------

function Healthbar::drawHealthbarCurved(%this, %health, %totalHealth)
{
	%numGreenSegs = %health/10;
	%numSegs = %totalHealth/10;
	
	%currX = -(%numSegs*%this.segmentWidth)/2;
	
	for(%i = 0; %i < %numSegs; %i++)
	{
		%currY = -(%currX*%currX)/60;
		%this.addSprite(%currX SPC %currY);
		
		if(%i < %numGreenSegs)
			%this.setSpriteImage("GameAssets:healthBarSegment", 0);
		else
			%this.setSpriteImage("GameAssets:healthBarSegmentRed", 0);
		 
		%this.setSpriteSize(%this.segmentWidth, %this.segmentHeight);
		
		%currX += %this.segmentWidth;
	}
}
//-----------------------------------------------------------------------------

function Healthbar::onUpdate(%this)
{
	if(!isObject(%this.owner))
	{
		%this.safeDelete();
		return;
	}
	%this.setPosition(getWord(%this.owner.getPosition(), 0)+%this.xOffset, getWord(%this.owner.getPosition(), 1)+%this.yOffset);
}

//-----------------------------------------------------------------------------

function Healthbar::assessDamage( %this )
{
	if(!isObject(%this.owner))
	{
		%this.safeDelete();
		return;
	}
		
	%this.clearSprites();

	if(%this.curved)
		%this.drawHealthbarCurved(%this.owner.health, %this.owner.fullHealth);
	else
		%this.drawHealthbar(%this.owner.health);
}

//-----------------------------------------------------------------------------

function Healthbar::destroy( %this )
{
}
