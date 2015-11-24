
//-----------------------------------------------------------------------------
//Copyright (c) 2015 RABBAT Studios
//-----------------------------------------------------------------------------


/// Game
$Game::CompanyName              = "RABBAT Studios";
$Game::ProductName              = "Project Evo";
/*
/// iOS
$pref::iOS::ScreenOrientation   = $iOS::constant::Landscape;
$pref::iOS::ScreenDepth		    = 32;
$pref::iOS::UseGameKit          = 0;
$pref::iOS::UseMusic            = 0;
$pref::iOS::UseMoviePlayer      = 0;
$pref::iOS::UseAutoRotate       = 1;
$pref::iOS::EnableOrientationRotation = 1;
$pref::iOS::EnableOtherOrientationRotation = 1;   
$pref::iOS::StatusBarType       = 0;
*/

/// Audio
$pref::Audio::driver = "OpenAL";
$pref::Audio::forceMaxDistanceUpdate = 0;
$pref::Audio::environmentEnabled = 0;
$pref::Audio::masterVolume   = 1.0;
$pref::Audio::channelVolume1 = 1.0;
$pref::Audio::channelVolume2 = 1.0;
$pref::Audio::channelVolume3 = 1.0;
$pref::Audio::sfxVolume = 1.0;
$pref::Audio::musicVolume = 1.0;

/// T2D
$pref::T2D::ParticlePlayerEmissionRateScale = 1.0;
$pref::T2D::ParticlePlayerSizeScale = 1.0;
$pref::T2D::ParticlePlayerForceScale = 1.0;
$pref::T2D::ParticlePlayerTimeScale = 1.0;
$pref::T2D::warnFileDeprecated = 1;
$pref::T2D::warnSceneOccupancy = 1;
$pref::T2D::imageAssetGlobalFilterMode = Bilinear;
$pref::T2D::TAMLSchema="";

/// Video
$pref::timeManagerProcessInterval = 34;
$pref::Video::appliedPref = 0;
$pref::Video::disableVerticalSync = 1;
$pref::Video::displayDevice = "OpenGL";
$pref::Video::preferOpenGL = 1;
$pref::Video::fullScreen = 0;
$pref::Video::defaultResolution = "1366 768 32";
$pref::Video::FullScreenResolution = "1280 720 32";
$pref::Video::windowedResolution = "1280 720 32";
//$pref::Video::windowedRes = "1280 720 32";
$pref::OpenGL::gammaCorrection = 0.5;

/// Fonts.
$Gui::fontCacheDirectory = expandPath( "^AppCore/fonts" );
