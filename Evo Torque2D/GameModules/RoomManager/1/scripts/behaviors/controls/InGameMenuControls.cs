//-----------------------------------------------------------------------------
// Basic GlobalControlBehavior
//-----------------------------------------------------------------------------

if (!isObject(InGameMenuControlsBehavior))
{
	echo("inGameMenuActionMap");
    %template = new BehaviorTemplate(InGameMenuControlsBehavior);
	
	if(isObject(inGameMenuActionMap))
	{
		echo("inGameMenuActionMap being popped and deleted");
		inGameMenuActionMap.pop();
		inGameMenuActionMap.delete();
	}
	
	//new ActionMap(inGameMenuActionMap);
	
    %template.friendlyName = "Menu Controls";
    %template.behaviorType = "Input";
    %template.description  = "Exiting basic control";

    %template.addBehaviorField(exitKey, "Key to bind to game exit", keybind, "keyboard escape");
}

function InGameMenuControlsBehavior::onBehaviorAdd(%this)
{
	echo("ingame object?: " @ isObject(inGameMenuActionMap));
	
    if (!isObject(inGameMenuActionMap))
       return;
   
   	echo("ingame object?: " @ getWord(%this.exitKey, 0));

	
    inGameMenuActionMap.bindObj(getWord(%this.exitKey, 0), getWord(%this.exitKey, 1), "exitGame", %this);
	
	inGameMenuActionMap.push();
	
	echo("InGameMenuControlsBehavior.onBehaviorAdd()");
}

function InGameMenuControlsBehavior::onBehaviorRemove(%this)
{
    if (!isObject(inGameMenuActionMap))
       return;

    //%this.owner.setUpdateCallback(false);

    inGameMenuActionMap.unbindObj(getWord(%this.exitKey, 0), getWord(%this.exitKey, 1), %this);
	
	//inGameMenuActionMap.pop();
	inGameMenuActionMap.delete();
}

function InGameMenuControlsBehavior::exitGame(%this, %val)
{
	if(%val == 1)
	{
		echo("RoomManager.exitGame()");
		mainWindow.delete();
		inGameMenuActionMap.pop();
		//inGameMenuActionMap.delete();
		//alxStop($roomMusicHandle);
		alxStopAll();
		Canvas.pushDialog(MenuDialog);
		$titleMusicHandle = alxPlay("GameAssets:mainMenuMusic");
	}
}
