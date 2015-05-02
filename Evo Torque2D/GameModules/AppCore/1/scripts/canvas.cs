
//-----------------------------------------------------------------------------
//Copyright (c) 2015 Chubby Bunny, LLC
//-----------------------------------------------------------------------------


//------------------------------------------------------------------------------
// initializeCanvas
// Constructs and initializes the default canvas window.
//------------------------------------------------------------------------------
$canvasCreated = false;
function initializeCanvas(%windowName)
{
    // Don't duplicate the canvas.
    if($canvasCreated)
    {
        error("Cannot instantiate more than one canvas!");
        return;
    }
	
    videoSetGammaCorrection($pref::OpenGL::gammaCorrection);
	
	echo("After Gamma");
	//echo("Before createCanvas: " @ $pref::Video::windowedRes);

    if ( !createCanvas(%windowName) )
    {
        error("Canvas creation failed. Shutting down.");
        quit();
    }
	
	//echo("After createCanvas: " @ $pref::Video::windowedRes);

    /*//$pref::iOS::ScreenDepth = 32;

    //if ( $pref::iOS::DeviceType !$= "" )
    //{
    //    %resolution = iOSResolutionFromSetting($pref::iOS::DeviceType, $pref::iOS::ScreenOrientation);
    //}
    //else
    //{}*/

	//echo("windowedResoultion: " @ $pref::Video::windowedResolution);
	
	//If there is a defined windowedResolution in pref
    if ( $pref::Video::windowedResolution !$= "" )
	{
        //%resolution = $pref::Video::windowedRes;
		%resolution = $pref::Video::windowedResolution;
		echo("Resolution: " @ %resolution);
		//echo("Total if: " @ $pref::Video::windowedResolution);
		//echo("Total if: " @ $pref::Video::windowedRes);
	}
    else
	{
        %resolution = $pref::Video::defaultResolution;
		echo("Resolution: " @ %resolution);
	}
	
    if ($platform $= "windows" || $platform $= "macos")
    {
		echo("Fullscreen: " @ $pref::Video::fullScreen);
		if( $pref::Video::fullScreen )  
			%resolution = $pref::Video::FullScreenResolution;
		
        setScreenMode(GetWord( %resolution, 0), GetWord( %resolution,1), GetWord( %resolution,2), $pref::Video::fullScreen );
    }
    else
    {
		echo("Not Mac or Windows, disallowing fullscreen." @ $pref::Video::fullScreen);

        setScreenMode(GetWord( %resolution, 0), GetWord( %resolution,1), GetWord( %resolution,2), false );
    }
	
	/*echo("Fullscreen: " @ $pref::Video::fullScreen);
	if( $pref::Video::fullScreen )  
    {  
		%goodres = $pref::Video::FullScreenResolution;  
		setScreenMode( GetWord( %goodres, 0), GetWord( %goodres,1), GetWord( %goodres,2), true );  
		echo("Fullscreen after: " @ $pref::Video::fullScreen);
    }  
    else  
    {  
		%goodres = $pref::Video::windowedResolution;  
		setScreenMode( GetWord( %goodres, 0), GetWord( %goodres,1), GetWord( %goodres,2), false );  
    }  */

    $canvasCreated = true;
}

//------------------------------------------------------------------------------
// resetCanvas
// Forces the canvas to redraw itself.
//------------------------------------------------------------------------------
function resetCanvas()
{
    if (isObject(Canvas))
        Canvas.repaint();
}

/*
//------------------------------------------------------------------------------
// iOSResolutionFromSetting
// Helper function that grabs resolution strings based on device type
//------------------------------------------------------------------------------
function iOSResolutionFromSetting( %deviceType, %deviceScreenOrientation )
{
    // A helper function to get a string based resolution from the settings given.
    %x = 0;
    %y = 0;
    
    %scaleFactor = $pref::iOS::RetinaEnabled ? 2 : 1;

    switch(%deviceType)
    {
        case $iOS::constant::iPhone:
            if(%deviceScreenOrientation == $iOS::constant::Landscape)
            {
                %x =  $iOS::constant::iPhoneWidth * %scaleFactor;
                %y =  $iOS::constant::iPhoneHeight * %scaleFactor;
            }
            else
            {
                %x =  $iOS::constant::iPhoneHeight * %scaleFactor;
                %y =  $iOS::constant::iPhoneWidth * %scaleFactor;
            }

        case $iOS::constant::iPad:
            if(%deviceScreenOrientation == $iOS::constant::Landscape)
            {
                %x =  $iOS::constant::iPadWidth * %scaleFactor;
                %y =  $iOS::constant::iPadHeight * %scaleFactor;
            }
            else
            {
                %x =  $iOS::constant::iPadHeight * %scaleFactor;
                %y =  $iOS::constant::iPadWidth * %scaleFactor;
            }

        case $iOS::constant::iPhone5:
            if(%deviceScreenOrientation == $iOS::constant::Landscape)
            {
                %x =  $iOS::constant::iPhone5Width;
                %y =  $iOS::constant::iPhone5Height;
            }
            else
            {
                %x =  $iOS::constant::iPhone5Height;
                %y =  $iOS::constant::iPhone5Width;
            }
    }
   
    return %x @ " " @ %y;
}*/