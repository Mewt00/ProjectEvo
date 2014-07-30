//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(GenericButtonBehavior))
{
   %template = new BehaviorTemplate(GenericButtonBehavior);
   
   %template.friendlyName = "Generic Button";
   %template.behaviorType = "GUI";
   %template.description  = "Executes a method when clicked. The method is called on the behavior's owner";

   %template.addBehaviorField(hoverImage, "The image to display when mousing over the object", object, "", t2dImageMapDatablock);
   %template.addBehaviorField(hoverFrame, "The frame of the hoverImage to display", int, 0);
   %template.addBehaviorField(clickImage, "The image to display when clicking on the object", object, "", t2dImageMapDatablock);
   %template.addBehaviorField(clickFrame, "The frame of the clickImage to display", int, 0);
   %template.addBehaviorField(method, "The method to call on mouse up", string, "onClick");
}

function GenericButtonBehavior::onBehaviorAdd(%this)
{
   %this.owner.setUseMouseEvents(true);
   
   %this.startImage = %this.owner.getImageMap();
   %this.startFrame = %this.owner.getFrame();
}

function GenericButtonBehavior::onMouseDown(%this, %modifier, %worldPos)
{
   if (!isObject(%this.clickImage))
      return;
   
   %this.owner.setImageMap(%this.clickImage, %this.clickFrame);
}

function GenericButtonBehavior::onMouseUp(%this, %modifier, %worldPos)
{
   if (isObject(%this.hoverImage))
      %this.owner.setImageMap(%this.hoverImage, %this.hoverFrame);
   else
      %this.owner.setImageMap(%this.startImage, %this.startFrame);
   
   if (%this.owner.isMethod(%this.method))
      %this.owner.call(%this.method);
}

function GenericButtonBehavior::onMouseEnter(%this, %modifier, %worldPos)
{
   if (!isObject(%this.hoverImage))
      return;
   
   %this.owner.setImageMap(%this.hoverImage, %this.hoverFrame);
}

function GenericButtonBehavior::onMouseLeave(%this, %modifier, %worldPos)
{
   %this.owner.setImageMap(%this.startImage, %this.startFrame);
}
