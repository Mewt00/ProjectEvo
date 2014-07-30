// --------------------------------------------------------------------
// winningEffect()
//
// This will trigger the winning effect
// --------------------------------------------------------------------
function SceneWindow2D::fireOffFireworks(%this, %fireWorkCount)
{
   // specify a count for the ammount of fireworks
   if(%fireWorkCount $= "")
      %fireWorkCount = 4;
   
   // specify the ammount of sparks to explode
   %explodeCount = 10;
   
   // loop through all of our fireworks to be shot off
   for(%i=0;%i<%fireWorkCount;%i++)
   {
      // get a randome time
      %time = getRandom(1.5,2);
      // create a scene object to mount the main firework particle effect to
      %fireWork = new t2dSceneObject("fireWork" @ %i) 
      { 
         sceneGraph = SceneWindow2D.getScenegraph(); 
         class = "fireWork";
      };  
      %fireWork.setPosition("0 80");
      %fireWork.setLinearVelocityPolar(getRandom(-20, 20), getRandom(130,165));
      %firework.setConstantForcePolar( 180, 35 );
      %fireWork.createmainFireWork();
      %fireWork.setLayer(1);
      %fireWork.schedule(%time * 500, "explodeFireWork", %explodeCount);  
   }
}

// --------------------------------------------------------------------
// fireWork::createMainFireWork()
//
// This function will attach the main firework
// --------------------------------------------------------------------
function fireWork::createMainFireWork(%this)
{
   // create an effect
   %fireWork = new t2dParticleEffect() { scenegraph = SceneWindow2D.getScenegraph(); };
   // load the effect
   %fireWork.loadEffect("~/data/particles/mainfirework.eff");
   %fireWork.setLayer(0);
 
   //find the emitter to be scaled
   %fireWorkEmitter = %fireWork.findEmitterObject( "flickertrail" );
   //select the particle color life graph
   %fireWorkEmitter.selectGraph( "red_life" );
   %fireWorkEmitter.clearDataKeys();
   %fireWorkEmitter.addDataKey( 0.0, getRandom(0,1));
   %fireWorkEmitter.selectGraph( "blue_life" );
   %fireWorkEmitter.clearDataKeys();
   %fireWorkEmitter.addDataKey( 0.0, getRandom(0,1));
   %fireWorkEmitter.selectGraph( "green_life" );
   %fireWorkEmitter.clearDataKeys();
   %fireWorkEmitter.addDataKey( 0.0, getRandom(0,1));
   
   // set up the effect to be destroyed upon completion and play the effect
   %fireWork.mount(%this);
   %fireWork.playEffect(true); 
   %fireWork.setLayer(1);
   
   // store this main firework effect
   %this.fireWork = %fireWork;  
}

// --------------------------------------------------------------------
// fireWork::explodeFireWork()
//
// This function will explode the main firework and spawn a bunch
// of little spark particles
// --------------------------------------------------------------------
function fireWork::explodeFireWork(%this, %explodeCount)
{
   // stop and safe delete the main effect
   %this.fireWork.stopEffect(true);
   %this.fireWork.schedule(1000, "safeDelete");
   
   // this will determine whether the firework is white, multi-colored, or a solid other color
   %white = getRandom(0, 5);
      
   if(%white <= 3)
   {
      %random = true;
   } else if(%white == 4)
   {
      // this will give us a white color
      %red = 1;
      %green = 1;
      %blue = 1;
   } else
   {
      // this will determine what color it will be set to if a solid color is 
      // randomly chosen
      %whichColor = getRandom(0, 2);
      
      if(%whichColor == 0)
      {
         %red = 1;
         %green = 0;
         %blue = 0;
      } else if(%whichColor == 1)
      {
         %red = 0;
         %green = 1;
         %blue = 0;
      } else
      {
         %red = 0;
         %green = 0;
         %blue = 1;
      }
   }
   
   // loop through each of our trailing sparks
   for(%i=0;%i<%explodeCount;%i++)
   {
      %firework = new t2dParticleEffect() { scenegraph = SceneWindow2D.getScenegraph(); };
		    %firework.loadEffect("~/data/particles/firework.eff");
      
      // if we have this set to multi-colored then get random colors
      if(%random)
      {
         %red = getRandom(0, 1);         
         %green = getRandom(0, 1);
         %blue = getRandom(0, 1);
      }
      
      //find the emitter to be scaled
      %fireWorkEmitter = %fireWork.findEmitterObject( "startrail" );
      //select the particle color life graph
      %fireWorkEmitter.selectGraph( "red_life" );
      %fireWorkEmitter.clearDataKeys();
      %fireWorkEmitter.addDataKey( 0.0, %red);
      %fireWorkEmitter.selectGraph( "blue_life" );
      %fireWorkEmitter.clearDataKeys();
      %fireWorkEmitter.addDataKey( 0.0, %blue);
      %fireWorkEmitter.selectGraph( "green_life" );
      %fireWorkEmitter.clearDataKeys();
      %fireWorkEmitter.addDataKey( 0.0, %green);
      
      // if we have this set to multi-colored then get random colors
      if(%random)
      {
         %red = getRandom(0, 1);         
         %green = getRandom(0, 1);
         %blue = getRandom(0, 1);
      }
      
      //find the emitter to be scaled
      %fireWorkEmitter = %fireWork.findEmitterObject( "flickertrail" );
      //select the particle color life graph
      %fireWorkEmitter.selectGraph( "red_life" );
      %fireWorkEmitter.clearDataKeys();
      %fireWorkEmitter.addDataKey( 0.0, %red);
      %fireWorkEmitter.selectGraph( "blue_life" );
      %fireWorkEmitter.clearDataKeys();
      %fireWorkEmitter.addDataKey( 0.0, %blue);
      %fireWorkEmitter.selectGraph( "green_life" );
      %fireWorkEmitter.clearDataKeys();
      %fireWorkEmitter.addDataKey( 0.0, %green);
         
      // set the sparks position, kill mode, play it, then launch it in a
      // random direction at a semi-random speed with a constant downward force
		    %firework.setPosition( %this.getPosition() );
		    %firework.setEffectLifeMode( KILL, 1.0 );
		    %firework.playEffect();
		    %firework.setLinearVelocityPolar( getRandom(360), getRandom(15,40) );
		    %firework.setConstantForcePolar( 180, 100 );
		    %firework.setLayer(1);
   }
}
