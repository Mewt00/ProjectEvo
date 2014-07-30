//-----------------------------------------------------------------------------
// GarageGames Billiards
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//As far as I can tell, there's no script-side global PI constant in the engine
$PI = 3.141592654;

//Consts used to specify whether the animation should be relative or absolute
$ANIM_MODE_REL = BIT(0);
$ANIM_MODE_ABS = BIT(1);

//-----------------------------------------------------------------------------

//The AnimationManager singleton is used to manage several animation "threads",
//or animation instances.

function AnimationManager::onAdd(%this)
{
   //Exec all of the animation datablock files
   exec( "./buttonBaseAnim.cs" );
   exec( "./flyInText.cs" );
   
   //Create a simset to keep track of the animation threads being managed
   %this.animThreads = new SimSet() {};
   
   %this.lastTime = getSimTime();
}

function AnimationManager::onRemove(%this)
{
   %this.animThreads.clear();
   %this.animThreads.delete();
}

//This function registers an animation with the manager, and the animation will
//now be updated by the manager until it ends.
//@arg %obj The t2dSceneObject that the animation affects
//@arg %animationSpec Specifies the animation datablock and the field affected,
//                    separated by a colon. The field component can be 
//                    "position", "size", "rotation", "alpha", or "color".
//@arg %mode Specifies whether the animation should be absolute or relative
//@arg %onAnimEnd Specifies the on anim end command for this animation thread
//@returns void
function AnimationManager::playAnimation(%this, %obj, %animationSpec, %mode, %onAnimEnd)
{
   //If they want to play an animation on an object that is already playing that
   //animation, ignore this request
   %numThreads = %this.animThreads.getCount();
   for(%i = 0; %i < %numThreads; %i++) 
   {
      
      %animThread = %this.animThreads.getObject(%i);
      
      nextToken(%animationSpec, token, ":");

      if(isObject(%animThread) && isObject(%animThread.obj) && isObject(%token) && isObject(%animThread.keyframeDatablock) 
         && %animThread.obj.getID() == %obj.getID() && %animThread.keyframeDatablock.getID() == %token.getID())
      {
         return;
      }
   }

   //Create a new animation thread
   %animThread = new ScriptObject() { class=AnimationThread; };
   %animThread.setup(%obj, %animationSpec, %mode);

   if( %onAnimEnd $= "" )
      %animThread.onAnimEnd = %animThread.keyframedatablock.onAnimEnd;
   else
      %animThread.onAnimEnd = %onAnimEnd;
   
   //Add this animation thread to the the set of threads to manage
   %this.animThreads.add(%animThread);
}

// This function plays a single keyframe animation specified not by a datablock but by a parameter
//@arg %obj The t2dObject that the animation affects
//@arg %keyframe The keyframe to be played
//@arg %field The field to be interpolated
//@arg %mode Specifies whether the animation should be absolute or relative
//@arg %onAnimEnd Specifies the on anim end command for this animation thread
//@returns void
function AnimationManager::playAnimationFrame(%this, %obj, %keyframe, %field, %mode, %onAnimEnd)
{
   //Create a new animation thread
   %animThread = new ScriptObject() { class=AnimationThread; };
   %animThread.obj = %obj;
   %animThread.keyframeDatablock.numKeyFrames = 1;
   %animThread.interpField = %field;
   %animThread.onAnimEnd = %onAnimEnd;
   //error("creating anim thread:" SPC %animThread SPC %obj SPC %keyframe SPC %mode SPC %onAnimEnd);
   
   //use the mode they specify  
   if(%mode != $ANIM_MODE_REL && %mode != $ANIM_MODE_ABS) 
   {
      error("ERROR: in AnimationManager::playAnimationFrame() : invalid argument for %mode - defaulting to Absolute"); 
      %animThread.mode = $ANIM_MODE_ABS;
   }
   else
      %animThread.mode = %mode;

   //Load the first key
   %leftovers = %keyframe;
         
   %leftovers = nextToken(%leftovers, token, ":");
   %animThread.keyTimeEnd = %token;
      
   %leftovers = nextToken(%leftovers, token, ":");
   %keyValueString = %token;
   %animThread.setInterpValues(%keyValueString);   
      
   %animThread.keyIdx = %keyIdx;
   %animThread.keyTimeCurr = 0;

 
   //Add this animation thread to the the set of threads to manage
   %this.animThreads.add(%animThread);
}


//This function stops all animations that are playing on the given object.
//@arg %obj The scene object for which to stop playing animations
//@returns void
function AnimationManager::stopAnimations(%this, %obj)
{
   %numThreads = %this.animThreads.getCount();
   for(%i = 0; %i < %numThreads; %i++)
   {
      if( %i >= %this.animThreads.getCount() )
         break;
         
      %animThread = %this.animThreads.getObject(%i);
      
      if( isObject(%animThread) )
      {
         if(!isObject(%animThread.obj))
         {
            %animThread.delete();
            %i--;
         }
         
         if(%animThread.obj.getID() == %obj.getID())
         {
            %animThread.delete();
            %i--;    
         }
      }
   }
   
}
  
//This function should be called from the t2dSceneGraph onUpdateScene()
//function. It loops through all the animation threads currently active
//and updates each one based on the time elapsed.
function AnimationManager::update(%this)
{
   %curTime = getSimTime();
   %timeElapsed = %curTime - %this.lastTime;
   %this.lastTime = %curTime;
   
   //Call update on each animation thread in the set
   for(%i = 0; %i < %this.animThreads.getCount(); %i++) 
   {
      %animThread = %this.animThreads.getObject(%i);
      
      // check if this thread exists, and if the object we are animating exists
      if(isObject(%animThread) && isObject(%animThread.obj))
      {
         %objSG = %animThread.obj.getSceneGraph();
         if( isObject( %objSG ) )
         {
            // now check if the scenegraph this object belongs to is paused
            if( !%objSG.getScenePause() )
               %animThread.update(%timeElapsed);
         }
         else // if we don't have a scenegraph, just let it update
            %animThread.update(%timeElapsed);
      }
      else
      {
         //The thread self-deleted or the object being animated is no more, so 
         //remove this now-invalid animation thread
         %this.animThreads.remove(%animThread);
      }
   }
   
}

//-----------------------------------------------------------------------------

//An animation thread is supposed to be an instance of an animation for a
//particular object.  It is managed by the AnimationManager and encapsulates
//the scripted animation for an animation instance...

//This function initializes the animation thread
function AnimationThread::setup(%this, %object, %animationSpec, %mode)
{
   %this.obj = %object;

   //Parse %animationSpec for the keyframe datablock and the attribute field
   //of the sprite that will be interpolated during the animation
   %animationSpec = nextToken(%animationSpec, token, ":");
   %this.keyframeDatablock = %token;
   %this.interpField = %animationSpec;
   
   //Set the mode for this animation thread (relative or absolute)
   //if they don't specify, use the default mode
   if(%mode $= "") 
   {
      %this.mode = %this.keyframeDatablock.defaultMode;
   }
   else 
   { 
      //use the mode they specify  
      if(%mode != $ANIM_MODE_REL && %mode != $ANIM_MODE_ABS) 
         error("ERROR: in AnimationThread::setup() : invalid argument for %mode"); 
      else 
         %this.mode = %mode;
   }

   //Load the first key
   %this.loadKey(0);

   return;
}

//This function loads an animation key for the animation thread. Note that
//the threads interpField member and mode member must already be set.
function AnimationThread::loadKey(%this, %keyIdx)
{
   %leftovers = %this.keyframeDatablock.key[%keyIdx];
      
   %leftovers = nextToken(%leftovers, token, ":");
   %this.keyTimeEnd = %token;
   
   %leftovers = nextToken(%leftovers, token, ":");
   %keyValueString = %token;
   %this.setInterpValues(%keyValueString);   
   
   %this.keyIdx = %keyIdx;
   %this.keyTimeCurr = 0;
   
   //echo("Just loaded a key to the thread:"); //TODO remove
   //%this.dump();
   
   return;
}

//This function parses the value portion of a keyframe in an animation datablock
//and resolves it with the animation mode (relative or absolute), to set the
//start and end values for the current keyframe region of the animation.
function AnimationThread::setInterpValues(%this, %keyValueString)
{
   //Set the start and end values to interpolate between based on the mode
   if(%this.interpField $= "position") 
   {
      %this.keyValStart = %this.obj.getPosition();

      if(%this.mode == $ANIM_MODE_ABS) 
      {
         %this.keyValEnd = %keyValueString;
      }
      else 
      { 
         //The mode is $ANIM_MODE_REL (bulletproof-checked already)
         %valEndX = %this.obj.getPositionX() + getWord(%keyValueString, 0);
         %valEndY = %this.obj.getPositionY() + getWord(%keyValueString, 1);
         %this.keyValEnd = %valEndX SPC %valEndY;
      }  
   }
   else if(%this.interpField $= "size") 
   {
      %this.keyValStart = %this.obj.getSize();

      if(%this.mode == $ANIM_MODE_ABS) 
      {
         %this.keyValEnd = %keyValueString;
      }
      else 
      { 
         //The mode is $ANIM_MODE_REL (bulletproof-checked already)
         %valEndWidth  = %this.obj.getSizeX() + getWord(%keyValueString, 0);
         %valEndHeight = %this.obj.getSizeY() + getWord(%keyValueString, 1);
         %this.keyValEnd = %valEndWidth SPC %valEndHeight;
      }     
   }
   else if(%this.interpField $= "rotation") 
   {
      %this.keyValStart = %this.obj.getRotation();
      
      if(%this.mode == $ANIM_MODE_ABS) 
      {
         %this.keyValEnd = %keyValueString;
      }
      else 
      { 
         //The mode is $ANIM_MODE_REL (bulletproof-checked already)
         %this.keyValEnd = %this.obj.getRotation() + %keyValueString;
      }     
   }   
   else if(%this.interpField $= "alpha") 
   {
      %currentColor = %this.obj.getBlendColor();
      %currentAlpha = getWord(%currentColor, 3);   
      %this.keyValStart = %currentAlpha;
      if(%this.mode == $ANIM_MODE_ABS) 
      {
         %this.keyValEnd = %keyValueString;
      }
      else 
      { 
         //The mode is $ANIM_MODE_REL (bulletproof-checked already)
         %this.keyValEnd = %currentAlpha + %keyValueString;
      }     
   }
   else if(%this.interpField $= "color") 
   {
      %currentColor = %this.obj.getBlendColor();
      %this.keyValStart = %currentColor;
      if(%this.mode == $ANIM_MODE_ABS) 
      {
         %this.keyValEnd = %keyValueString;
      }
      else 
      { 
         //The mode is $ANIM_MODE_REL (bulletproof-checked already)
         %endColorR = getWord(%currentColor, 0) + getWord(%keyValueString, 0);
         %endColorG = getWord(%currentColor, 1) + getWord(%keyValueString, 1);
         %endColorB = getWord(%currentColor, 2) + getWord(%keyValueString, 2);
         %endColorA = getWord(%currentColor, 3) + getWord(%keyValueString, 3);
         
         %this.keyValEnd = %endColorR SPC %endColorG SPC %endColorB SPC %endColorA;
      }     
   }
   else if(%this.interpField $= "adjcuedist")
   {
      %currentDistance = %this.obj.adjDistance;
      %this.keyValStart = %currentDistance;
      
      if(%this.mode == $ANIM_MODE_ABS)
      {
         %this.keyValEnd = %keyValueString;
      }
      else
      {
         //The mode is $ANIM_MODE_REL (bulletproof-checked already)
         %this.keyValEnd = %currentDistance + %keyValueString;
      }
   }
   else if(%this.interpField $= "cueinterp")
   {
      %currentAngle = $cueAngle;
      %this.keyValStart = %currentAngle;
      if(%this.mode == $ANIM_MODE_ABS)
      {
         %this.keyValEnd = %keyValueString;
      }
      else
      {
         //The mode is $ANIM_MODE_REL (bulletproof-checked already)
         %this.keyValEnd = %currentAngle + %keyValueString;
      }
   }
   else if(%this.interpField $= "numberOverlay")
   {
      %currentValue = $Game::ballNumberOverlayAlpha;
      %this.keyValStart = %currentValue;
      if(%this.mode == $ANIM_MODE_ABS)
      {
         %this.keyValEnd = %keyValueString;
      }
      else
      {
         //The mode is $ANIM_MODE_REL (bulletproof-checked already)
         %this.keyValEnd = %currentValue + %keyValueString;
      }
   }
   else if(%this.interpField $= "frameSize") 
   {
      %this.keyValStart = %this.obj.getSizeY();
      %this.keyValEnd = %keyValueString;
   }
   else 
   {
      error("ERROR: in AnimationThread::setInterpValues(): unknown %field arg:" SPC %field);
   }       
   
   return;
}

//This function updates the animation that this thread is managing. It calls the 
//interpolation function that does the actual mutating of the object being animated.
function AnimationThread::update(%this, %elapsedTime)
{
   //Convert %elapsedTime from seconds to milliseconds
   //%elapsedTime = %elapsedTime * 1000;
 
   //Update the elapsed time in this frame
   %this.keyTimeCurr += %elapsedTime;
    
   //Perform an update for the current key frame. If the current key frame has
   //exceeded its allotted time, the interpolation function should take care of
   //setting the sprite to the correct value of the keyframe's end.   
   interpolate(%this.obj, %this.interpField, %this.keyValStart, %this.keyValEnd, %this.keyTimeCurr, %this.keyTimeEnd);
   
   //If the end of the current key has been reached, either advance to the next
   //key or kill this AnimationThread
   if( %this.keyTimeCurr >= %this.keyTimeEnd )
   {
   
      //If we haven't reached the end of the animation
      if(%this.keyIdx < ( %this.keyframeDatablock.numKeyframes - 1 ) ) 
      {
         //We've probably gone a little into the next key, so make sure the animation flows smoothly
         %newKeyTimeCurr = %this.keyTimeCurr - %this.keyTimeEnd;
         %this.keyIdx++;
         %this.loadKey(%this.keyIdx);
         %this.keyTimeCurr = %newKeyTimeCurr;
         %this.update(0);
      }
      else 
      { 
         //Reached the end of the animation, so clean up this thread
         eval(%this.onAnimEnd);
         %this.delete();
      }
   }
}


//-----------------------------------------------------------------------------

// Interpolates certain values of a sprite, ranging from position to color.
// @returns void
// @arg %obj The sprite to be interpolated
// @arg %field A string that specifies what property of the sprite to 
//             interpolate. Possible values: "position", "rotation", "size", 
//             "color"
// @arg %valStart integer The beginning value of the %field arg
// @arg %valStart integer The end value of the %field arg
// @arg %time The total time of the interpolation in milliseconds
function interpolate(%obj, %field, %valStart, %valEnd, %tCurr, %tEnd)
{
   
   //Adjust the time arguments; for some reason 1000 milliseconds to the schedule
   //function is really a value of 100, don't know why
   //%tCurr /= 10;
   //%tEnd /=10;
   
   //The interpolateHelper() only interpolates a single variable, but for some
   //interpolation tasks, we'll need to vary multiple components...
   if(%field $= "position") 
   {
      interpolateHelper(%obj, "positionX", getWord(%valStart, 0), getWord(%valEnd, 0), %tCurr, %tEnd, %callback);
      interpolateHelper(%obj, "positionY", getWord(%valStart, 1), getWord(%valEnd, 1), %tCurr, %tEnd, %callback);
   }
   else if(%field $= "size") 
   {
      interpolateHelper(%obj, "sizeX", getWord(%valStart, 0), getWord(%valEnd, 0), %tCurr, %tEnd, %callback);
      interpolateHelper(%obj, "sizeY", getWord(%valStart, 1), getWord(%valEnd, 1), %tCurr, %tEnd, %callback);
   }
   else if(%field $= "rotation") 
   {
      interpolateHelper(%obj, %field, %valStart, %valEnd, %tCurr, %tEnd, %callback);
   }   
   else if(%field $= "alpha") 
   {
      interpolateHelper(%obj, "colorA", %valStart, %valEnd, %tCurr, %tEnd, %callback);
   }
   else if(%field $= "color") 
   {
      interpolateHelper(%obj, "colorR", getWord(%valStart, 0), getWord(%valEnd, 0), %tCurr, %tEnd, %callback);
      interpolateHelper(%obj, "colorG", getWord(%valStart, 1), getWord(%valEnd, 1), %tCurr, %tEnd, %callback);
      interpolateHelper(%obj, "colorB", getWord(%valStart, 2), getWord(%valEnd, 2), %tCurr, %tEnd, %callback);
      interpolateHelper(%obj, "colorA", getWord(%valStart, 3), getWord(%valEnd, 3), %tCurr, %tEnd, %callback);
   }
   else if(%field $= "adjcuedist")
   {
      interpolateHelper(%obj, %field, %valStart, %valEnd, %tCurr, %tEnd, %callback);
   }
   else if(%field $= "cueinterp")
   {
      interpolateHelper(%obj, %field, %valStart, %valEnd, %tCurr, %tEnd, %callback);
   }
   else if(%field $= "numberoverlay") 
   {
      interpolateHelper(%obj, %field, %valStart, %valEnd, %tCurr, %tEnd, %callback);
   }
   else if(%field $= "frameSize") 
   {
      interpolateHelper(%obj, %field, %valStart, %valEnd, %tCurr, %tEnd, %callback);
   }
   else 
   {
      error("ERROR: in interpolate(): unknown %field arg:" SPC %field);
   }

   return;
}

//This is where the actual interpolation math and mutating of the animating object gets done.
function interpolateHelper(%obj, %field, %valStart, %valEnd, %tCurr, %tEnd)
{
   //echo("interpolateHelper called on" SPC %obj SPC %field SPC %valStart SPC %valEnd SPC %tCurr SPC %tEnd);
   
   //If we have reached the end of the animation, short circuit the calculation
   if(%tCurr >= %tEnd) 
   {
      %val = %valEnd;
   }
   else 
   { 
      //Calculate the new interpolated value for this time step


      //Since sinArgA and B aren't getting passed in anymore, just set them to some nice values
      //%sinArgA = 50; 
      //%sinArgB = 100;
      //Change the %sinArg's from a value range of 0 to 100 into a range of -$PI/2 to $PI/2
      //%sinArgA = $PI * %sinArgA / 100 - $PI/2; 
      //%sinArgB = $PI * %sinArgB / 100 - $PI/2;
      //Calculate the interpolated value for the field at this point in the "animation" time     
      //Calculate the percentage along the y-distance of our shifted sine curve function
      //%sinArgDiff = mAbs(%sinArgA - %sinArgB);
      %sinArgDiff = -1.57;
      
      
      //avoid division by zero      
      if(%sinArgDiff < 0.0001) 
      { 
         //If you want to use only linear interpolation, use this instead
         %percentage = %tCurr / %tEnd; //effectively linear interpolation
      }
      else 
      {      
         //%l is the x-position along our shifted sine curve corresponding to the current
         //time in the interpolation
         %l = %tCurr / %tEnd * (%sinArgB - %sinArgA) + %sinArgA;
           
         %fL = mSin(%l)       * 0.5 + 0.5;
         %fA = mSin(%sinArgA) * 0.5 + 0.5;
         %fB = mSin(%sinArgB) * 0.5 + 0.5;
         %percentage = (%fL - %fA) / (%fB - %fA);
      }
   
      //Finally, calculate the interpolated value
      %val = %percentage * (%valEnd - %valStart) + %valStart;
      
   }

   //We now have our new interpolated value for this time step, so set the
   //appropriate field in the sprite
   if(%field $= "positionX") 
   {
      %obj.setPositionX(%val);
   }
   else if(%field $= "positionY") 
   {
      %obj.setPositionY(%val);
   }
   else if(%field $= "sizeX") 
   {
      //echo("setting X size to " @ %val);
      %obj.setSizeX(%val);
   }
   else if(%field $= "sizeY") 
   {
      //echo("setting Y size to " @ %val);
      %obj.setSizeY(%val);
   }
   else if(%field $= "rotation") 
   {
      %obj.setRotation(%val);
   }   
   else if(%field $= "colorR" || %field $= "colorG" || %field $= "colorB" || %field $= "colorA") 
   {
      %currentColor = %obj.getBlendColor();
      %currentColorR = getWord(%currentColor, 0);
      %currentColorG = getWord(%currentColor, 1);
      %currentColorB = getWord(%currentColor, 2);
      %currentColorA = getWord(%currentColor, 3);   
      
      if(%field $= "colorR") 
      {
         %obj.setBlendColor(%val SPC %currentColorG SPC %currentColorB SPC %currentColorA);
      }
      else if(%field $= "colorG") 
      {
         %obj.setBlendColor(%currentColorR SPC %val SPC %currentColorB SPC %currentColorA);
      }
      else if(%field $= "colorB") 
      {
         %obj.setBlendColor(%currentColorR SPC %currentColorG SPC %val SPC %currentColorA);
      }
      else if(%field $= "colorA") 
      {
         %obj.setBlendColor(%currentColorR SPC %currentColorG SPC %currentColorB SPC %val);
      }      
   }
   else if(%field $= "adjcuedist")
   {
      %obj.updateCue(%val);
   }
   else if(%field $= "cueinterp")
   {
      $cueAngle = %val;
      %obj.updateCue();
   }
   else if(%field $= "numberoverlay")
   {
      $Game::ballNumberOverlayAlpha = %val;
   }
   else if(%field $= "frameSize") 
   {
      // get the current parent half size Y
      %parentSizeY = %obj.parentObj.base.getSizeY() / 2.0;
      
      // get the current frame size Y
      %currentY = %obj.getSizeY();
      
      // calculate half the difference of the change to size Y
      if( isObject( %obj.linkNext ) )
         %difference = (%val - %currentY);   // slide down screen
      else
         %difference = -(%val - %currentY);  // slide up screen
      %halfDifference = %difference / 2.0;
      
      // set the new frame size
      %obj.setSizeY( %val );
      
      // we are responsible for shifting the positions of the
      // other frames if we are growing or shrinking
      if( %difference > 0.0 || %difference < 0.0 )
      {
         // special case, top most frame
         if( !isObject( %obj.linkPrev ) )
         {
            // walk down the list
            %nextObj = %obj.linkNext;
            while( isObject( %nextObj ) )
            {
               // get this frame's world position Y
               %objPosY = %nextObj.getPositionY();
               
               // change this frame's world position by half of the change to size Y
               %newPosY = %objPosY + %halfDifference;
               
               // calculate this frame's new mount offset Y
               %newOffY = %newPosY / %parentSizeY;
               %nextObj.mount( %obj.parentObj.base, getWord( %nextObj.getMountOffset(), 0 ) SPC %newOffY );
               
               // move on to the next frame
               %nextObj = %nextObj.linkNext;
            }
         }
         
         // special case, bottom most frame
         else if( !isObject( %obj.linkNext ) )
         {
            // walk up the list
            %nextObj = %obj.linkPrev;
            while( isObject( %nextObj ) )
            {
               // get this frame's world position Y
               %objPosY = %nextObj.getPositionY();
               
               // change this frame's world position by half of the change to size Y
               %newPosY = %objPosY + %halfDifference;
               
               // calculate this frame's new mount offset Y
               %newOffY = %newPosY / %parentSizeY;
               %nextObj.mount( %obj.parentObj.base, getWord( %nextObj.getMountOffset(), 0 ) SPC %newOffY );
               
               // move on to the next frame
               %nextObj = %nextObj.linkPrev;
            }
         }
         
         // middle two frames
         else
         {
            // first walk down the list
            %nextObj = %obj.linkNext;
            while( isObject( %nextObj ) )
            {
               // get this frame's world position Y
               %objPosY = %nextObj.getPositionY();
               
               // change this frame's world position by half of the change to size Y
               %newPosY = %objPosY + %halfDifference;
               
               // calculate this frame's new mount offset Y
               %newOffY = %newPosY / %parentSizeY;
               %nextObj.mount( %obj.parentObj.base, getWord( %nextObj.getMountOffset(), 0 ) SPC %newOffY );
               
               // move on to the next frame
               %nextObj = %nextObj.linkNext;
            }
            
            // then walk up the list
            %nextObj = %obj.linkPrev;
            while( isObject( %nextObj ) )
            {
               // get this frame's world position Y
               %objPosY = %nextObj.getPositionY();
               
               // change this frame's world position by half of the change to size Y
               %newPosY = %objPosY - %halfDifference;
               
               // calculate this frame's new mount offset Y
               %newOffY = %newPosY / %parentSizeY;
               %nextObj.mount( %obj.parentObj.base, getWord( %nextObj.getMountOffset(), 0 ) SPC %newOffY );
               
               // move on to the next frame
               %nextObj = %nextObj.linkPrev;
            }
         }
      }
   }
   else 
   {
      error("ERROR: in interpolateHelper(): unknown %field arg:" SPC %field);
   }

}
