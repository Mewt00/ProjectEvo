$managedDatablockSet = new SimSet() {
   canSaveDynamicFields = "1";
      setType = "Datablocks";

   new t2dImageMapDatablock(blankImageMap) {
      imageName = "~/data/images/blank.png";
      imageMode = "FULL";
      frameCount = "-1";
      filterMode = "SMOOTH";
      filterPad = "0";
      preferPerf = "1";
      cellRowOrder = "1";
      cellOffsetX = "0";
      cellOffsetY = "0";
      cellStrideX = "0";
      cellStrideY = "0";
      cellCountX = "-1";
      cellCountY = "-1";
      cellWidth = "0";
      cellHeight = "0";
      preload = "1";
      allowUnload = "0";
   };
};
