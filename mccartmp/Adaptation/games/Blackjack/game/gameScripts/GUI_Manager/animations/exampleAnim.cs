//This is just a rough template of some datablocks that collectively make up
//an animation...

//Make sure to name all your animaton files like "*Anim.cs"

//Here's the calls you would make in script to play the animation
//AnimationManager.playAnimation(dummySprite1, "waynesWorld:size", $ANIM_MODE_ABS); 
//AnimationManager.playAnimation(dummySprite1, "foo:position", $ANIM_MODE_REL);
//AnimationManager.playAnimation(dummySprite1, "pulse:alpha", "");
//AnimationManager.playAnimation(dummySprite1, "spin:rotation", $ANIM_MODE_REL);

datablock t2dBaseDatablock(foo)
{
   defaultMode = $ANIM_MODE_REL;
   
   numKeyframes = 4;
   
   //Key format = "timeDeltaMs:valueString:sinArgA:sinArgB"
   key[0] = "500:-30 0:0:50";
   key[1] = "500:0 -30:50:100";
   key[2] = "500:30 0:50:100";
   key[3] = "500:0 30:50:100";
   
   onAnimEnd = "echo(\"Done with anim!\");";
};

datablock t2dBaseDatablock(waynesWorld)
{
   defaultMode = $ANIM_MODE_ABS;
   
   numKeyframes = 3;
   
   key[0] = "500:5 5:0:50";
   key[1] = "500:60 60:50:100";
   key[2] = "1000:10 10:50:100";
   
   onAnimEnd = "echo(\"Done with anim!\");";
};

datablock t2dBaseDatablock(pulse)
{
   defaultMode = $ANIM_MODE_ABS;
   
   numKeyframes = 2;
   
   key[0] = "500:100:0:100";
   key[1] = "500:255:0:100";
   
   onAnimEnd = "echo(\"Done with anim!\");";
};

datablock t2dBaseDatablock(spin)
{
   defaultMode = $ANIM_MODE_REL;
   
   numKeyframes = 1;
   
   key[0] = "2000:360:0:100";
   
   onAnimEnd = "echo(\"Done with anim!\");";
};