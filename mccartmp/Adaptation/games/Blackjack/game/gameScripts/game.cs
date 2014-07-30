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
   exec("./GUI_Manager/GUI_Main.cs");
   initializeGUISystem();
   
   exec("./card.cs");
   exec("./cardStack.cs");
   exec("./cardGraphics.cs");
   exec("./fireworks.cs");
   
   exec("./blackjack.cs");   
   
   Canvas.setContent(mainScreenGui);
   Canvas.setCursor(DefaultCursor);
   
   new ActionMap(moveMap);   
   moveMap.push();
   
   $enableDirectInput = true;
   activateDirectInput();
   enableJoystick();
   
   %scenegraph = sceneWindow2D.loadLevel(%level);
   
   exec("./blackjackGUI.cs");
   initMenu(%scenegraph);

   SceneWindow2D.guiActive = true;

   for (%i=0; %i<4; %i++)
   {
      %fullDeck = new t2dSceneObject() {class="cardStack";};
      %fullDeck.initializeFullDeck();
      %fullDeck.transferTo(shoe, 0, RESTOFSTACK);
      %fullDeck.safeDelete();
   }
   //shoe.initializeFullDeck();
   associateCardsWithImages(shoe, %scenegraph);
   shoe.shuffle();

   %scenegraph.tableControl = new ScriptObject(blackjackFSM_Singleton)
   {
      class = "blackjackFSM";
   };
   
   %scenegraph.tableControl.curState = "PLACEBETS";
   
   betMarker.enable = true;
   betSlider.enable = true;
   betSlider.command = "betMarker.mount(betSlider, getWord(betSlider.getLocalPoint(%worldPosition), 0), 0);";
   
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
