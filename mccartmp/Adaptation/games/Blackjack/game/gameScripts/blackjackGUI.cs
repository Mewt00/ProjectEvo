$Font::DefaultFont = "Arial Black";

$Buttons::DefaultSize = "30 25";
$Buttons::FatterSize = "36 25";
$Buttons::SmallSize = "20 15";

function initMenu(%scenegraph)
{
   
   
   %loanBtn = new t2dSceneObject(loanButton) {
      scenegraph = %scenegraph;
      class = AnimatedButton;
      superClass = ButtonBase;
      position = "200 46";
      size = "110 20";
      baseImage = moneyBarImageMap;
      //overlayImage = gameMenuEquipOverlay;
      fontName = $Font::DefaultFont;
      fontColor = "1 1 1 1";
      fontOffset = "0 -0.15";
      fontSize = "6";
      disabled = true;
      text = "Way to not set the text";
      command = "getLoan();";
   };
   
   
   
   
   %standBtn = new t2dSceneObject(standButton) {
      scenegraph = %scenegraph;
      class = AnimatedButton;
      superClass = ButtonBase;
      position = "83 64";
      size = $Buttons::DefaultSize;
      baseImage = gameMenuButtonImage;
      //overlayImage = gameMenuEquipOverlay;
      fontName = $Font::DefaultFont;
      fontColor = "0 0 0 1";
      fontOffset = "0.05 0";
      fontSize = "6";
      text = "STAND";
      command = "SceneWindow2D.getSceneGraph().tableControl.standActiveHand();";
   };
   
   %standBtn.setCollisionPolyCustom( 4, "-0.56 -0.57 0.60 -0.57 0.59 0.59 -0.56 0.59" );
   %standBtn.setCollisionActive( false, true );
   
   
   %hitBtn = new t2dSceneObject(hitButton) {
      scenegraph = %scenegraph;
      class = AnimatedButton;
      superClass = ButtonBase;
      position = "58 64";
      size = $Buttons::DefaultSize;
      baseImage = gameMenuButtonImage;
      //overlayImage = gameMenuEquipOverlay;
      fontName = $Font::DefaultFont;
      fontColor = "0 0 0 1";
      fontOffset = "0.05 0";
      fontSize = "6";
      text = "HIT";
      command = "SceneWindow2D.getSceneGraph().tableControl.hitActiveHand();";
   };
   
   %hitBtn.setCollisionPolyCustom( 4, "-0.56 -0.57 0.60 -0.57 0.59 0.59 -0.56 0.59" );
   %hitBtn.setCollisionActive( false, true );
   
   
   
   %dealBtn = new t2dSceneObject(dealButton) {
      scenegraph = %scenegraph;
      class = AnimatedButton;
      superClass = ButtonBase;
      position = "33 64";
      size = $Buttons::DefaultSize;
      baseImage = gameMenuButtonImage;
      //overlayImage = gameMenuEquipOverlay;
      fontName = $Font::DefaultFont;
      fontColor = "0 0 0 1";
      fontOffset = "0.05 0";
      fontSize = "6";
      text = "DEAL";
      command = "";
   };
   
   %dealBtn.setCollisionPolyCustom( 4, "-0.56 -0.57 0.60 -0.57 0.59 0.59 -0.56 0.59" );
   %dealBtn.setCollisionActive( false, true );
   
   
   %quickBetBtn = new t2dSceneObject(quickBetButton) {
      scenegraph = %scenegraph;
      class = AnimatedButton;
      superClass = ButtonBase;
      position = "5 64";
      size = $Buttons::FatterSize;
      baseImage = gameMenuButtonImage;
      //overlayImage = gameMenuEquipOverlay;
      fontName = $Font::DefaultFont;
      fontColor = "0 0 0 1";
      fontOffset = "0.05 0";
      fontSize = "6";
      text = "QUICK BET\n$" @ SceneWindow2D.getSceneGraph().getBetAmount();
      command = ""; //defined further down
   };
   
   %quickBetBtn.setCollisionPolyCustom( 4, "-0.56 -0.57 0.60 -0.57 0.59 0.59 -0.56 0.59" );
   %quickBetBtn.setCollisionActive( false, true );
   
   
   %changeBetBtn = new t2dSceneObject(changeBetButton) {
      scenegraph = %scenegraph;
      class = AnimatedButton;
      superClass = ButtonBase;
      position = "-23 64";
      size = $Buttons::DefaultSize;
      baseImage = gameMenuButtonImage;
      //overlayImage = gameMenuEquipOverlay;
      fontName = $Font::DefaultFont;
      fontColor = "0 0 0 1";
      fontOffset = "0.05 0";
      fontSize = "6";
      text = "CHANGE\nBET";
      command = "raiseBetMenu();";
   };
   
   
   %changeBetBtn.setCollisionPolyCustom( 4, "-0.56 -0.57 0.60 -0.57 0.59 0.59 -0.56 0.59" );
   %changeBetBtn.setCollisionActive( false, true );
   
   
   %closeBetBtn = new t2dSceneObject(closeBetButton) {
      scenegraph = %scenegraph;
      class = AnimatedButton;
      superClass = ButtonBase;
      position = "-23 89";
      size = $Buttons::DefaultSize;
      baseImage = gameMenuButtonImage;
      //overlayImage = gameMenuEquipOverlay;
      fontName = $Font::DefaultFont;
      fontColor = "0 0 0 1";
      fontOffset = "0.05 0";
      fontSize = "6";
      text = "CLOSE";
      command = "lowerBetMenu();";
      disabled = true;
   };
   
   %closeBetBtn.setCollisionPolyCustom( 4, "-0.56 -0.57 0.60 -0.57 0.59 0.59 -0.56 0.59" );
   %closeBetBtn.setCollisionActive( false, true );
   
   
   //the bet menu is needed because all the rest of the elements are going to be mounted to it
   // or reference it in commands for moving
   if( !isObject(betMenu) )
   {
      error(" There is no betMenu object!" );
      return;
   }
   
   betMenu.up = false;
   
   
   
   %quickBetBtn.command = "SceneWindow2D.getSceneGraph().spawnHand();"
                          SPC "if("@%closeBetBtn.getID()@".getEnabled()){eval(" @ %closeBetBtn.getID() @ ".command);}"
                          SPC "blackjackFSM_Singleton.applyNewBetList(blackjackFSM_Singleton.calculateBetList());";
   %dealBtn.command =  "SceneWindow2D.getSceneGraph().tableControl.initialBetsPlaced = true;"
                       SPC "if("@%closeBetBtn.getID()@".getEnabled()){eval(" @ %closeBetBtn.getID() @ ".command);}";
   
   
   //echo("%closeBetBtn = <"@%closeBetBtn.command@">");
   
   
   
   
   %betSelectionList = new t2dSceneObject(betSelectionList) 
   {
      scenegraph              = %sceneGraph;
      class                   = SelectionListText;
      superClass              = SelectionList;
      position                = betMenu.getPosition();
      
      //command is called every time the shown object changes - the command can use %newValue or %newText
      command = "SceneWindow2D.getSceneGraph().setBetAmount(%newValue);"
               SPC %quickBetBtn.getID() @ ".setText( \"QUICK BET\\n$\" @ SceneWindow2D.getSceneGraph().getBetAmount());";
      
      //-----------------------------------------------------------------------------
      // Next/Previous Button Appearance ( Shared Attributes )
      //-----------------------------------------------------------------------------

      // The size of the prev/next buttons
      buttonSize              = "10 10";
      // The base image (background) to be used on the Next/Prev item buttons
      buttonBaseImage         = gameMenuButtonImage;
      // The font to be used on prev/next buttons (if any text is associated with them)
      buttonFontName          = $Font::DefaultFont;
      // The font color to be used on prev/next buttons (if any text is associated with them)
      buttonFontColor         = "0 0 0 1";
      // The font mount position to be used on prev/next buttons (if any text is associated with them)
      buttonFontOffset        = "0 0";
      // The font size to be used on prev/next buttons (if any text is associated with them)
      buttonFontSize          = "4";
      
      //-----------------------------------------------------------------------------
      // Previous Button Appearance 
      //-----------------------------------------------------------------------------

      // The text to be placed on the 'previous item' button.  ("" means no text)
      prevButtonText          = ""; 
      // The overlay image to be placed on top of the buttonBaseImage
      // ( << graphic or the like - generally with no button text )
      prevButtonOverlayImage  = leftArrowImageMap;
      // Position on the base to mount the previous button ( "-0.9 0" Default )
      prevButtonMountPos      = "-4 0";
      // The cell of the imagemap to use (defaults to 0)
      prevButtonImageIndex    = 0;

      //-----------------------------------------------------------------------------
      // Next Button Appearance
      //-----------------------------------------------------------------------------

      // The text to be placed on the 'next item' button.  ("" means no text)
      nextButtonText          = ""; 
      // The overlay image to be placed on top of the buttonBaseImage
      // ( >> graphic or the like - generally with no button text )
      nextButtonOverlayImage  = rightarrowImageMap;
      // Position on the base to mount the previous button ( "0.9 0" Default )
      nextButtonMountPos      = "4.2 0";
      // The cell of the imagemap to use (defaults to 0)
      nextButtonImageIndex    = 0;
      
      //-----------------------------------------------------------------------------
      // Text Display Attributes
      //-----------------------------------------------------------------------------
      
      // Position to mount the text display on the base ( "0 0" Default )
      textMountPos            = "0 0";
      // The font to be used on item text
      textFontName          = $Font::DefaultFont;
      // The font color to be used on item text
      textFontColor         = "1 1 1 1";
      // The font mount position to be used on item text
      textFontOffset        = "0 0";
      // The font size to be used on item text
      textFontSize          = "10";
   };
   
   %betSelectionList.add("$500", 500);
   
   %betSelectionList.mount( betMenu, "0 -0.6");
   
   
   
   
   
   %betBtn = new t2dSceneObject(betButton) {
      scenegraph = %scenegraph;
      class = AnimatedButton;
      superClass = ButtonBase;
      position = "0 0";
      size = $Buttons::SmallSize;
      baseImage = gameMenuButtonImage;
      //overlayImage = gameMenuEquipOverlay;
      fontName = $Font::DefaultFont;
      fontColor = "0 0 0 1";
      fontOffset = "0.05 0";
      fontSize = "6";
      text = "BET";
      command = "SceneWindow2D.getSceneGraph().spawnHand();"
                SPC "blackjackFSM_Singleton.applyNewBetList(blackjackFSM_Singleton.calculateBetList());";
   };
   
   %betBtn.setCollisionPolyCustom( 4, "-0.56 -0.57 0.60 -0.57 0.59 0.59 -0.56 0.59" );
   %betBtn.setCollisionActive( false, true );
   %betBtn.mount( betMenu, "0 -0.05", 0, false, true, true, false);
   
   
   
   
   
   
   
   
   
   
   
   
   
   
   //the rest of the buttons are examples
   return;
   
   
   //////////////////////////////////////////////////////////////////////////////
   //---------------------------------------------------------------------------
   //---------------------------------------------------------------------------
   //////////////////////////////////////////////////////////////////////////////
   
   %MyCheckBox = new t2dSceneObject() 
   {
      scenegraph              = %sceneGraph;
      class                   = CheckBox;
      position = "-10 -15";

      image                    = checkBoxImageMap;
      imageSize                = "5 5";
      checkedIndex             = 0;
      uncheckedIndex           = 1;
      imageMountPos            = "0 0";

      fontOffset            = "0.6 0";
      fontName          = "Comic Sans MS";
      fontColor         = "1 1 1 1";
      fontSize          = "3";
      text              = "Cow";
      textAlign         = "Left";
   };

   //////////////////////////////////////////////////////////////////////////////
   //---------------------------------------------------------------------------
   //---------------------------------------------------------------------------
   //////////////////////////////////////////////////////////////////////////////
   
   %btn3 = new t2dSceneObject() {
      scenegraph   = %scenegraph;
      class        = SpriteButton;
      superClass   = ButtonBase;
      position     = "-10 20";
      size         = "4 4";
      text           = "";
      overlayImage   = arrowIcons;
      imageIndex     = 0;
      hoverImageIndex= 2;
      clickImageIndex= 4;
      baseImage      = "";
      noAnimate      = false;
      immediateEval  = true;
      layer          = 9;
      
      command        = ""; //"ui_gameSkinDlg.push(\"" @ %this @ ".hideMenu();\");";
   };
   
   
   %btn3.setCollisionPolyCustom( 4, "-0.56 -0.57 0.60 -0.57 0.59 0.59 -0.56 0.59" );
   %btn3.setCollisionActive( false, true );
   
   
   //////////////////////////////////////////////////////////////////////////////
   //---------------------------------------------------------------------------
   //---------------------------------------------------------------------------
   //////////////////////////////////////////////////////////////////////////////
   
   
   %MySelectionList = new t2dSceneObject() 
   {
      scenegraph              = %sceneGraph;
      class                   = SelectionListText;
      superClass              = SelectionList;
      position                = "0 30";
      //-----------------------------------------------------------------------------
      // Next/Previous Button Appearance ( Shared Attributes )
      //-----------------------------------------------------------------------------

      // The size of the prev/next buttons
      buttonSize              = "10 10";
      // The base image (background) to be used on the Next/Prev item buttons
      buttonBaseImage         = gameMenuButtonImage;
      // The font to be used on prev/next buttons (if any text is associated with them)
      buttonFontName          = "Comic Sans MS";
      // The font color to be used on prev/next buttons (if any text is associated with them)
      buttonFontColor         = "1 1 1 1";
      // The font mount position to be used on prev/next buttons (if any text is associated with them)
      buttonFontOffset        = "0 0";
      // The font size to be used on prev/next buttons (if any text is associated with them)
      buttonFontSize          = "4";
      
      //-----------------------------------------------------------------------------
      // Previous Button Appearance 
      //-----------------------------------------------------------------------------

      // The text to be placed on the 'previous item' button.  ("" means no text)
      prevButtonText          = ""; 
      // The overlay image to be placed on top of the buttonBaseImage
      // ( << graphic or the like - generally with no button text )
      prevButtonOverlayImage  = gameMenuHelpOverlay;
      // Position on the base to mount the previous button ( "-0.9 0" Default )
      prevButtonMountPos      = "-3.0 0";
      // The cell of the imagemap to use (defaults to 0)
      prevButtonImageIndex    = 0;

      //-----------------------------------------------------------------------------
      // Next Button Appearance
      //-----------------------------------------------------------------------------

      // The text to be placed on the 'next item' button.  ("" means no text)
      nextButtonText          = ""; 
      // The overlay image to be placed on top of the buttonBaseImage
      // ( >> graphic or the like - generally with no button text )
      nextButtonOverlayImage  = gameMenuOptionOverlay;
      // Position on the base to mount the previous button ( "0.9 0" Default )
      nextButtonMountPos      = "3.0 0";
      // The cell of the imagemap to use (defaults to 0)
      nextButtonImageIndex    = 0;
      
      //-----------------------------------------------------------------------------
      // Text Display Attributes
      //-----------------------------------------------------------------------------
      
      // Position to mount the text display on the base ( "0 0" Default )
      textMountPos            = "0 0";
      // The font to be used on item text
      textFontName          = "Comic Sans MS";
      // The font color to be used on item text
      textFontColor         = "1 1 1 1";
      // The font mount position to be used on item text
      textFontOffset        = "0 0";
      // The font size to be used on item text
      textFontSize          = "4";
   };
   
   %MySelectionList.add("A Monkey", 1);
   %MySelectionList.add("A Cow", 2);
   %MySelectionList.add("Zombie Lord", 3);
   
   
   //////////////////////////////////////////////////////////////////////////////
   //---------------------------------------------------------------------------
   //---------------------------------------------------------------------------
   //////////////////////////////////////////////////////////////////////////////
   
   %MySlider = new t2dSceneObject() 
   {
      scenegraph              = %sceneGraph;
      class                   = Slider;
      
      position                = "0 15";
      size                    = "10 5";
      
      value                   = "0.3";

      //-----------------------------------------------------------------------------
      // Image Display Attributes
      //-----------------------------------------------------------------------------
      
      // Image map to use for check/unchecked states
      baseImage                = sliderBaseImageMap;

      //-----------------------------------------------------------------------------
      // Thumb Image Attributes
      //-----------------------------------------------------------------------------
      
      // Image for the sliding part itself
      thumbImage            = sliderThumbImageMap;
      // The size of the slidable part
      thumbSize          = "1 3";
   };
   
   //////////////////////////////////////////////////////////////////////////////
   //---------------------------------------------------------------------------
   //---------------------------------------------------------------------------
   //////////////////////////////////////////////////////////////////////////////
   
}





function raiseBetMenu()
{
   if( betMenu.up )
      return;
      
   blackjackFSM_Singleton.applyNewBetList(blackjackFSM_Singleton.calculateBetList());  //makes sure that bet options are logical
   changeBetButton.setEnabled(false);  //don't let them click the button more than once - it'll screw up the relative animation
   AnimationManager.playAnimationFrame(changeBetButton,  "300:-23 89", "position", $ANIM_MODE_ABS, "" );   //send the button down
   AnimationManager.playAnimationFrame(moneyBar,      "200:0 -19", "position", $ANIM_MODE_REL, "" );   //send the moneyBar down
   AnimationManager.playAnimationFrame(betMenu,       "150:0 -40", "position", $ANIM_MODE_REL, "betMenu.up = true;" );   //bring the menu up
   AnimationManager.playAnimationFrame(closeBetButton,   "200:-23 64", "position", $ANIM_MODE_ABS, closeBetButton.getID() @ ".setEnabled(true);" ); //bring the close menu button up and when it's finished, enable the button
}

function lowerBetMenu()
{   
   if( !betMenu.up )
      return;
   
   closeBetButton.setEnabled(false);
   AnimationManager.playAnimationFrame( betMenu,      "150:0 40", "position", $ANIM_MODE_REL, "betMenu.up = false; changeBetButton.setEnabled(false);" );
   AnimationManager.playAnimationFrame( moneyBar,     "200:0 19", "position", $ANIM_MODE_REL, "" );
   AnimationManager.playAnimationFrame( closeBetButton,  "200:-23 89", "position", $ANIM_MODE_ABS, "" );
   AnimationManager.playAnimationFrame( changeBetButton, "200:-24 64", "position", $ANIM_MODE_ABS, changeBetButton.getID() @ ".setEnabled(true);" );
}


$sayingCount = 28;
//Loan Sayings
$loanSaying[0] = "Sell Kidneys to Stranger";
$loanSaying[1] = "Auction Off Child's Teddy Bear";
$loanSaying[2] = "Withdraw Kids' College Fund";
$loanSaying[3] = "Rob a Bank";
$loanSaying[4] = "Call Mafia Contact";
$loanSaying[5] = "Call the Neighborhood Pushover";
$loanSaying[6] = "Slide Some Fake Chips onto the Table";
$loanSaying[7] = "Wink Provacatively at Rich Gamblers";
$loanSaying[8] = "Act Drunk While Stealing Wallets";
$loanSaying[9] = "Whip Out Your Gun and Scream";
$loanSaying[10] = "Eat Mayonnaise for Cash";
$loanSaying[11] = "Yell \"Fire!\" and Yoink Some Chips";
$loanSaying[12] = "Discuss Content Packs with Tim A.";
$loanSaying[13] = "Eat Fried Ranch Before Awed Spectators";
$loanSaying[14] = "Sell the Clothes Off Your Back";
$loanSaying[15] = "Put All Your Games on GGE";
$loanSaying[16] = "Stand Up and Yell Like Justin";
$loanSaying[17] = "Offer to Install Christmas Lighting";
$loanSaying[18] = "Write a Book: \"The Dangers of Gambling\"";
$loanSaying[19] = "Create a \"Make Game\" Button";
$loanSaying[20] = "Use TGB to Make a Niche Game";
$loanSaying[21] = "Port Random Things to Linux";
$loanSaying[22] = "Sell Open Source Software on Ebay";
$loanSaying[23] = "Learn to Print Money";
$loanSaying[24] = "Sell Your Tendons to Programmers";
$loanSaying[25] = "Step 1:Summon Zombies Step 2: ?? Step 3:Profit";
$loanSaying[26] = "Extort a GarageGames intern";
$loanSaying[27] = "Invest in GG slave labor (internships)";

function showLoanButton()
{
   
   /*for(%i = 0; %i < $sayingCount; %i++)
   {
      echo("["@%i@"]: "@$loanSaying[%i]);
   } */ 
   
   
   %random = mFloor(getRandom(0,$sayingCount-1));
   %checks = 0;
   
   while( getSubStr($loanSaying[%random], 0, 1) $= "X" )
   {  
      
      if( %checks >= $sayingCount )
         AllowAllSayings();
      if( %checks > 100 )
         %random = 0;
      
      %random = mFloor(getRandom(0,$sayingCount-1));
      %checks++;
   }
   loanButton.SetText($loanSaying[%random]);
   $loanSaying[%random] = "X"@$loanSaying[%random];
   
   AnimationManager.playAnimationFrame( loanButton, "200:-170 0", "position", $ANIM_MODE_REL, "loanButton.setEnabled(true); loanButton.out = true;" );   
}

function AllowAllSayings()
{
   echo("allowing all sayings");
   for(%i = 0; %i < $sayingCount; %i++)
   {
      if( getSubStr($loanSaying[%i], 0, 1) $= "X" )
         $loanSaying[%i] = getSubStr($loanSaying[%i], 1, 100);
   }
   /*for(%i = 0; %i < $sayingCount; %i++)
   {
      echo("["@%i@"]: "@$loanSaying[%i]);
   } */  
}

function hideLoanButton()
{
   loanButton.out = false;
   loanButton.setEnabled(false);
   AnimationManager.playAnimationFrame( loanButton, "200:170 0", "position", $ANIM_MODE_REL, "" );   
}

function getLoan()
{
   hideLoanButton();
   %loan = mFloor(getRandom(100, 6000));
   blackjackFSM_Singleton.updateCredit(%loan);
   SceneWindow2D.getScenegraph().resetWallet();
   
   %newBetList = blackjackFSM_Singleton.calculateBetList();
   echo(%newBetList);
   blackjackFSM_Singleton.applyNewBetList(%newBetList);
}