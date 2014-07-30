//-----------------------------------------------------------------------------
// GarageGames Billiards
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// PopupText Class
//-----------------------------------------------------------------------------
//
// This class implements a simple text GUI element that flies onto the screen.
//
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// PopUpText - Example Use
//-----------------------------------------------------------------------------


//%popUpText = new t2dTextObject()
//{
   //scenegraph = %exScenegraph;
   //class = PopupText;
   //superClass = textObjectClass;
   //
   //layer = 5;
   ////typically offscreen
   //position = "-135 95";
   //
   ////delay from creation to movement start
   //delayTime = "1300";
   //
   //font = "Arial";
   //textAlign = "Left";
   //lineHeight = "16";
   //autoSize = "1";
   //fontSizes = "20";
   //textColor = "1 1 1";
   //hideOverlap = "0";
   //wordWrap = "0";
   //hideOverflow = "0";
   //aspectRatio = "1";
   //lineSpacing = "0";
   //characterSpacing = "0";  
//};



function PopupText::onAdd(%this)
{
   
   Parent::onAdd(%this);

   AnimationManager.playAnimation( %this, "flyToCenter:position", $ANIM_MODE_ABS, %this @ ".schedule(" @ %this.delayTime @ ", \"finish\");");
}

function PopupText::onRemove(%this)
{
   //Parent::onRemove(%this);
}

function PopupText::finish(%this)
{
   AnimationManager.playAnimation( %this, "fadeAway:alpha", $ANIM_MODE_ABS, %this @ ".onEnd();");   
}

function PopupText::onEnd(%this)
{
   eval(%this.command);
   %this.safeDelete();
}
