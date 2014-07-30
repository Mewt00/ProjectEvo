function pathLayer::onLevelLoaded(%this, %sg)
{
   $pathGrid = new pathGrid2D() { sceneGraph = %sg; };
   
   $pathgrid.setdiagonalmovement(true);
   $pathGrid.setTileLayer(%this);
   $pathGrid.currentTileLayer = %this;
   
}

function pathGrid2d::setTileWeightByWorldCoord(%this, %xCoord, %yCoord, %newWeight, %notify)
{
   // this function requires that the developer ensure the xCoord and yCoord are within the bounds
   // of the tileMap itself (and therefore the path grid) before applying the weight change
   
   %curTile = %this.currentTileLayer.pickTile( %xCoord, %yCoord);
   %this.currentTileLayer.setTileCustomData( getWord(%curTile, 0),
                                             getWord(%curTile, 1),
                                             %newWeight );
   %this.setWeight(getWord(%curTile, 0),
                   getWord(%curTile, 1),
                   %newWeight );

  if (%notify == true)
  {
     // the caller wants us to handle notifying each aStarActor of the change
     notifyActors(%this, "WorldChange");
  }
}
   
function pathGrid2d::setTileWeightByTileIndex(%this, %xTile, %yTile, %newWeight, %notify)
{
   // Alternate method for setting weights if you already have the tile square grid coordinates
   // available.
   
   %this.currentTileLayer.setTileCustomData( %xTile, %yTile, %newWeight);
   %this.setWeight(%xTile, %yTile, %newWeight);
  if (%notify == true)
  {
     // the caller wants us to handle notifying each aStarActor of the change
     notifyActors(%this, "WorldChange");
  }
}
   

function t2dTileLayer::getTileWorldPosition(%this, %tileXPosition, %tileYPosition)
{
   // determine if we were passed a word-list or x/y coords
   if(getWordCount(%tileXPosition) > 1)
   {
      %tilex = getWord(%tileXPosition, 0);
      %tiley = getWord(%tileXPosition, 1);
   }
   else
   {
      %tilex = %tileXPosition;
      %tiley = %tileYPosition;
   }
   
   // %buff just stores stuff real quick -- I like 'getSize' over 'getSizeX', personal preference
   %buff = %this.getTileSize();
   %tsx = getWord(%buff, 0);
   %tsy = getWord(%buff, 1);
   
   %buff = %this.getTileCount();
   %tcx = getWord(%buff, 0);
   %tcy = getWord(%buff, 1);
   
   %p = %this.getPosition();
   %px = getWord(%p, 0);
   %py = getWord(%p, 1);
   
   // get the top-left
   %tlx = -(((%tcx * %tsx) / 2) + (%tsx / 2));
   %tly = -(((%tcy * %tsy) / 2) + (%tsy / 2));
   
   // determine tile world position
   %wx = %px + (%tlx + (%tsx * (%tilex++)));
   %wy = %py + (%tly + (%tsy * (%tiley++)));
   return %wx SPC %wy;
}

