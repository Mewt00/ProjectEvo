//-----------------------------------------------------------------------------
// Shooter Tool AI
//-----------------------------------------------------------------------------

if (!isObject(ShooterToolBehavior))
{
    %template = new BehaviorTemplate(ShooterToolBehavior);

    %template.friendlyName = "Shooter tool AI";
    %template.behaviorType = "AI";
    %template.description  = "Shooter rep shooting";
}

function ShooterToolBehavior::onBehaviorAdd(%this)
{
	%this.mySchedule = schedule(getRandom(%this.owner.reloadTime) + $roomStartLag, 0, "ShooterToolBehavior::doShoot", %this);
}

function ShooterToolBehavior::doShoot(%this)
{
	if (isObject(%this.owner.owner) && isObject(%this.owner.owner.mainTarget))
	{
		%this.owner.shoot();
		
		%this.mySchedule = schedule(%this.owner.reloadTime, 0, "ShooterToolBehavior::doShoot", %this);
	}
}

function ShooterToolBehavior::onBehaviorRemove(%this)
{
}

//-----------------------------------------------------------------------------
/*
function PlayerStrike::onUpdate(%this)
{
	%this.setSpriteBlendAlpha(getEventTimeLeft(%this.mySchedule) / %this.lifeSpan);
}
*/
//------------------------------------------------------------------------------------

