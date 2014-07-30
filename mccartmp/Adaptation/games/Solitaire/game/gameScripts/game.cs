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
   
   exec("./card.cs");
   exec("./cardSprite.cs");
   exec("./cardStack.cs");
   exec("./mouseInput.cs");
   exec("./solitaire.cs");
   exec("./solitaireGUI.cs");
   exec("./solitaireCard.cs");
   exec("./solitaireStack.cs");
   exec("./sound.cs");
   
   Canvas.setContent(mainScreenGui);
   Canvas.setCursor(DefaultCursor);
   
   $guiActive = false;
   moveMap.bindCmd(keyboard, "escape", "SolitaireGui::toggleGui();", "");
   moveMap.bindCmd(keyboard, "tab", "SolitaireGui::toggleGui();", "");  // a backup keybind, since level editor overrides ESC
   moveMap.bindCmd(keyboard, "enter", "if($SolitaireGui::GuiActive) {Solitaire::pressDealButton(); SolitaireGui::removeGui();}", "");
   // add a debug hook to trigger the "you win" GUI -- make sure it's commented out for actual play!
   //moveMap.bindCmd(keyboard, "w", "$SolitaireGUI::DebugSignalWin=true;", "$SolitaireGUI::DebugSignalWin=false;");
   
   new ActionMap(moveMap);   
   moveMap.push();
   
   $enableDirectInput = true;
   activateDirectInput();
   enableJoystick();
   
   sceneWindow2D.loadLevel(%level);
   
   // Some bookkeeping to deal with people who go nuts with the GUI buttons.
   $SolitaireGUI::NumDeferredDeals = 0;
   $SolitaireGUI::DeferredDealEvent = 0;
   $SolitaireGUI::EnableInputEvent = 0;

   // Let the player play!   
   $SolitaireGUI::FirstTimeGui = true;
   SolitaireGui::popUpGui();
   
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
