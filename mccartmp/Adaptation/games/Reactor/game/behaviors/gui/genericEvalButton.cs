//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(GenericEvalButtonBehavior))
{
   %template = new BehaviorTemplate(GenericEvalButtonBehavior);
   
   %template.friendlyName = "Generic Eval Button";
   %template.behaviorType = "GUI";
   %template.description  = "Eval's the given statement when clicked.";

   %template.addBehaviorField(hoverImage, "The image to display when mousing over the object", object, "", t2dImageMapDatablock);
   %template.addBehaviorField(hoverFrame, "The frame of the hoverImage to display", int, 0);
   %template.addBehaviorField(clickImage, "The image to display when clicking on the object", object, "", t2dImageMapDatablock);
   %template.addBehaviorField(clickFrame, "The frame of the clickImage to display", int, 0);
   %template.addBehaviorField(statement, "The statement to eval on mouse up", string, "onClick");
}

function GenericEvalButtonBehavior::onBehaviorAdd(%this)
{
   %this.owner.setUseMouseEvents(true);
   
   %this.startImage = %this.owner.getImageMap();
   %this.startFrame = %this.owner.getFrame();
}

function GenericEvalButtonBehavior::onMouseDown(%this, %modifier, %worldPos)
{
   if (!isObject(%this.clickImage))
      return;
   
   %this.owner.setImageMap(%this.clickImage, %this.clickFrame);
}

function GenericEvalButtonBehavior::onMouseUp(%this, %modifier, %worldPos)
{
   if (isObject(%this.hoverImage))
      %this.owner.setImageMap(%this.hoverImage, %this.hoverFrame);
   else
      %this.owner.setImageMap(%this.startImage, %this.startFrame);
   
   eval(%this.statement);
   
}

function GenericEvalButtonBehavior::onMouseEnter(%this, %modifier, %worldPos)
{
   if (!isObject(%this.hoverImage))
      return;
   
   %this.owner.setImageMap(%this.hoverImage, %this.hoverFrame);
}

function GenericEvalButtonBehavior::onMouseLeave(%this, %modifier, %worldPos)
{
   %this.owner.setImageMap(%this.startImage, %this.startFrame);
}
