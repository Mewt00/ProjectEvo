//-----------------------------------------------------------------------------
// Basic player controls & behaviors
//-----------------------------------------------------------------------------

if (!isObject(PlayerMouseInputBehavior))
{
    %template = new BehaviorTemplate(PlayerMouseInputBehavior);

    %template.friendlyName = "Mouse Controls";
    %template.behaviorType = "Input";
    %template.description  = "Shooter style movement control";

    %template.addBehaviorField(walkSpeed, "Speed of travel", float, 0.0);
}

function PlayerMouseInputBehavior::onBehaviorAdd(%this)
{
    if (!isObject(GlobalActionMap))
       return;
	   
	   
	//shot barrel offset (instead of bullet coming out of center of player)	
	%this.barrelXoffset = 55*%this.owner.sizeRatio;
	%this.barrelYoffset = -42*%this.owner.sizeRatio;
	
	//blade offset (instead of strike effect coming out of center of player)	
	%this.bladeXoffset = 100*%this.owner.sizeRatio;
	%this.bladeYoffset = 50*%this.owner.sizeRatio;
}

function PlayerMouseInputBehavior::onBehaviorRemove(%this)
{
}

//------------------------------------------------------------------------------------
   
function PlayerMouseInputBehavior::onTouchDown(%this, %touchID, %worldPosition, %mouseClicks)
{
	%this.pressFire();
	echo("rock out");
}

//------------------------------------------------------------------------------------
  
function PlayerMouseInputBehavior::updateMovement(%this)
{	 
    %this.owner.setLinearVelocityX((%this.right - %this.left) * %this.walkSpeed);
    %this.owner.setLinearVelocityY((%this.up - %this.down) * %this.walkSpeed);
}

function PlayerMouseInputBehavior::moveUp(%this, %val)
{
    %this.up = %val;
    %this.updateMovement();
}
//------------------------------------------------------------------------------------

function PlayerMouseInputBehavior::pressFire(%this)
{

		// add a bullet to the arena
		%newBullet = new CompositeSprite()
		{
			class = "PlayerBullet";
			fireAngle = %this.owner.getAngle();
		};
		
		%this.rangedCount++;
		%this.owner.getScene().add( %newBullet );
		
		%newBullet.setPosition(%this.owner.getWorldPoint(%this.barrelXoffset, %this.barrelYoffset) );
	
} 

//------------------------------------------------------------------------------------

function PlayerMouseInputBehavior::pressMelee(%this, %val)
{
	if(%val == 1)
	{
		// add a strike effect to the arena
		%newStriker = new CompositeSprite()
		{
			class = "PlayerStrike";
			strikeAngle = %this.owner.getAngle();
		};
			
		%this.meleeCount++;
		%this.owner.getScene().add( %newStriker );
		
		%newStriker.setPosition(%this.owner.getWorldPoint(%this.bladeXoffset, %this.bladeYoffset) );
	}
} 