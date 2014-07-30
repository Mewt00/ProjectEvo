//-----------------------------------------------------------------------------
// GarageGames Billiards
// Copyright (C) GarageGames.com, Inc.
//
//-----------------------------------------------------------------------------
// CheckBox Script Class
//-----------------------------------------------------------------------------
//
// This class implements a checkbox style script class that may be used for GUI
// elements that are animatable in T2D.
//
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// T2D Gui Check Box - Example Use
//-----------------------------------------------------------------------------

%MyCheckBox = new t2dSceneObject() 
 {
      scenegraph              = %mySceneGraph;
      class                   = CheckBox;
      position = "0 0";

      image                    = myCheckBoxImageMap;
      imageSize                = "5 5";
      checkedIndex             = 0;
      uncheckedIndex           = 1;
      imageMountPos            = "0 0";

      //offset for text (textAlign will make sure the offset is used correctly)
      // "Left" will offset the text's left to the fontOffset
      // "Right" will offset the text's right to the fontOffset
      // so, you can have different strings and they'll line up with the same offset
      fontOffset            = "0.6 0";
      fontName          = "Comic Sans MS";
      fontColor         = "1 1 1 1";
      fontSize          = "3";
      text              = "Checkbox!";
      textAlign         = "Left";
};


//-----------------------------------------------------------------------------
// Action Methods
//-----------------------------------------------------------------------------
function CheckBox::setChecked( %this, %isChecked )
{
   // Update View
   if( %isChecked ) 
      %this.checkSprite.setImageMap( %this.image, %this.checkedIndex );
   else 
      %this.checkSprite.setImageMap( %this.image, %this.uncheckedIndex );

   // Update State
   %this.toggled = %isChecked;

   // User Callback
   %this.OnCheck( %isChecked );
}

function CheckBox::getChecked( %this )
{
   return %this.toggled;
}

//-----------------------------------------------------------------------------
// State Callbacks
//-----------------------------------------------------------------------------
function CheckBox::OnCheck( %this, %isChecked )
{
   //note that in your checkbox command you can use %isChecked
   // to know what state the box is in
   if( %this.command !$= "")
      eval(%this.command);
}

//-----------------------------------------------------------------------------
// Construction/Destruction
//-----------------------------------------------------------------------------
function CheckBox::onAdd(%this)
{
   %scenegraph = %this.getSceneGraph();

   // Create the Text
   %this.textObj = new t2dTextObject() 
   { 
      scenegraph     = %this.getSceneGraph();
      
      textAlign = %this.textAlign;
      
      text = %this.text;
      font = %this.fontName;
      
      lineHeight = %this.fontSize;
      autoSize = "1";
      fontSizes = "20";
      textColor = %this.fontColor;
      hideOverlap = "0";
      wordWrap = "0";
      
      
      position = "0 0";
      hideOverflow = "0";
      aspectRatio = "1";
      lineSpacing = "0";
      characterSpacing = "0";
   };
   
   %totalOffset = %this.calculateOffset( %this.fontOffset);
   %this.textObj.mount( %this, %totalOffset );
   %this.addAutoFontSize();
   

   // Create the check box image
   %this.checkSprite = new t2dStaticSprite() { scenegraph  = %this.getSceneGraph(); class = "CheckMouseEvents"; };
   %this.checkSprite.setSize( %this.imageSize );
   %this.checkSprite.setImageMap( %this.image, 0 );
   %this.checkSprite.mount( %this, %this.imageMountPos );
   %this.checkSprite.checkObj = %this; 
   
   // Allow our check sprite to receive mouse events
   %doInput = !%this.suppressInput;
   if( %this.useSizeCheck )
      %this.setUseMouseEvents(%doInput);
   %this.checkSprite.setUseMouseEvents(%doInput);

   // Setup initial state
   if( %this.toggled ) 
      %this.checkSprite.setImageMap( %this.image, %this.checkedIndex );
   else 
      %this.checkSprite.setImageMap( %this.image, %this.uncheckedIndex );

   %this.enabled = true;
}

function CheckBox::onRemove(%this)
{
   if( isObject( %this.textObj ) )
      %this.textObj.safeDelete();

   if( isObject( %this.checkSprite ) )
      %this.checkSprite.safeDelete();
}

function CheckBox::setText( %this, %text )
{
   %this.textObj.text = %text;
}

function CheckBox::setEnabled( %this, %enabled )
{
   if( ! %enabled )
   {
      %this.enabled = false;
      %this.textObj.setBlendColor( "0.4 0.4 0.4 0.9" );
   }
   else
   {
      %this.enabled = true;
      %this.textObj.setBlendColor( %this.fontcolor );
   }
}

function CheckBox::setLayer( %this, %layer )
{
   %this.textObj.setLayer( %layer );
   %this.checkSprite.setLayer( %layer );
}

function CheckBox::getBlendColor( %this )
{
   return %this.checkSprite.getBlendColor();
}

function CheckBox::setBlendColor( %this, %color )
{
   %this.checkSprite.setBlendColor( %color );
}

function CheckBox::addAutoFontSize(%this)
{
   %camArea = sceneWindow2D.getCurrentCameraArea();
   %camHeight = mAbs( getWord( %camArea, 1) - getWord( %camArea, 3) );
   %pixelHeight = getWord( sceneWindow2D.getWindowExtents(), 3);
   %this.textObj.addAutoFontSize( %pixelHeight , %camHeight, %this.fontSize);
}

//Is used to align the text to the %this.textAlign parameter so that the fontOffset is more useful
function CheckBox::calculateOffset( %this, %baseFontOffset){
   
   switch$ (%this.textAlign)
   {
      case "Center":
         return %baseFontOffset;
      case "Left":
         return t2dVectorAdd( %baseFontOffset, %this.textObj.getWidth()/10 SPC "0"); //TYETODO: I have no idea why /10 works. It should be /2, right?
      case "Right":
         return t2dVectorSub( %baseFontOffset, %this.textObj.getWidth()/10 SPC "0");
   }
   return %baseFontOffset;
}
//-----------------------------------------------------------------------------
// Mouse Events
//-----------------------------------------------------------------------------
function CheckBox::onMouseUp(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( %this.useSizeCheck )
   {
      if( !%this.enabled )
         return;
         
      // Toggle our state
      %this.setChecked( !%this.toggled );
   }
   else
      %this.checkSprite.onMouseUp(%modifier, %worldPosition, %mouseClicks);
}

function CheckMouseEvents::onMouseUp(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( !%this.checkObj.enabled )
      return;
      
   // Toggle our state
   %this.checkObj.setChecked( !%this.checkObj.toggled );
}