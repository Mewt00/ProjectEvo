//-----------------------------------------------------------------------------
// Acid Tool AI
//-----------------------------------------------------------------------------

if (!isObject(AcidToolBehavior))
{
    %template = new BehaviorTemplate(AcidToolBehavior);

    %template.friendlyName = "Acid tool AI";
    %template.behaviorType = "AI";
    %template.description  = "Acid rep shooting";
}

function AcidToolBehavior::onBehaviorAdd(%this)
{
	%this.mySchedule = schedule(getRandom(%this.owner.reloadTime) + $roomStartLag, 0, "AcidToolBehavior::doShoot", %this);
}

function AcidToolBehavior::doShoot(%this)
{
	if (isObject(%this.owner.owner) && isObject(%this.owner.owner.mainTarget))
	{
		%this.owner.shoot();
		
		%this.mySchedule = schedule(%this.owner.reloadTime, 0, "AcidToolBehavior::doShoot", %this);
	}
}

function AcidToolBehavior::onBehaviorRemove(%this)
{
}