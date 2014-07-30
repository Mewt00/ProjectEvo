$card::defaultMinSpeed = 230;  // In general, cards move at this speed.
$card::defaultMinTime = 0.2;  // If movement will take longer than this many seconds, increase speed.

$card::sizeX = 20;     // The size of the card sprites.
$card::sizeY = 27.143;

$card::cameraArea = sceneWindow2D.getCurrentCameraArea();
$card::sortSizeMultiplier = (getWord($card::cameraArea, 3) - getWord($card::cameraArea, 1)) / $card::sizeY;

$card::motionLayer = 5;  // A high layer, so cards in flight don't end up underneath other cards.
$card::fireworkLayer = 1;
$card::buttonLayer = 0;  // The highest layer -- the GUI is above all!

function associateCardsWithImages(%deck, %scenegraph)
{
   for (%i=0; %i<%deck.getNumCards(); %i++)
   {
      %card = %deck.getCard(%i);
      for (%s=0; %s<getWordCount($card::suits); %s++)
      {
         if (getWord($card::suits, %s) $= %card.getSuit()) {break;}
      }
      for (%v=0; %v<getWordCount($card::values); %v++)
      {
         if (getWord($card::values, %v) $= %card.getValue()) {break;}
      }
   
      %card.sprite = new t2dStaticSprite()
      {
         scenegraph = %scenegraph;
         backImageMap = cardback_smlImageMap;
         frontImageMap = playingcardsImageMap;
         frontFrame = %s*getWordCount($card::values) + %v;
         imageMap = cardback_smlImageMap;
         size = $card::sizeX SPC $card::sizeY;
         layer = 15;
         class = "cardSprite";
         card = %card;
      };
      %stackOriginX = %deck.getPositionX() - %deck.getSizeX()/2 + 
                      %card.sprite.getSizeX()/2;
      %stackOriginY = %deck.getPositionY() - %deck.getSizeY()/2 + 
                      %card.sprite.getSizeY()/2;

      %positionX = %stackOriginX + %card.owner.hSkew * %card.index;
      %positionY = %stackOriginY + %card.owner.vSkew * %card.index;
      %card.sprite.setPosition(%positionX, %positionY);

      %card.callbackActive = true;
      %card.onStatusUpdate();
   }
}

// This allows the "onStatusUpdate" effect to be delayed from the actual update.
function card::setUpdateDelay(%this, %delay)
{
   %this.delaying = true;
   %this.delayTime = %delay;
}

// A helper function to implement deferred updates.
function card::deferredUpdate(%this)
{
   //echo(%this.getCard() SPC "doing deferred update" SPC getRealTime());
   %this.delaying = false;
   %this.deferScheduled = false;
   %this.onStatusUpdate();
}
   

function card::onStatusUpdate(%this)
{
   // Sometimes we don't want the update to take place right away.
   // This defers it until later.
   if (%this.delaying && !%this.deferScheduled)
   {
      %this.schedule(%this.delayTime, deferredUpdate);
      %this.deferScheduled = true;
      return;
   }
   else if (%this.delaying)
   {
      return;
   }

   if (!isObject(%this.owner))
   {
      error(%this SPC "card doesn't have an owner!" SPC %this.owner);
      %this.danger();
   }
   
   %stackOriginX = %this.owner.getPositionX() - %this.owner.getSizeX()/2 + 
                   %this.sprite.getSizeX()/2;
   %stackOriginY = %this.owner.getPositionY() - %this.owner.getSizeY()/2 + 
                   %this.sprite.getSizeY()/2;
   
   %positionX = %stackOriginX + %this.owner.hSkew * %this.index;
   %positionY = %stackOriginY + %this.owner.vSkew * %this.index;
   if ((%positionX != %this.sprite.getPositionX()) ||
       (%positionY != %this.sprite.getPositionY()))
   {
      %distance = t2dVectorDistance(%this.sprite.getPosition(), 
                                       %positionX SPC %positionY);
      %minSpeed = (%this.owner.cardMinSpeed > 0) ? (%this.owner.cardMinSpeed) : ($card::defaultMinSpeed);
      %minTime =  (%this.owner.cardMinTime > 0)  ? (%this.owner.cardMinTime) :  ($card::defaultMinTime);

      %speed = t2dGetMax(%minSpeed, %distance/%minTime); // get there in third second or less.

      %this.sprite.moveTo(%positionX, %positionY, %speed, true, true);  // set up callback to support movement in "motionLayer"
   }
   if (%this.facing $= "up")
   {
      %this.sprite.imageMap = %this.sprite.frontImageMap;
      %this.sprite.frame = %this.sprite.frontFrame;
   }
   else
   {
      %this.sprite.imageMap = %this.sprite.backImageMap;
      %this.sprite.frame = 0;
   }
   %this.sprite.setSortPointY(%this.index*$card::sortSizeMultiplier);
   if (%this.sprite.getAtRest())
   {
      %this.sprite.setLayer(%this.owner.getLayer());
   }
   else
   {
      %this.sprite.setLayer($card::motionLayer);
   }
}

function cardSprite::onPositionTarget(%this)
{
   %this.setLayer(%this.card.owner.getLayer());
}



function cardGraphics::setCard(%this, %suit, %value)
{
   %this.setFrame(%s*getWordCount($card::values) + %v);
}


function SceneWindow2D::danger(%this)
{
   if (isObject(%this.danger))
   {
      %this.danger.delete();
   }
   /*%this.danger = new t2dStaticSprite()
   {
      imageMap = dangerImageMap;
      scenegraph = sceneWindow2D.getSceneGraph();
      size = 80 SPC 20;
      position = 0 SPC 0;
   };*/
}




function t2dSceneObject::startFadeOut(%this, %time, %rate){
        %this.schedule(%time, fadeOut, %time, %rate);
}

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

function t2dSceneObject::startFadeIn(%this, %time, %rate){
        %this.schedule(%time, fadeIn, %time, %rate);
}

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

function SceneWindow2D::onMouseDragged(%this, %modifier, %worldPosition, %clicks)
{
   %this.onMouseMove(%modifier, %worldPosition, %clicks);
}

function SceneWindow2D::moveChipStackToMouse(%this)
{
   %worldPosition = %this.getMousePosition();
   %mx = getWord(%worldPosition, 0);
   %my = getWord(%worldPosition, 1);
   %this.chipStack.hSkew = %mx/350;
   %this.chipStack.vskew = -(75 - %my)/350;
   %this.chipStack.setPosition(%worldPosition);
   for (%i=0; %i<%this.chipStack.getNumCards(); %i++)
   {
      %this.chipStack.getCard(%i).onStatusUpdate();
   }
}
   

function SceneWindow2D::onMouseMove(%this, %modifier, %worldPosition, %clicks)
{
   %this.checkGuiButtons(%worldPosition);   
}



function SceneWindow2D::onMouseDown(%this, %modifier, %worldPosition, %clicks)
{
   %this.mouseButtonDown = true;
   %this.checkGuiButtons(%worldPosition);   
   
   // We lose track of what happens to the mouse when it leaves our window.
   // If a mouseDown is the first thing that happens after entry, then all is
   // right with the world.
   if (%this.mouseRecentEntry)
   {
      %this.mouseRecentEntry = false;
   }

   // First, do some bookkeeping to take care of the situation when the engine 
   // gets confused and sends onMouseDown/onMouseUp events out of order with each
   // other.
   if (%this.mouseDownDebt > 0)
   {
      %this.mouseDownDebt--;
      return;
   }
   
   %this.mouseDown++;
   if (%this.mouseDown == 2)
   {
      // the window manager is confused, and sent us a second "onMouseDown" before
      // we got an "onMouseUp" event.  We'll compensate by calling the onMouseUp
      // function ourselves.  We keep track of doing this with "mouseUpDebt", because
      // the "real" onMouseDown event will come later, and we don't want to run the
      // onMouseDown function more often than we should.
      %this.onMouseUp(%modifier, %worldPosition, %clicks);
      %this.mouseUpDebt++;
   }
      
   if (%this.mouseDown != 1)
   {
      %this.danger();
      error("onMouseDown called" SPC %this.mouseDown SPC "times in a row!");
   }


   if (%this.guiActive)
   {
      // GUI is up, don't do any game functions.
      return;
   }

   if (!%this.allowGameInput)
   {
      // Mouse is disabled, because we're dealing out the cards.
      return;
   }
}
      
function SceneWindow2D::onMouseUp(%this, %modifier, %worldPosition, %clicks)
{
   %this.mouseButtonDown = false;
   %this.checkGuiButtons(%worldPosition);   

   // We lose track of what happens to the mouse when it leaves our window.
   // Absorb spurious onMouseUps that might come from that.
   if (%this.mouseRecentEntry)
   {
      %this.mouseRecentEntry = false;
      return;
   }
   
   // Sometimes the window manager sends a mousedown before a mouseup.
   // When this happens we call phantom onMouseDowns.  We need to absorb
   // these phantoms when the "real" onMouseDown event comes.
   if (%this.mouseUpDebt > 0)
   {
      %this.mouseUpDebt--;
      return;
   }
   
   // bookkeeping to detect problems with out-of-sequence mouseDown/mouseUp events.
   %this.mouseDown--;

   if (%this.mouseDown == -1)
   {
      // We got a mouseUp before the corresponding mouseDown, so do a phantom one.
      %this.onMouseDown(%modifier, %worldPosition, %clicks);
      %this.mouseDownDebt++;
   }

   if (%this.mouseDown != 0)
   {
      %this.danger();
      error("onMouseUp called, and" SPC %this.mouseDown SPC "onMouseDowns still outstanding!");
   }

   if (%this.guiActive)
   {
      // GUI is up, don't do any game functions.
      %this.activateGuiButton(%worldPosition);
      return;
   }

   if (!%this.allowGameInput)
   {
      // Mouse is disabled, because we're dealing out the cards.
      return;
   }
}
         

function SceneWindow2D::onMouseLeave(%this, %modifier, %worldPosition, %clicks)
{
   // mouse left the window.
   if ($mouseDown > 0)
   {
      // treat it like a virtual onMouseUp.
      %this.onMouseUp(%modifier, %worldPosition, %clicks);
   }
   
   // We can refresh our bookkeeping.
   %this.mouseDown = 0;
   %this.mouseUpDebt = 0;
   %this.mouseDownDebt = 0;
}
   
function SceneWindow2D::onMouseEnter(%this, %modifier, %worldPosition, %clicks)
{
   %this.mouseRecentEntry = true;
}


function SceneWindow2D::toggleGui(%this)
{
   if (%this.guiActive)
   {
      %this.removeGui();
   }
   else
   {
      %this.popUpGui();
   }
}

function SceneWindow2D::popUpGui(%this)
{
   %this.quitButton = new t2dStaticSprite()
   {
      class = "button";
      scenegraph = sceneWindow2D.getSceneGraph();
      imageMap = quitButtonImageMap;
      frame = 0;
      size = 50 SPC 25;
      position = -27 SPC 0;
      blendColor = 1 SPC 1 SPC 1 SPC 0;
      layer = $card::buttonLayer;
      command = "endGame();";
   };
   %this.quitButton.fadeIn(25, 0.1);
   
   %this.dealButton = new t2dStaticSprite()
   {
      class = "button";
      scenegraph = sceneWindow2D.getSceneGraph();
      imageMap = dealButtonImageMap;
      frame = 0;
      size = 50 SPC 25;
      position = 27 SPC 0;
      blendColor = 1 SPC 1 SPC 1 SPC 0;
      layer = $card::buttonLayer;
      command = "resetDeck();";
   };
   %this.dealButton.fadeIn(25, 0.1);

   %this.guiActive = true;
}

function SceneWindow2D::removeGui(%this)
{
   %this.quitButton.fadeOut(25, 0.1);
   %this.dealButton.fadeOut(25, 0.1);
   %this.guiActive = false;
}

function SceneWindow2D::checkGuiButtons(%this, %worldPosition)
{
   if (isObject(%this.buttonSet))
   {
      for (%i=0; %i<%this.buttonSet.getCount(); %i++)
      {
         %button = %this.buttonSet.getObject(%i);
         if (%button.enable)
         {
            %button.setFrame(0);
         }
         else
         {
            %button.setFrame(3);
         }
      }
   }
   
   %buttons = %this.getSceneGraph().pickPoint(%worldPosition, 1, BIT($card::buttonLayer));

   for (%i=0; %i<getWordCount(%buttons); %i++)
   {
      %object = getWord(%buttons, %i);
      if (%object.class !$= "button")
      {
         //error("Button finder found something besides a button!" SPC %object);
         %this.danger();
      } else
      {
         if (!%object.enable)
         {
            %object.setFrame(3);
         } else if (%this.mouseButtonDown)
         {
            %object.setFrame(2);
         } else
         {
            %object.setFrame(1);
         }
      }
   }
}

function SceneWindow2D::activateGuiButton(%this, %worldPosition)
{
   %buttons = sceneWindow2D.getSceneGraph().pickPoint(%worldPosition, 1, BIT($card::buttonLayer));
   for (%i=0; %i<getWordCount(%buttons); %i++)
   {
      %object = getWord(%buttons, %i);
      if (%object.class !$= "button")
      {
         //error("Button finder found something besides a button!" SPC %object);
         %this.danger();
      }
      if (%object.enable)
      {
         //error("objectCommand = " @ %object.command);
         eval(%object.command);
      }
   }
}

function button::onAdd(%this)
{
   if (!isObject(SceneWindow2D.buttonSet))
   {
      SceneWindow2D.buttonSet = new SimSet();
   }
   
   SceneWindow2D.buttonSet.add(%this);
}
