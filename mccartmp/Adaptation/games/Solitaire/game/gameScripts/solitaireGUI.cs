//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

/// Functions that implement the "buttons" and such that make up the GUI

// If the buttons aren't shown, show them.  If they are, get rid of them.
function SolitaireGui::toggleGui()
{
   if ($SolitaireGUI::GuiActive)
   {
      SolitaireGui::removeGui();
   }
   else
   {
      SolitaireGui::popUpGui();
   }
}

// Bring the buttons into existence.
function SolitaireGui::popUpGui()
{
   // some bookkeeping.
   if ($SolitaireGUI::ScheduledPopUpEvent != 0)
   {
      cancel($SolitaireGUI::ScheduledPopUpEvent);
      $SolitaireGUI::ScheduledPopUpEvent = 0;
   }

   if (mouse.getNumCards() > 0)
   {
      // can't hold onto cards and use GUI at the same time!
      mouse.transferTo(mouse.PickedUpFrom, 0, RESTOFSTACK);
   }

   // create the GUI elements.
   if (!isObject($SolitaireGUI::QuitButton)) 
   {
      $SolitaireGUI::QuitButton = new t2dStaticSprite()
      {
         class = "button";
         scenegraph = sceneWindow2D.getSceneGraph();
         imageMap = quitButtonImageMap;
         frame = 0;
         size = 50 SPC 38.444;
         position = -27 SPC 10;
         blendColor = 1 SPC 1 SPC 1 SPC 0;
         layer = $CardSprite::ButtonLayer;
         command = "endGame(); quit();";
      };
      if ($runWithEditors)
      {
         $SolitaireGUI::QuitButton.command = "endGame(); toggleLevelEditor();";
      }
   }
      
   if (!isObject($SolitaireGUI::DealButton)) 
   {
      $SolitaireGUI::DealButton = new t2dStaticSprite()
      {
         class = "button";
         scenegraph = sceneWindow2D.getSceneGraph();
         imageMap = dealButtonImageMap;
         frame = 0;
         size = 50 SPC 38.444;
         position = 27 SPC 10;
         blendColor = 1 SPC 1 SPC 1 SPC 0;
         layer = $CardSprite::ButtonLayer;
         command = "Solitaire::pressDealButton();";
      };
   }
   
   if (!isObject($SolitaireGUI::BackButton))
   {
      $SolitaireGUI::BackButton = new t2dStaticSprite()
      {
         class = "button";
         scenegraph = sceneWindow2D.getSceneGraph();
         imageMap = backImageMap;
         size = 11.683 SPC 6.975;
         position = 0 SPC 71.350;
         layer = $CardSprite::ButtonLayer;
         command = "";
      };
   }
   
   if (!isObject($SolitaireGUI::SplashTitle))
   {
      $SolitaireGUI::SplashTitle = new t2dStaticSprite()
      {
         scenegraph = sceneWindow2D.getSceneGraph();
         imageMap = SolitaireSplashImageMap;
         size = 100 SPC 37.391;
         position = 0 SPC -26;
         blendColor = 1 SPC 1 SPC 1 SPC 0;
         layer = $CardSprite::ButtonLayer+1;
      };
   }

   // To provide some better contrast for when the GUI buttons are displayed
   // while there are cards out on the table, put some see-through darkness
   // over top of the entire table underneath the buttons.
   if (!isObject($SolitaireGUI::Blackness))
   {
      $SolitaireGUI::Blackness = new t2dStaticSprite()
      {
         scenegraph = sceneWindow2D.getSceneGraph();
         imageMap = blacknessImageMap;
         size = 200 SPC 150;
         position = 0 SPC 0;
         blendColor = 1 SPC 1 SPC 1 SPC 0;
         layer = $CardSprite::ButtonLayer+2;
      };
   }
   
   // Determine whether to do the "normal" GUI popup, or the version that also signals victory
   if ((!isObject($SolitaireGUI::You)) && (!$SolitaireGUI::VictorySignalled) && (Solitaire::checkForWin()))
   {
      // we need to tell the player that he's won!

      // "you" and "win" are "cards" that are "dealt" onto the screen.
      if (isObject($SolitaireGUI::You)) {$SolitaireGUI::You.delete();}
      $SolitaireGUI::You = new t2dStaticSprite()
      {
         scenegraph = sceneWindow2D.getSceneGraph();
         imageMap = victoryImageMap;
         frame = 0;
         size = 50 SPC 38.44;
         position = 0 SPC 120;
         layer = $CardSprite::ButtonLayer+1;
      };
   
      if (isObject($SolitaireGUI::Win)) {$SolitaireGUI::Win.delete();}
      $SolitaireGUI::Win = new t2dStaticSprite()
      {
         scenegraph = sceneWindow2D.getSceneGraph();
         imageMap = victoryImageMap;
         frame = 1;
         size = 50 SPC 38.44;
         position = 0 SPC 120;
         layer = $CardSprite::ButtonLayer+1;
      };


      // The normal buttons start off the screen, and then get "dealt" onto it, too.
      $SolitaireGUI::QuitButton.setBlendColor(1, 1, 1, 1);
      $SolitaireGUI::QuitButton.setPosition(0, 120);
      $SolitaireGUI::DealButton.setBlendColor(1, 1, 1, 1);
      $SolitaireGUI::DealButton.setPosition(0, 120);
      
      // Since we want these things to move like the cards do, we use similar code.
      %distance = t2dVectorDistance($SolitaireGui::You.getPosition(), -27 SPC 10);
   
      $SolitaireGUI::You.moveTo(-27, 10, %distance/0.1);
      alxPlay(whoosh);
      $SolitaireGUI::Win.schedule(100, moveTo, 27, 10, %distance/0.1);
      schedule(100, 0, alxPlay, whoosh);
      $SolitaireGUI::QuitButton.schedule(1000, moveTo, -27, 10, %distance/0.1);
      schedule(1000, 0, alxPlay, whoosh);
      $SolitaireGUI::DealButton.schedule(1100, moveTo, 27, 10, %distance/0.1);
      schedule(1100, 0, alxPlay, whoosh);
      $SolitaireGUI::BackButton.schedule(1200, fadeIn, 25, 0.1);
      $SolitaireGUI::SplashTitle.schedule(1200, fadeIn, 25, 0.1);
      $SolitaireGUI::Blackness.schedule(1200, fadeIn, 25, 0.1);

      $SolitaireGUI::VictorySignalled = true;      
   }
   else
   {
      // Normal GUI popup.
      $SolitaireGUI::QuitButton.setPosition(-27, 10);
      $SolitaireGUI::DealButton.setPosition(27, 10);
      $SolitaireGUI::BackButton.fadeIn(25, 0.1);
      $SolitaireGUI::SplashTitle.fadeIn(25, 0.1);
      $SolitaireGUI::QuitButton.fadeIn(25, 0.1);
      $SolitaireGUI::DealButton.fadeIn(25, 0.1);

      if ($SolitaireGUI::FirstTimeGui)
      {
         // we don't want the blackness on the first startup.
         $SolitaireGUI::FirstTimeGui = false;
      }
      else
      {
         $SolitaireGUI::Blackness.fadeIn(25, 0.1);
      }
   }

   if (isObject($SolitaireGUI::MenuButton))
   {
      $SolitaireGUI::MenuButton.fadeOut(25, 0.1);
   }

   $SolitaireGUI::GuiActive = true;
   SolitaireGui::checkGuiButtons(sceneWindow2D.getMousePosition());   
}

// Get rid of the buttons.
function SolitaireGui::removeGui()
{
   if (isObject($SolitaireGUI::You)) 
   {
      $SolitaireGUI::You.fadeOut(25, 0.1);
   }
   if (isObject($SolitaireGUI::Win))
   {
      $SolitaireGUI::Win.fadeOut(25, 0.1);
   }

   if (!isObject($SolitaireGUI::MenuButton))
   {
      $SolitaireGUI::MenuButton = new t2dStaticSprite()
      {
         class = "button";
         scenegraph = sceneWindow2D.getSceneGraph();
         imageMap = menuImageMap;
         size = 11.683 SPC 6.975;
         position = 0 SPC 71.350;
         layer = $CardSprite::ButtonLayer;
         command = "SolitaireGui::toggleGui();";
      };
   }
   $SolitaireGUI::MenuButton.fadeIn(25, 0.1);

   $SolitaireGUI::SplashTitle.fadeOut(25, 0.1);
   $SolitaireGUI::Blackness.fadeOut(25, 0.1);
   $SolitaireGUI::QuitButton.fadeOut(25, 0.1);
   $SolitaireGUI::DealButton.fadeOut(25, 0.1);
   $SolitaireGUI::BackButton.fadeOut(25, 0.1);

   $SolitaireGUI::GuiActive = false;
}

// See if the mouse is over a button.
function SolitaireGui::checkGuiButtons(%worldPosition)
{
   if (isObject($SolitaireGUI::QuitButton)) {$SolitaireGUI::QuitButton.setFrame(0);}
   if (isObject($SolitaireGUI::DealButton)) {$SolitaireGUI::DealButton.setFrame(0);}
   if (isObject($SolitaireGUI::MenuButton)) {$SolitaireGUI::MenuButton.setFrame(0);}
   if (isObject($SolitaireGUI::BackButton)) {$SolitaireGUI::BackButton.setFrame(0);}
   
   %buttons = sceneWindow2D.getSceneGraph().pickPoint(%worldPosition, 1, BIT($CardSprite::ButtonLayer));
   for (%i=0; %i<getWordCount(%buttons); %i++)
   {
      %object = getWord(%buttons, %i);
      if (%object.class !$= "button")
      {
         error("Button finder found something besides a button!" SPC %object);
      }
      if (mouse.buttonDown)
      {
         %object.setFrame(2);
      }
      else
      {
         %object.setFrame(1);
      }
   }
}

// One of the buttons needs to do its thing.
function SolitaireGui::activateGuiButton(%worldPosition)
{
   %numButtonsActivated = 0;
   %buttons = sceneWindow2D.getSceneGraph().pickPoint(%worldPosition, 1, BIT($CardSprite::buttonLayer));
   for (%i=0; %i<getWordCount(%buttons); %i++)
   {
      %object = getWord(%buttons, %i);
      if (%object.class !$= "button")
      {
         error("Button finder found something besides a button!" SPC %object);
      }
      if ($SolitaireGUI::GuiActive)
      {
         SolitaireGui::removeGui();
      }
      if (%object.command !$= "")
      {
         eval(%object.command);
      }
      %numButtonsActivated++;
   }
   return %numButtonsActivated;
}

// Let user input get to the functions that process it.
function SolitaireGui::enableGameInput()
{
   $SolitaireGUI::AllowGameInput = true;
}



/// (SimID this, integer time, float rate)
/// Fades a t2dSceneObject out by periodically updating its Alpha value
///
/// @param this The t2dSceneObject object
/// @param time The number of milliseconds between updates
/// @param rate The amount of alpha to change on each step.
///
function t2dSceneObject::fadeOut(%this, %time, %rate)
{   
   if (%this.fadeEvent != 0)
   {
      cancel(%this.fadeEvent);
      %this.fadeEvent = 0;
   }

   // Fetch the current alpha value
   %alpha = %this.getBlendAlpha();

   // Decrease it
   %alpha -= %rate; 

   if(%alpha < 0)
   {
      %this.safeDelete();
   }
   else
   {
      %this.setBlendAlpha( %alpha );
      %this.fadeEvent = %this.schedule(%time, fadeOut, %time, %rate);
   }
}

/// (SimID this, integer time, float rate)
/// Fades a t2dSceneObject out by periodically updating its Alpha value
///
/// @param this The t2dSceneObject object
/// @param time The number of milliseconds between updates
/// @param rate The amount of alpha to change on each step.
///
function t2dSceneObject::fadeIn(%this, %time, %rate)
{   
   if (%this.fadeEvent != 0)
   {
      cancel(%this.fadeEvent);
      %this.fadeEvent = 0;
   }

   // Fetch the current alpha value
   %alpha = %this.getBlendAlpha();

   // Decrease it
   %alpha += %rate; 

   if(%alpha > 1)
   {         
   }
   else
   {
      %this.setBlendAlpha( %alpha );
      %this.fadeEvent = %this.schedule(%time, fadeIn, %time, %rate);
   }
}
