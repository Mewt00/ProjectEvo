datablock t2dBaseDatablock(flyToCenter)
{
   defaultMode = $ANIM_MODE_ABS;
   numKeyframes = 5;
   //Key format = "timeDeltaMs:valueString"
   key[0] = "155:1.5 -1.5";
   key[1] = "25:-1.5 -1.5";
   key[2] = "25:-1.5 1.5";
   key[3] = "25:1.5 1.5";
   key[4] = "25:0 0";

   onAnimEnd="";
};

datablock t2dBaseDatablock(fadeAway)
{
   defaultMode = $ANIM_MODE_ABS;
   numKeyframes = 1;
   //Key format = "timeDeltaMs:valueString"
   key[0] = "400:0";

   onAnimEnd="";
};