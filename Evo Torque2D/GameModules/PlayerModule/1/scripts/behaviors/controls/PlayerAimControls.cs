//-----------------------------------------------------------------------------
// Basic player aim conrols
//-----------------------------------------------------------------------------

if (!isObject(PlayerAimControlsBehavior))
{
    %template = new BehaviorTemplate(PlayerAimControlsBehavior);

  %template.friendlyName = "Shooter Aiming";
  %template.behaviorType = "Input";
  %template.description  = "Shooter style aiming control";

  %template.addBehaviorField(upKey, "", keybind, "keyboard up");
  %template.addBehaviorField(dwnKey, "", keybind, "keyboard down");
  %template.addBehaviorField(lftKey, "", keybind, "keyboard left");
  %template.addBehaviorField(rgtKey, "", keybind, "keyboard right");
  
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
    GlobalActionMap.bindObj("keyboard", %this.upKey, "faceUp", %this);
    GlobalActionMap.bindObj("keyboard", %this.rgtKey, "faceRight", %this);
    GlobalActionMap.bindObj("keyboard", %this.dwnKey, "faceDown", %this);
    GlobalActionMap.bindObj("keyboard", %this.lftKey, "faceLeft", %this);
	
	//diagonal
    GlobalActionMap.bindObj("keyboard", %this.ulKey, "faceUL", %this);
    GlobalActionMap.bindObj("keyboard", %this.urKey, "faceUR", %this);
    GlobalActionMap.bindObj("keyboard", %this.drKey, "faceDR", %this);
    GlobalActionMap.bindObj("keyboard", %this.dlKey, "faceDL", %this);
}

function PlayerAimControlsBehavior::onBehaviorRemove(%this)
{
    if (!isObject(GlobalActionMap))
       return;

    GlobalActionMap.unbindObj("keyboard", %this.upKey, %this);
    GlobalActionMap.unbindObj("keyboard", %this.rgtKey, %this);
    GlobalActionMap.unbindObj("keyboard", %this.dwnKey, %this);
    GlobalActionMap.unbindObj("keyboard", %this.lftKey, %this);

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