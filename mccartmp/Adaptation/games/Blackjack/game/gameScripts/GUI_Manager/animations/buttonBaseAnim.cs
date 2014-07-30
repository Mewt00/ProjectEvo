datablock t2dBaseDatablock(buttonOverAnim)
{
   defaultMode = $ANIM_MODE_REL;    
   numKeyframes = 2;
   //Key format = "timeDeltaMs:valueString"
   key[0] = "100:4 4";
   key[1] = "100:-2 -2";
   onAnimEnd="";
};

datablock t2dBaseDatablock(buttonLeaveAnim)
{
   defaultMode = $ANIM_MODE_REL;     
   numKeyframes = 2;
   //Key format = "timeDeltaMs:valueString"
   key[0] = "100:-4 -4";
   key[1] = "100:2 2";
   onAnimEnd="";
};


datablock t2dBaseDatablock(buttonWiggleAnim)
{
   defaultMode = $ANIM_MODE_REL;
   numKeyframes = 8;
   //Key format = "timeDeltaMs:valueString"
   key[0] = "25:5 -5";
   key[1] = "25:-5 -5";
   key[2] = "25:-5 5";
   key[3] = "50:5 5";
   key[4] = "25:5 5";
   key[5] = "25:-5 5";
   key[6] = "25:-5 -5";
   key[7] = "50:5 -5";

   onAnimEnd="";
};
