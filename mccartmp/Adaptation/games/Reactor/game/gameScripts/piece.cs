//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

/// @class Piece
/// @brief The class for one reactor piece
///
/// The Piece class manages one piece, meaning basically the turning functions
///
/// @field int turnSpeed The speed that the piece will turn


/// (SimID this)
/// This callback function is called by the engine when %this is clicked.
/// When a piece is clicked and there are no reactions currently going on, 
/// we reset the score and then prepare and start the clicked piece moving.
/// 
/// @param this The Piece object
function Piece::onMouseUp(%this,%modifier,%worldPos, %mouseClicks)
{  
   //sanity is fun!
   if( !isObject(%this.board) )
      error("The clicked piece doesn't belong to a board.");
   
   //first, check if turning is already happening
   //since only one reaction can go on at a time
   if(%this.board.isStable())
   {
      //reset the score, but not the board
      %this.board.newReaction(true, false);
      
      //now, start this Piece turning
      %this.prepareTurn();
      %this.startTurn(); 
      
   }
   
}

/// (SimID this)
/// This function sets a rotationTarget for the object and increases the score.
/// It is very important to activate a turn in two steps so that the logic
/// that starts a turn doesn't conflict with itself as it runs through the array
/// 
/// @param this The Piece object
function Piece::prepareTurn(%this)
{
   
   //use the next orientation, but don't change it here
   %this.setRotationTarget((%this.orientation+1) * 90.0, true, true, true, 0.1);
   %this.hasTarget = true;   
   
   //add 1 to the board's score
   %this.board.incScore(1);
}

/// (SimID this)
/// This function should be called after prepareTurn has been called. It will
/// start the piece rotating towards the rotationTarget from prepareTurn.
/// 
/// @param this The Piece object
function Piece::startTurn(%this)
{
   //Change the piece's orientation to reflect where it's turning
   %this.orientation++;
   %this.orientation = %this.orientation % 4;
   
   //start turning - it will stop when it hits the rotationtarget
   //turnSpeed is declared as a dynamic field in the level builder
   %this.setAngularVelocity(%this.board.turnSpeed);
   
   //add one to the number of pieces that are turning currently
   %this.board.movingPieces++;
   
   //start a particle effect
   if( %this.board.useParticles )
      %this.particle.playEffect();
   
   //Switch the piece to its rotating animation
   %this.playAnimation(pieceCellsRotating, true, 0, false);
   
   //play a sound
   playNextPop();
   
}

/// (SimID this)
/// This callback function is called by the engine when the rotation target is
/// hit. When that is done, we can check if all the pieces are done rotating,
/// in which case we are ready for the board to start the next round of turns.
/// 
/// @param this The Piece object
function Piece::onRotationTarget(%this)
{
   //Return the piece to its non-rotating animation
   %this.playAnimation(pieceCellsAnimation, true, 0, true);
   
   %this.hasTarget =false;
   
   //this is important because only pieces that moved last state
   //can activate ones when calculating the next
   %this.movedLastEval = true;
   
   //one fewer piece is moving
   %this.board.movingPieces--;
   
   //if there are no Pieces being turned
   //  then we can evaluate any new moves
   if(%this.board.isStable())
      %this.board.evalBoard();
   
   
}

/// (SimID this)
/// generates a random rotation and then rotates the piece to that
/// 
/// @param this The Piece object
/// 
function Piece::randomizeOrientation(%this)
{
   //randomize the pieces orientation. It can be one of 4 states
   // up, right, down, left
   %this.orientation = getRandom(0,3);
   //then we rotate the piece to that orientation
   %this.setRotation(%this.orientation * 90.0);
}


/// (SimID this)
/// Returns whether one of the piece's activation nodes is pointing up
/// 
/// @param this The Piece object
/// @return bool indicating whether or not there is a node pointing up
function Piece::up(%this)
{
   return ((%this.orientation == 0) || (%this.orientation == 1));
}

/// (SimID this)
/// Returns whether one of the piece's activation nodes is pointing down
/// 
/// @param this The Piece object
/// @return bool indicating whether or not there is a node pointing down
function Piece::down(%this)
{
   return ((%this.orientation == 2) || (%this.orientation == 3));
}

/// (SimID this)
/// Returns whether one of the piece's activation nodes is pointing left
/// 
/// @param this The Piece object
/// @return bool indicating whether or not there is a node pointing left
function Piece::left(%this)
{
   return ((%this.orientation == 0) || (%this.orientation == 3));
}

/// (SimID this)
/// Returns whether one of the piece's activation nodes is pointing right
/// 
/// @param this The Piece object
/// @return bool indicating whether or not there is a node pointing right
function Piece::right(%this)
{
   return ((%this.orientation == 1) || (%this.orientation == 2));
}