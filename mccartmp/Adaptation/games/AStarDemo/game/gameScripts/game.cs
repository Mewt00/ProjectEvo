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
   Canvas.setContent(mainScreenGui);
   Canvas.setCursor(DefaultCursor);
   
   new ActionMap(moveMap);   
   moveMap.push();
   // when we clone an object, the underlying code places the object in the scene for us automatically.
   // it also will warn us that the object was already in a scene (part of the low level cloning process),
   // which can sometimes be an un-needed warning. Setting the preference below allows us to ignore these
   // warnings, but should be tested during development of complex multi-scene games to make sure we aren't 
   // accidentally moving objects from one scene to that exact same scene due to logic flaws.
   // For our demo, we don't need to worry about this, so we set the warning off by default:
   
   $pref::T2D::warnSceneOccupancy = false;   
   $enableDirectInput = true;
   activateDirectInput();
   enableJoystick();
   
   sceneWindow2D.loadLevel(%level);
   initializeMap("GridObjectTemplate");
   // start off our spawn cycle
   schedule(1000, 0, startSpawnCycle, 15000);
}

//---------------------------------------------------------------------------------------------
// endGame
// Game cleanup should be done here.
//---------------------------------------------------------------------------------------------
function endGame()
{
   sceneWindow2D.endLevel();
   moveMap.pop();
   moveMap.delete();
}
