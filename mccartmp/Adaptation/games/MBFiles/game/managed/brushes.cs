$brushSet = new SimSet() {
   canSaveDynamicFields = "1";
   internalName = "Brushes";
      setType = "Brushes";

   new ScriptObject(blankBrush1) {
      canSaveDynamicFields = "1";
      class = "TileBrush";
         collision = "-1";
         customData = "0";
         displayName = "blankBrush1";
         FlipX = "-1";
         FlipY = "-1";
         frame = "0";
         image = "blankImageMap";
         script = "None";
   };
   new ScriptObject(testBrush) {
      canSaveDynamicFields = "1";
      class = "TileBrush";
         collision = "0";
         collisionPoly = "-0.0344828 -0.603448 -0.206897 -0.12069";
         customData = "0";
         displayName = "testBrush";
         FlipX = "-1";
         FlipY = "-1";
         frame = "0";
         image = "None";
         script = "None";
   };
   new ScriptObject(testBrush1) {
      canSaveDynamicFields = "1";
      class = "TileBrush";
         collision = "0";
         collisionPoly = "-0.0344828 -0.603448 -0.206897 -0.12069";
         customData = "0";
         displayName = "testBrush1";
         FlipX = "-1";
         FlipY = "-1";
         frame = "0";
         image = "puzzleGem_3ImageMap";
         script = "None";
   };
};
