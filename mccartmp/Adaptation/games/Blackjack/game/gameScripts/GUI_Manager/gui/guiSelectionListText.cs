//-----------------------------------------------------------------------------
// GarageGames Billiards
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------


//-----------------------------------------------------------------------------
// T2D Gui Selection List Class - Example Use
//-----------------------------------------------------------------------------
//
//%exampleSelectionList = new t2dSceneObject() 
//{
   //scenegraph              = %exampleSceneGraph;
   //class                   = SelectionListText;
   //superClass              = SelectionList;
   //position                = "0 0";
   //
   ////command is called every time the shown object changes - the command can use %newValue or %newText
   //command = "echo(\"Example Selection List changed to: \"%newValue);";
   //
   ////-----------------------------------------------------------------------------
   //// Next/Previous Button Appearance ( Shared Attributes )
   ////-----------------------------------------------------------------------------
//
   //// The size of the prev/next buttons
   //buttonSize              = "10 10";
   //// The base image (background) to be used on the Next/Prev item buttons
   //buttonBaseImage         = myMenuButtonImage;
   //// The font to be used on prev/next buttons (if any text is associated with them)
   //buttonFontName          = "Arial";
   //// The font color to be used on prev/next buttons (if any text is associated with them)
   //buttonFontColor         = "1 1 1 1";
   //// The font mount position to be used on prev/next buttons (if any text is associated with them)
   //buttonFontOffset        = "0 0";
   //// The font size to be used on prev/next buttons (if any text is associated with them)
   //buttonFontSize          = "4";
   //
   ////-----------------------------------------------------------------------------
   //// Previous Button Appearance 
   ////-----------------------------------------------------------------------------
//
   //// The text to be placed on the 'previous item' button.  ("" means no text)
   //prevButtonText          = ""; 
   //// The overlay image to be placed on top of the buttonBaseImage
   //// ( << graphic or the like - generally with no button text )
   //prevButtonOverlayImage  = exLeftArrowImageMap;
   //// Position on the base to mount the previous button ( "-0.9 0" Default )
   //prevButtonMountPos      = "-2 0";
   //// The cell of the imagemap to use (defaults to 0)
   //prevButtonImageIndex    = 0;
//
   ////-----------------------------------------------------------------------------
   //// Next Button Appearance
   ////-----------------------------------------------------------------------------
//
   //// The text to be placed on the 'next item' button.  ("" means no text)
   //nextButtonText          = ""; 
   //// The overlay image to be placed on top of the buttonBaseImage
   //// ( >> graphic or the like - generally with no button text )
   //nextButtonOverlayImage  = exRightarrowImageMap;
   //// Position on the base to mount the previous button ( "0.9 0" Default )
   //nextButtonMountPos      = "2 0";
   //// The cell of the imagemap to use (defaults to 0)
   //nextButtonImageIndex    = 0;
   //
   ////-----------------------------------------------------------------------------
   //// Text Display Attributes
   ////-----------------------------------------------------------------------------
   //
   //// Position to mount the text display on the base ( "0 0" Default )
   //textMountPos            = "0 0";
   //// The font to be used on item text
   //textFontName          = "Arial";
   //// The font color to be used on item text
   //textFontColor         = "1 1 1 1";
   //// The font mount position to be used on item text
   //textFontOffset        = "0 0";
   //// The font size to be used on item text
   //textFontSize          = "8";
//};
   //
//%exampleSelectionList.add("A Choice", 1);
   


function SelectionListText::onAdd(%this)
{
   // Parent first.
   Parent::onAdd(%this);

   // Create the Item Text Display
   %this.textObj = new t2dTextObject() {
      scenegraph = %this.getSceneGraph();
      class = ButtonText;
      
      text = "empty";
      font = %this.textFontName;
      textAlign = "Center";
      lineHeight = %this.textFontSize;
      autoSize = "1";
      fontSizes = "10";
      textColor = %this.textFontColor;
      hideOverlap = "0";
   };
   // Layer/Group/Mount Item Text
   %this.textObj.setLayer(%this.layer); 
   %this.textObj.mount(%this, %this.textMountPos);
   
   %this.addAutoFontSize(%this);
}

function SelectionListText::onRemove(%this)
{
   // Remove the Text
   if(isObject(%this.textObj))
      %this.textObj.safeDelete();

   // Parent Last!
   Parent::onRemove( %this );
}

function SelectionListText::setBlendColor( %this, %color )
{
   %this.textObj.setBlendColor( %color );
   Parent::setBlendColor( %this, %color );
}

function SelectionListText::addAutoFontSize(%this)
{
   %camArea = sceneWindow2D.getCurrentCameraArea();
   %camHeight = mAbs( getWord( %camArea, 1) - getWord( %camArea, 3) );
   %pixelHeight = getWord( sceneWindow2D.getWindowExtents(), 3);
   %this.textObj.addAutoFontSize( %pixelHeight , %camHeight, %this.textFontSize);
}

//-----------------------------------------------------------------------------
// Action Methods
//-----------------------------------------------------------------------------

function SelectionListText::UpdateDisplay( %this )
{
   // Parent First! (Does Sanity Checking)
   Parent::UpdateDisplay( %this );

   // This code can be terribly confusing to look at so I'll just explain what it does.
   // First we fade the text to 0 alpha then set the new text, then fade it back in. Simple, ehh?
   %alphaFade = %this.textObj @ ".text =\"" @ %this.currentEntry.text @ "\"; AnimationManager.playAnimationFrame(" @ %this.textObj;
   %alphaFade = %alphaFade @ ",\"100:1:0:100\", \"alpha\"," @  $ANIM_MODE_ABS @ ");";

   // Do it!
   AnimationManager.stopAnimations( %this.textObj );
   AnimationManager.playAnimationFrame( %this.textObj, "100:0", "alpha", $ANIM_MODE_ABS, %alphaFade );
}