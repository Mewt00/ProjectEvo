//-----------------------------------------------------------------------------
// Tar Tool AI
//-----------------------------------------------------------------------------

if (!isObject(TarToolBehavior))
{
    %template = new BehaviorTemplate(TarToolBehavior);

    %template.friendlyName = "Shooter tool AI";
    %template.behaviorType = "AI";
    %template.description  = "Shooter rep shooting";
}

function TarToolBehavior::onBehaviorAdd(%this)
{
	%this.mySchedule = schedule(getRandom(%this.owner.reloadTime) + $roomStartLag, 0, "TarToolBehavior::doShoot", %this);
}

function TarToolBehavior::doShoot(%this)
{
	if (isObject(%this.owner.owner) && isObject(%this.owner.owner.mainTarget))
	{
		%this.owner.shoot();
		
		%this.mySchedule = schedule(%this.owner.reloadTime, 0, "TarToolBehavior::doShoot", %this);
	}
}

function TarToolBehavior::onBehaviorRemove(%this)
{
}
