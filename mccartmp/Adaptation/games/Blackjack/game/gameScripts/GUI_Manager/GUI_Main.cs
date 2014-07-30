//-----------------------------------------------------------------------------
// GarageGames Billiards
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//some defaults
$Font::Default = "Arial";
$Font::Small = "Times New Roman";

//-----------------------------------------------------------------------------

//put this function in your startGame (somewhere before you start using animated buttons, obviously)
function initializeGUISystem()
{
   loadGUIObjects();
   createManagers();
}

//put this function in your endGame
function destroyGUISystem()
{
   destroyManagers();
}

//add your custom gui objects to this exec block
function loadGUIObjects()
{
   exec("./gui/guiDatablocks.cs");
   exec("./gui/guiButtonBase.cs");
   exec("./gui/guiAnimatedButton.cs");
   exec("./gui/guiSpriteButton.cs");
   exec("./gui/guiCheckBox.cs");
   exec("./gui/guiSlider.cs");
   exec("./gui/guiSelectionList.cs");
   exec("./gui/guiSelectionListText.cs");
   exec("./gui/guiTextPopup.cs");

   exec("./animations/animations.cs");
   
   //makes sure the GUI objects can get mouse events
   if(!sceneWindow2D.getUseObjectMouseEvents())
      sceneWindow2D.setUseObjectMouseEvents(true);
   
   //makes a scenegraph that will update the animation system every frame
   //if( !isObject($AnimationSceneGraph))
   //$AnimationSceneGraph   = new t2dSceneGraph( ) { class = "AnimationSceneGraph"; doUpdateWhenPaused = true; };
}

function AnimationSceneGraph::onUpdateScene(%this)
{
   error("Animationscenegraph updating!");
   if(isObject(AnimationManager))
   {
      AnimationManager.update( %elapsedTime );
   }
}

//-----------------------------------------------------------------------------



function createManagers()
{     
   // create the animation manager
   if(isObject(AnimationManager)) 
      AnimationManager.delete();
   new ScriptObject(AnimationManager) {};
}

function destroyManagers()
{      
   
   // destroy the animation manager
   if(isObject(AnimationManager))
      AnimationManager.delete();

}
