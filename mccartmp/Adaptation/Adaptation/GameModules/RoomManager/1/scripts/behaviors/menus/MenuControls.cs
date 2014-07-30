//-----------------------------------------------------------------------------
// Basic menu controls and listeners
//-----------------------------------------------------------------------------

if (!isObject(MenuControlBehavior))
{
    %template = new BehaviorTemplate(MenuControlBehavior);

    %template.friendlyName = "Menu Controls";
    %template.behaviorType = "Input";
    %template.description  = "Menu and option basic control";

    %template.addBehaviorField(enterKey, "Key to bind to next room", keybind, "keyboard enter");
}

function MenuControlBehavior::onBehaviorAdd(%this)
{
    if (!isObject(GlobalActionMap))
       return;
	
    GlobalActionMap.bindObj(getWord(%this.enterKey, 0), getWord(%this.enterKey, 1), "changeRoom", %this);
}

function MenuControlBehavior::onBehaviorRemove(%this)
{
    if (!isObject(GlobalActionMap))
       return;

    %this.owner.disableUpdateCallback();

    GlobalActionMap.unbindObj(getWord(%this.enterKey, 0), getWord(%this.enterKey, 1), %this);
}

function MenuControlBehavior::changeRoom(%this, %val)
{
	if(%this.owner.myManager != 0)
	{
		%this.owner.myManager.changeToArena();
		GlobalActionMap.unbindObj(getWord(%this.enterKey, 0), getWord(%this.enterKey, 1), %this);
		//%this.owner.safeDelete();
	}
}
