//---------------------------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//---------------------------------------------------------------------------------------------

//---------------------------------------------------------------------------------------------
// startGame
// All game logic should be set up here. This will be called by the level builder when you
// select "Run Game" or by the startup process of your game to load the first level.
//---------------------------------------------------------------------------------------------
function startGame(%level)
{
   //exec'ing is important
   exec("./piece.cs");
   exec("./board.cs");
   exec("./audioDatablocks.cs");
   exec("./GenericImageButton.cs");
   
   
   Canvas.setContent(mainScreenGui);
   Canvas.setCursor(DefaultCursor);
   
   new ActionMap(moveMap);   
   moveMap.push();
   
   $enableDirectInput = true;
   activateDirectInput();
   enableJoystick();
   
   sceneWindow2D.loadLevel(%level);
   
   setup();
   
   //sceneWindow2D.setUseObjectMouseEvents(true);
}


/// ()
/// makes sure objects will get mouse events
/// 
function setup()
{
   //ready the audio!
   audioSetup();
   
   // make sure the scene window is set to send mouse events to objects
   if(!sceneWindow2D.getUseObjectMouseEvents()) {
      sceneWindow2D.setUseObjectMouseEvents(true);
   }  
}


//---------------------------------------------------------------------------------------------
// endGame
// Game cleanup should be done here.
//---------------------------------------------------------------------------------------------
function endGame()
{
   sceneWindow2D.stopCameraShake();
   sceneWindow2D.endLevel();
   moveMap.pop();
   moveMap.delete();
}
