//-----------------------------------------------------------------------------
// Basic menu controls and listeners
//-----------------------------------------------------------------------------

if (!isObject(RoomDefeatControls))
{
    %template = new BehaviorTemplate(RoomDefeatControls);

    %template.friendlyName = "Menu Controls";
    %template.behaviorType = "Input";
    %template.description  = "Menu and option basic control";

    %template.addBehaviorField(enterKey, "Key to bind to next room", keybind, "keyboard enter");
}

function RoomDefeatControls::onBehaviorAdd(%this)
{
    if (!isObject(GlobalActionMap))
       return;
	
    GlobalActionMap.bindObj(getWord(%this.enterKey, 0), getWord(%this.enterKey, 1), "changeRoom", %this);
}

function RoomDefeatControls::onBehaviorRemove(%this)
{
    if (!isObject(GlobalActionMap))
       return;

    GlobalActionMap.unbindObj(getWord(%this.enterKey, 0), getWord(%this.enterKey, 1), %this);
}

function RoomDefeatControls::changeRoom(%this, %val)
{
	if(%val == 1)
	{
		if(%this.owner.myManager != 0)
		{
		/*	echo("RoomManager.exitGame()");
		mainWindow.delete();
		inGameMenuActionMap.pop();
		//inGameMenuActionMap.delete();
		//alxStop($roomMusicHandle);
		alxStopAll();
		Canvas.pushDialog(MenuDialog);
		$titleMusicHandle = alxPlay("GameAssets:mainMenuMusic");*/
			
			%this.owner.myScene.schedule(320, "clear");  
			%this.owner.myManager.goToTitleScreen();
			GlobalActionMap.unbindObj(getWord(%this.enterKey, 0), getWord(%this.enterKey, 1), %this);
			
			%mySchedule = schedule(10, 0, "RoomDefeatGUI::deleteThis", %this.owner);
		}
	}
}
