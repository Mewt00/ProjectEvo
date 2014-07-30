//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

/// @class Board
/// @brief Manages a reactor board, setting up and evaluating stages
///
/// The Board should be a tilelayer, named grid if it's the reactor board
/// The board will contain pieces, set them up, reset them, and manage reactions


/// (SimID this, Scenegraph scenegraph)
/// Performs the initial setup of the playing board. Creates all the pieces,
/// their particle effect, and randomizes the board.
/// 
/// @param this The Board object
///
function Board::onLevelLoaded(%this, %scenegraph)
{
   //Save our scenegraph for when we
   //create new objects in it
   $reactorGraph = %this.getSceneGraph();

   //turn particles off here if you'd like
   %this.useParticles = true;

   %this.score = 0;
   %this.highScore = 0;
    
    //just so we can use shorter names ;)
   %this.xCount = %this.getTileCountX();
   %this.yCount = %this.getTileCountY();
   
   //the shakeAt variable indicates how many pieces need to be active to
   // start a camera shake
   %this.shakeAt = %this.xCount * %this.yCount / 12;
   
   //if our model piece for cloning doesn't exist, give up on everything
   if( !isObject( pieceParent ) )
   {
      error("Requires prototype piece called 'pieceParent' to clone");
      return;
   }
   
   //Create all of our pieces in a double loop. They will be placed
   //in an array owned by the grid and set up.  
   for(%a = 0; %a < %this.xCount; %a++)
   {
      for(%b = 0; %b < %this.yCount; %b++)
      {         
         //we're going to use the piece from the
         // levelbuilder as a model for the rest
         %this.pieces[%a,%b] = pieceParent.clone(true);
         
         //Following line shouldn't be needed but fixes some mysterious clone
         // stream error that happens about 1/20 times
         //Dolly had problems too, you know
         while(%this.pieces[%a,%b] $= 0)
         {
            %this.pieces[%a,%b] = pieceParent.clone(true);
         }
         
         //let the piece know who it's daddy/board is (so it can report back to the board)
         %this.pieces[%a,%b].board = %this;
         
         //set the position of the piece relative to the
         //  tile layer (grid) we made
         %this.pieces[%a,%b].setPosition(%this.getTileGridPosition(%a, %b));
         
         //set the size to the size of a grid soace
         %this.pieces[%a,%b].setSize(%this.getTileSize());
         
         //randomize the piece's orientation.
         %this.pieces[%a,%b].randomizeOrientation();
         
         //make sure our piece is clickable!
         %this.pieces[%a,%b].setUseMouseEvents(true);
         
         if( %this.useParticles )
         {         
            //Make a new particle effect for the piece
            %this.pieces[%a,%b].particle = new t2dParticleEffect() 
            {
               scenegraph = $reactorGraph;
            };   
            
            %this.pieces[%a,%b].particle.loadEffect("~/data/particles/effect.eff");
            %this.pieces[%a,%b].particle.setPosition(%this.pieces[%a,%b].getPosition());
            %this.pieces[%a,%b].particle.setLayer(%this.pieces[%a,%b] - 1);
         }
                  
      }
   }
}

/// (SimID this, bool resetScore, bool randomizeBoard)
/// Resets the score and board if desired, but only if the 
/// board is still and stable
/// 
/// @param this The Board object
/// @param resetScore Whether to reset the score or not
/// @param randomizeBoard Whether to reset the board or not
///
function Board::newReaction(%this, %resetScore, %randomizeBoard)
{  
   //A new reaction cannot be started if there is already one going on
   if(!%this.isStable())
      return; 
   
   if(%resetScore)
      %this.score = 0;   
   
   if(%randomizeBoard)
   {      
      for(%a = 0; %a < %this.xCount; %a++)
      {
         for(%b = 0; %b < %this.yCount; %b++)
         {
            %this.pieces[%a,%b].randomizeOrientation();
         }
      }
   }
}

/// (SimID this, int amount)
/// Adds amount to the score
/// 
/// @param this The Board object
/// @param amount Quantity to add to the score
///
function Board::incScore(%this, %amount)
{
   %this.score += %amount;
   %this.updateScoreText(%this.score);
}

/// (SimID this)
/// This function is called when all pieces have reached their rotation targets.
/// The function finds which pieces were just activated, sets up their targets,
/// and begins their rotation.
/// 
/// @param this The Board object
///
function Board::evalBoard(%this)
{  
      
   //We're going to go through the entire board and check if a piece that turned
   // last reaction frame has turned to "face" another one. If so, activate the
   // next one!
   for(%a = 0; %a < %this.xCount; %a++)
   {
      for(%b = 0; %b < %this.yCount; %b++)
      {
         //determine if the piece is aligned with
         //  an adjacent one - if it is, prepare the turn
         if( %this.pieces[%a,%b].movedLastEval)
         {
            %this.checkLinkAndActivate(%a, %b, 1, 0);
            %this.checkLinkAndActivate(%a, %b,-1, 0);
            %this.checkLinkAndActivate(%a, %b, 0, 1);
            %this.checkLinkAndActivate(%a, %b, 0,-1);
   
         }         
      }
   }   
   
   //Now that we have prepared the pieces that were activated, we need to turn
   // them. The two loops cannot be combined because if we did, then the one
   // state would be overwritten as we started the turns.
   for(%a = 0; %a < %this.xCount; %a++)
   {
      for(%b = 0; %b < %this.yCount; %b++)
      {
         //set the piece to not have moved last turn
         //this will be overwritten by the piece when (and if)
         // it hits its rotation target
         %this.pieces[%a,%b].movedLastEval = false;
         
         //turn the pieces that were given targets
         if(%this.pieces[%a,%b].hasTarget)
            %this.pieces[%a,%b].startTurn();            
      }
   }
   
   //If we've got lots of pieces moving, shake that camera!
   if(%this.movingPieces > %this.shakeAt)
   {
      sceneWindow2D.startCameraShake(2, 0.5);
   }
   
   //if all is calm and the score beats the current high, do the obvious
   if(%this.isStable() && %this.score > %this.highScore)
   {
      %this.highScore = %this.score;
      %this.updateHighScoreText(%this.highScore);
   }
  
}

/// (SimID this, int val)
/// Checks a piece at the given position with the one in the direction given
/// by the last two params. The pieces are checked to see if they line up.
/// If they do, the second one is prepared to turn.
/// 
/// @param this The Board object
/// @param val Where the piece is on the X axis
/// @return adjusted value
function Board::checkLinkAndActivate(%this, %pieceX, %pieceY, %checkX, %checkY )
{
   %turningPiece = %this.pieces[%pieceX,%pieceY];
   %checkingPiece = %this.pieces[%pieceX + %checkX,%pieceY + %checkY];
   
   //stops us from checking pieces that are off the board (don't exist)
   if(!isObject(%checkingPiece))
      return;
   
   if(%checkY == -1){
      if(%turningPiece.up() && %checkingPiece.down())
         %checkingPiece.prepareTurn();
   } else if ( %checkY == 1){
      if(%turningPiece.down() && %checkingPiece.up())
         %checkingPiece.prepareTurn();
   } else if ( %checkX == 1){
      if(%turningPiece.right() && %checkingPiece.left())
         %checkingPiece.prepareTurn();
   } else if ( %checkX == -1){
      if(%turningPiece.left() && %checkingPiece.right())
         %checkingPiece.prepareTurn();
   }

}


/// (SimID this)
/// Simple function that returns whether the board is reacting or not
/// 
/// @param this The Board object
/// @return True if the board is not reacting
function Board::isStable(%this)
{      
   return (%this.movingPieces <= 0);
}


/// (SimID this, int x, int y)
/// This function finds the world-relative position of a tile given the
/// tile-relative coordinates. (Basically the same as used in Checkers Tutorial)
/// 
/// @param this The Board object
/// @param x Grid x-Position on the tile layer
/// @param y Grid y-Position on the tile layer
/// @return World-relative position
function Board::getTileGridPosition(%this, %x, %y)
{
   // grab the Board layer's position
   %tileMapPos = %this.getPosition();
   // divide the position up into x and y variables
   %tileMapPosX = getWord(%tileMapPos, 0);
   %tileMapPosY = getWord(%tileMapPos, 1);

   // grab the Board layer's size
   %tileMapSize = %this.getSize();
   
   // divide the size up into x and y variables
   %tileMapSizeX = getWord(%tileMapSize, 0);
   %tilemapSizeY = getWord(%tileMapSize, 1);
   
   // calculate the start position
   %tileMapStartX = %tileMapPosX - (%tileMapSizeX / 2);
   %tileMapStartY = %tileMapPosY - (%tileMapSizeY / 2);
   
   // calculate the position and pass it back
   %tS = %this.getTileSize();
   %pos = (%tileMapStartX + (%x * %tS)) + %tS/2 SPC (%tileMapStartY + (%y * %tS)) + %tS/2;
   
   return %pos;
}

/// (SimID this, int score)
/// Function updates the score text object
/// 
/// @param this The Board object
/// @param score score to update the text with
function Board::updateScoreText(%this, %score)
{  
   //Spice this up when the text objects can be put in
   if(isObject(scoreText))
      scoreText.text = "Score: " @ %score;
   
}

/// (SimID this, int highScore)
/// Function updates the score text object
/// 
/// @param this The Board object
/// @param highScore score to update the text with
function Board::updateHighScoreText(%this, %highScore)
{  
   //Spice this up when the text objects can be put in
   if(isObject(highScoreText))
      highScoreText.text = "Best: " @ %highScore;
   
}