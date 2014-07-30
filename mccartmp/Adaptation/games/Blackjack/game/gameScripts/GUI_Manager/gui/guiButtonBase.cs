//-----------------------------------------------------------------------------
// GarageGames Billiards
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

function ButtonBase::onAdd(%this)
{
   %scenegraph = %this.getSceneGraph();
 
   // the 'button' is in group 1, layer 10
   // everything else in group 2, layer 10
   %this.setGraphGroup(1);
   %this.setLayer(%this.layer $= "" ? 10 : %this.layer);
 
   // create the base of the button
   %this.base = new t2dStaticSprite() { scenegraph = %scenegraph; };
   %this.base.setGraphGroup(2);
   %this.base.setLayer(%this.layer $= "" ? 10 : %this.layer);
   
   // create the overlay for the button
   %this.overlay = new t2dStaticSprite() { scenegraph = %scenegraph; };
   %this.overlay.setGraphGroup(2);
   %this.overlay.setLayer(%this.layer $= "" ? 10 : %this.layer);
   
   // create the button text
   %this.textObj = new t2dTextObject() { 
      scenegraph = %scenegraph;
      class = ButtonText;
      
      //text = %this.getText();
      font = %this.getFont();
      textAlign = "Center";
      lineHeight = %this.getFontSize();
      autoSize = "1";
      fontSizes = "1";
      textColor = %this.fontColor;
      hideOverlap = "0";
      wordWrap = "0";
      
   };
   %this.textObj.button = %this;
   %this.textObj.setGraphGroup(2);
   %this.textObj.setLayer(%this.layer $= "" ? 10 : %this.layer);
   
   // mount the button pieces
   %this.base.mount(%this);                
   %this.overlay.mount(%this);
   %this.textObj.mount(%this, %this.getFontOffset());
   
   // set the size
   %this.adjustSize(%this.grabSize());
   
   // set the position
   %this.setPosition(%this.grabPosition());
   
   // set the images
   if(%this.baseImage !$= "" || %this.overlayImage !$= "")
      %this.setImage(%this.baseImage, %this.overlayImage);
   
   
   // setup the text
   %this.setFontSize( %this.getFontSize() );
   %this.setText(%this.getText());
   
   // allow to receive mouse events
   %this.setUseMouseEvents(true);
   %this.textObj.setUseMouseEvents(true);
   
   
   
}

function ButtonBase::onRemove(%this)
{
   if(isObject(%this.textObj))
      %this.textObj.safeDelete();
   
   if(isObject(%this.overlay))
      %this.overlay.safeDelete();
   
   if(isObject(%this.base))
      %this.base.safeDelete();
}


function ButtonBase::setBlendColor( %this, %color )
{
   %this.base.setBlendColor( %color );
   %this.textObj.setBlendColor( %color );
   %this.overlay.setBlendColor( %color );
   Parent::setBlendColor( %this, %color );   
}

//-----------------------------------------------------------------------------


function ButtonBase::onMouseUp(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( %this.disabled != true )
      eval(%this.command);
}

function ButtonBase::onMouseDown(%this, %modifier, %worldPosition, %mouseClicks)
{
}

function ButtonBase::onMouseMove(%this, %modifier, %worldPosition, %mouseClicks)
{
}


//-----------------------------------------------------------------------------
// The below mouse events are grabbed by the text associated with a button
// and subsequently passed on to it's parent (the button) - Justin`
//-----------------------------------------------------------------------------
function ButtonText::onMouseUp(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( !isObject(%this.button.baseImage) && !isObject(%this.button.overlayImage) )
      %this.button.onMouseUp(%modifier, %worldPosition, %mouseClicks);
}

function ButtonText::onMouseDown(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( !isObject(%this.button.baseImage) && !isObject(%this.button.overlayImage) )
      %this.button.onMouseDown(%modifier, %worldPosition, %mouseClicks);
}

function ButtonText::onMouseMove(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( !isObject(%this.button.baseImage) && !isObject(%this.button.overlayImage) )
      %this.button.onMouseMove(%modifier, %worldPosition, %mouseClicks);
}

function ButtonText::onMouseEnter(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( !isObject(%this.button.baseImage) && !isObject(%this.button.overlayImage) )
      %this.button.onMouseEnter(%modifier, %worldPosition, %mouseClicks);
}

function ButtonText::onMouseLeave(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( !isObject(%this.button.baseImage) && !isObject(%this.button.overlayImage) )
      %this.button.onMouseLeave(%modifier, %worldPosition, %mouseClicks);
}

//-----------------------------------------------------------------------------


function ButtonBase::setImage(%this, %baseImage, %overlayImage)
{
   // -- sets the base and overlay button image
   // ----
   
   %this.baseImage = %baseImage;
   %this.overlayImage = %overlayImage;
   
   
   
   if(%this.baseImage !$= "")
   {
      %this.base.setImageMap( %this.baseImage , (%this.disabled) ? %this.disabledIndex : 0 );
      %this.base.setVisible(true);
   }
   else
      %this.base.setVisible(false);
   
   if(%this.overlayImage !$= "")
   {
      %this.overlay.setImageMap( %this.overlayImage,  ( ( %this.imageindex !$= "" ) ? %this.imageIndex : 0 ));
      %this.overlay.setVisible(true);
   }
   else
      %this.overlay.setVisible(false);
}


//-----------------------------------------------------------------------------

// Sets this button to use it's disabled index or not
function ButtonBase::setEnabled( %this, %value )
{
   
   //the commentted out parts are for the case when you want to
   // change the imageMap's frame instead of fading the object a bit
   
   %this.disabled = !%value;
   //if( %this.baseImage !$= "" )
   //   %this.base.setImageMap( %this.baseImage, (%this.disabled) ? %this.disabledIndex : 0 );
   //else
   //{
      if( %this.disabled )
         AnimationManager.playAnimationFrame(%this,  "200:0.2", "alpha", $ANIM_MODE_ABS, "" );
      else
         AnimationManager.playAnimationFrame(%this,  "100:1", "alpha", $ANIM_MODE_ABS, "" );
   //}
}

function ButtonBase::getEnabled( %this )
{
   return !%this.disabled;
}

function ButtonBase::setText(%this, %buttonText)
{
   // -- sets the button text
   // ----
   
   
   %this.text = %buttonText;
   
   %this.textObj.text = %buttonText;
   
   
}

function ButtonBase::getText(%this)
{
   // -- return the button text
   // ----
   
   %text = "";
   if(%this.text !$= "")
      %text = %this.text;
  
   return %text;
}

function ButtonBase::setFontOffset(%this, %fontOffset)
{
   // -- sets the font offset from base
   // ----
   
   %this.fontOffset = %fontOffset;
   %this.textObj.mount(%this, %fontOffset);
}

function ButtonBase::getFontOffset(%this)
{
   // -- return the font offset from base
   // ----
      
   %offset = "0 0";
   if(%this.fontOffset !$= "")
      %offset = %this.fontOffset;
   
   return %offset;
}

function ButtonBase::setFontSize(%this, %fontSize)
{
   // -- sets the font scale for the text
   // ----
   
   // scale is in relation to the rest of the size
   %this.fontSize = %fontSize;
   %this.textObj.lineHeight = %fontSize;
   
   //auto-generates fontSize based on window height (px), camera height (world units), and desired font height
   %camArea = sceneWindow2D.getCurrentCameraArea();
   %camHeight = mAbs( getWord( %camArea, 1) - getWord( %camArea, 3) ) + 1;
   %pixelHeight = getWord( sceneWindow2D.getWindowExtents(), 3);
   
   %this.textObj.removeAllFontSizes();
   
   %this.textObj.addAutoFontSize( %pixelHeight , %camHeight, %fontSize);
   
}

function ButtonBase::getFontSize(%this)
{
   // -- return the font scale
   // ----
   
   %size = 1;
   if(%this.fontSize !$= "")
      %size = %this.fontSize;
   
   return %size;
}

function ButtonBase::setFont(%this, %fontName)
{
   // -- sets the font style for the text
   // ----
   
   %this.fontName = %fontName;
   %this.textObj.font = %fontName;
}

function ButtonBase::getFont(%this)
{
   // -- return the font style
   // ----
   
   %fontName = $Font::Default;
   if(%this.fontName !$= "")
      %fontName = %this.fontName;
   
   return %fontName;
}

function ButtonBase::setFontColor(%this, %fontColor)
{
   // -- sets the font color for the text
   // ----
   
   %this.fontColor = %fontColor;
   %this.textObj.setBlendColor(%fontColor);
}

function ButtonBase::getFontColor(%this)
{
   // -- return the font color
   // ----
   
   %fontColor = "1 1 1 1";
   if(%this.fontColor !$= "")
      %fontColor = %this.fontColor;
   
   return %fontColor;
}


//-----------------------------------------------------------------------------


function ButtonBase::setSizeToText(%this, %isOn)
{
   // -- set size of button to be width/height of text
   // ----
   
   // this is sort of a hack, we'll basically switch the 
   // groups around so that the text field becomes the
   // clickable region
   
   if(%isOn)
   {
      %this.setGraphGroup(2);
      %this.textObj.setGraphGroup(1);
   }
   else
   {
      %this.setGraphGroup(1);
      %this.textObj.setGraphGroup(2);
   }
}

function ButtonBase::adjustSize(%this, %size)
{
   // -- sets the button size
   // ----
   
   // NOTE - use this instead of setSize
   
   %this.size = %size;
   %this.setSize(%size);
   
   // this is here so our changes will propogate to all of the sub-objects
   // text uses its own sizing
   %this.base.setSize(%size);
   %this.overlay.setSize(%size);
}

function ButtonBase::grabSize(%this)
{
   // -- return the button size
   // ----
   
   // NOTE - use this isntead of getSize
   
   %size = 1;
   if(%this.size !$= "")
      %size = %this.size;
   
   return %size;
}

function ButtonBase::adjustPosition(%this, %position)
{
   // -- sets the button position
   // ----
   
   // NOTE - use this instead of setPosition
   
   %this.position = %position;
   %this.setPosition(%position);
}

function ButtonBase::grabPosition(%this)
{
   // -- return the position of button
   // ----
   
   // NOTE - use this instead of getPosition
   
   %position = %this.getPosition();
   if(%this.position !$= "")
      %position = %this.position;
   
   return %position;
}
