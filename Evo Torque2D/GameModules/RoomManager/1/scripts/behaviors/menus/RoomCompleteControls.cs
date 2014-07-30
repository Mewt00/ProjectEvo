//-----------------------------------------------------------------------------
// Basic menu controls and listeners
//-----------------------------------------------------------------------------

if (!isObject(RoomCompleteControls))
{
    %template = new BehaviorTemplate(RoomCompleteControls);

    %template.friendlyName = "Menu Controls";
    %template.behaviorType = "Input";
    %template.description  = "Menu and option basic control";

    %template.addBehaviorField(enterKey, "Key to bind to next room", keybind, "keyboard enter");
}

function RoomCompleteControls::onBehaviorAdd(%this)
{
    if (!isObject(GlobalActionMap))
       return;
	
    GlobalActionMap.bindObj(getWord(%this.enterKey, 0), getWord(%this.enterKey, 1), "changeRoom", %this);
}

function RoomCompleteControls::onBehaviorRemove(%this)
{
    if (!isObject(GlobalActionMap))
       return;

    GlobalActionMap.unbindObj(getWord(%this.enterKey, 0), getWord(%this.enterKey, 1), %this);
}

function RoomCompleteControls::changeRoom(%this, %val)
{
	if(%val == 1)
	{
		if(%this.owner.myManager != 0)
		{
			%this.owner.myManager.endRoomCompleteScreen();
			GlobalActionMap.unbindObj(getWord(%this.enterKey, 0), getWord(%this.enterKey, 1), %this);
			
			%mySchedule = schedule(10, 0, "RoomCompleteGUI::deleteThis", %this.owner);
		}
	}
}
