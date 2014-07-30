//-----------------------------------------------------------------------------
// Basic GlobalControlBehavior
//-----------------------------------------------------------------------------

if (!isObject(GlobalControlBehavior))
{
    %template = new BehaviorTemplate(GlobalControlBehavior);

    %template.friendlyName = "Menu Controls";
    %template.behaviorType = "Input";
    %template.description  = "Menu and option basic control";

    %template.addBehaviorField(exitKey, "Key to bind to game exit", keybind, "keyboard escape");
}

function GlobalControlBehavior::onBehaviorAdd(%this)
{
    if (!isObject(GlobalActionMap))
       return;
	
    GlobalActionMap.bindObj(getWord(%this.enterKey, 0), getWord(%this.enterKey, 1), "exitGame", %this);
	
	echo("GlobalControlBehavior.onBehaviorAdd()");
}

function GlobalControlBehavior::onBehaviorRemove(%this)
{
    if (!isObject(GlobalActionMap))
       return;

    %this.owner.disableUpdateCallback();

    GlobalActionMap.unbindObj(getWord(%this.enterKey, 0), getWord(%this.enterKey, 1), %this);
}

function GlobalControlBehavior::exitGame(%this, %val)
{
	if(%val == 1)
	{
		echo("GlobalControlBehavior.exitGame()");
		quit();
	}
}
