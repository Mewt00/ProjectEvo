// This file handles the interaction between the user and the underlying static sprites that
// display on our map.

function PainterObject::onLevelLoaded(%this, %sceneGraph)
{
   // this object is what will be mounted to our mouseObject once the level load
   // is complete. We use a two object mechanism here to follow the mouse pointer
   // because you cannot mount an object directly to the mouse pointer, but setPosition()
   // doesn't allow our swept poly physics to work fully
   $PainterObject = %this;
   %this.enableUpdateCallback();
}

function PainterObject::onUpdate(%this)
{
   
   %moveToLoc = mouseObject.getPosition();
     %this.moveTo(%moveToLoc,
                  15000,     // speed
                  true,  // stop?
                  false,   // callback?
                  true,  // snap?
                  0.2     // margin
                  );
                  
}
function initializeMap(%template)
{
   
   // set up defaults in a 4:3 ratio
   
   if (!isObject(%template) )
   {
      error("initializeMap: You must provide a template object to clone!");
      return false;
   }

   %gridSize = $pathgrid.currentTileLayer.getTileCount();
   %xSize = getWord(%gridSize, 0);
   %ySize = getWord(%gridSize, 1);
   %spriteSize = $pathgrid.currentTileLayer.getTileSize();
   %spriteSizeX = getWord(%spriteSize, 0);
   %spriteSizeY = getWord(%spriteSize, 1);
   
   for (%xIndex = 0; %xIndex < %xSize; %xIndex++)
   {
      for (%yIndex = 0; %yIndex < %ySize; %yIndex++)
      {
        %worldPosition = $pathgrid.currentTileLayer.getTileWorldPosition(%xIndex, %yIndex);
        %newGridObject = %template.cloneWithBehaviors();
        %newGridObject.setSize(%spriteSizeX, %spriteSizeY);
        %newGridObject.aStarGrid = $pathgrid;
        %newGridObject.setPosition(%worldPosition);
      }
   }
}
       
function GridObject::onCollision(%srcObject, %dstObject, %srcRef, %dstRef, %time, %normal, %contacts, %points)
{
   
   %startFrame = 0;
   %endFrame = 7;
   %frameTime = 0; // use the game default. We can override that default here if we wish
   
   if (%dstObject == $PainterObject)
   {
      // check the state of the mouse button
      if ($ActiveMouseObject::mouseDown == true)
      {
         // start a paint cycle on this object
         %srcObject.startPaint(%startFrame, %endFrame, %frameTime);
      }
   }
}

// Set up a global "tweak" preference that allows us to easily change the timing
// for our painting tasks in one place. By default, there are 30 ticks per second,
// or 1/30 seconds per tick.


$pref::Sim::TicksPerSecond = 30.0;
$pref::Game::FrameDurationSeconds = 2.0;

function GridObject::onUpdate(%this)
{
   // this callback occurs for us every 32 milliseconds, so we want to keep track
   // of our countdown to the next frame change, and only change frames when our currentTimer
   // has run out.
   %this.currentFrameTimer--;
   // check to see if we've reached a frame change
   if (%this.currentFrameTimer <= 0)
   {
      // time to change our frame, and update our counter
      // we have two cases: we're either on the last frame, or we have other frames to render
      if (%this.currentFrame == %this.endFrame -1)
      {
         // we're done painting this block for now
         %this.setFrame(%this.endFrame);
         %this.currentFrameTimer = -1;
         // the following is an optimization since in our current state, we no longer need to update
         // frames, we also no longer need update callbacks. We also know that the process of starting
         // a frame change on a GridObject is triggered by a collision, and it's the responsibility
         // of the onCollision to turn on tick physics callbacks, so we can safely turn them off now,
         // saving a lot of processing time, since there are so many GridObjects!
         %this.disableUpdateCallback();
         
         // change the aStar world state
         %worldPosition = %this.getPosition();
         %worldX = getWord(%worldPosition, 0);
         %worldY = getWord(%worldPosition, 1);
         %this.aStarGrid.setTileWeightByWorldCoord(%worldX, %worldY, "0", true);
      }
      else
      {
         // here's our second case: we need to move the next frame to display

         %this.currentFrame++;
         %this.setFrame(%this.currentFrame);
         %this.currentFrameTimer = %this.timePerFrame;
       
      }
   }
}
      

function GridObject::startPaint(%this, %startFrame, %endFrame, %frameTime)
{
  %this.startFrame = %startFrame;
  %this.endFrame = %endFrame;
  
  // see if the caller has overridden our standard frame time
  if (%frameTime == 0)
  {
    %paintFrameTime = $pref::Game::FrameDurationSeconds;
  }
  else
  {
     %paintFrameTime = %frameTime;
  }
  
  %this.timePerFrame = %paintFrameTime * $pref::Sim::TicksPerSecond;
  %this.currentFrameTimer = %this.timePerFrame;
  %this.setFrame(%startFrame);
  %this.currentFrame = %startFrame;
  // make sure we turn on tick callbacks so our onUpdate callback handler is called
  %this.enableUpdateCallback();

  // change the aStar world state
  %worldPosition = %this.getPosition();
  %worldX = getWord(%worldPosition, 0);
  %worldY = getWord(%worldPosition, 1);
  %this.aStarGrid.setTileWeightByWorldCoord(%worldX, %worldY, "10", true);

}
   
