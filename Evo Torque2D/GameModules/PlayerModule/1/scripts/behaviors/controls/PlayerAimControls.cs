//-----------------------------------------------------------------------------
// Basic player aim conrols: joystick corresponds to 8 keys, for each cardinal direction
//-----------------------------------------------------------------------------

if (!isObject(PlayerAimControlsBehavior))
{
    %template = new BehaviorTemplate(PlayerAimControlsBehavior);

  %template.friendlyName = "Shooter Aiming";
  %template.behaviorType = "Input";
  %template.description  = "Shooter style aiming control";

  %template.addBehaviorField(upKey, "", keybind, "keyboard up");
  %template.addBehaviorField(downKey, "", keybind, "keyboard down");
  %template.addBehaviorField(leftKey, "", keybind, "keyboard left");
  %template.addBehaviorField(rightKey, "", keybind, "keyboard right");
  
  %template.addBehaviorField(ulKey, "", keybind, "keyboard I");
  %template.addBehaviorField(urKey, "", keybind, "keyboard O");
  %template.addBehaviorField(drKey, "", keybind, "keyboard Comma");
  %template.addBehaviorField(dlKey, "", keybind, "keyboard M");
}

function PlayerAimControlsBehavior::onBehaviorAdd(%this)
{
    if (!isObject(GlobalActionMap))
       return;
	   
	//cardinal
    GlobalActionMap.bindObj(getWord(%this.upKey, 0), getWord(%this.upKey, 1), "faceUp", %this);
    GlobalActionMap.bindObj(getWord(%this.rightKey, 0), getWord(%this.rightKey, 1), "faceRight", %this);
    GlobalActionMap.bindObj(getWord(%this.downKey, 0), getWord(%this.downKey, 1), "faceDown", %this);
    GlobalActionMap.bindObj(getWord(%this.leftKey, 0), getWord(%this.leftKey, 1), "faceLeft", %this);
	
	//diagonal
    GlobalActionMap.bindObj(getWord(%this.ulKey, 0), getWord(%this.ulKey, 1), "faceUL", %this);
    GlobalActionMap.bindObj(getWord(%this.urKey, 0), getWord(%this.urKey, 1), "faceUR", %this);
    GlobalActionMap.bindObj(getWord(%this.drKey, 0), getWord(%this.drKey, 1), "faceDR", %this);
    GlobalActionMap.bindObj(getWord(%this.dlKey, 0), getWord(%this.dlKey, 1), "faceDL", %this);
}

function PlayerAimControlsBehavior::onBehaviorRemove(%this)
{
    if (!isObject(GlobalActionMap))
       return;

    GlobalActionMap.unbindObj("keyboard", %this.upKey, %this);
    GlobalActionMap.unbindObj("keyboard", %this.rightKey, %this);
    GlobalActionMap.unbindObj("keyboard", %this.downKey, %this);
    GlobalActionMap.unbindObj("keyboard", %this.leftKey, %this);

    GlobalActionMap.unbindObj("keyboard", %this.ulKey, %this);
    GlobalActionMap.unbindObj("keyboard", %this.urKey, %this);
    GlobalActionMap.unbindObj("keyboard", %this.drKey, %this);
    GlobalActionMap.unbindObj("keyboard", %this.dlKey, %this);
}

//------------------------------------------------------------------------------------
  
function PlayerAimControlsBehavior::updateFacingDirection(%this, %direction)
{
	%this.owner.setAngle(%direction);
}

function PlayerAimControlsBehavior::faceUp(%this, %val)
{	
	%targetRotation = 90;
	
	if(%val == 1)
		%this.updateFacingDirection(%targetRotation);
}

function PlayerAimControlsBehavior::faceDown(%this, %val)
{	
	%targetRotation = 270;
	
    if(%val == 1)
		%this.updateFacingDirection(%targetRotation);
}

function PlayerAimControlsBehavior::faceLeft(%this, %val)
{
	%targetRotation = 180;
	
    if(%val == 1)
		%this.updateFacingDirection(%targetRotation);
}

function PlayerAimControlsBehavior::faceRight(%this, %val)
{
	%targetRotation = 0;
	
    if(%val == 1)
		%this.updateFacingDirection(%targetRotation);
}

//------------------------------------------------------------------------------------
function PlayerAimControlsBehavior::faceUR(%this, %val)
{	
	%targetRotation = 45;
	
	if(%val == 1)
		%this.updateFacingDirection(%targetRotation);
}

function PlayerAimControlsBehavior::faceDR(%this, %val)
{	
	%targetRotation = 315;
	
    if(%val == 1)
		%this.updateFacingDirection(%targetRotation);
}

function PlayerAimControlsBehavior::faceDL(%this, %val)
{
	%targetRotation = 225;
	
    if(%val == 1)
		%this.updateFacingDirection(%targetRotation);
}

function PlayerAimControlsBehavior::faceUL(%this, %val)
{
	%targetRotation = 135;
	
    if(%val == 1)
		%this.updateFacingDirection(%targetRotation);
}

//------------------------------------------------------------------------------------