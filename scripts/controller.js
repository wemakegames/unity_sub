"use strict";

var main = function(
    CommonUI,
    GameClient,
    DPad,
    Input,
    Misc,
    MobileHacks,
    Touch,
    AudioManager) {
  var g_client;
  var g_audioManager;

var globals = {
    debug: false,
  };
Misc.applyUrlSettings(globals);
MobileHacks.fixHeightHack();

function $(id) {
  return document.getElementById(id);
}

console.log( "Create new GameClient" );
g_client = new GameClient({
  gameId: "subbuteo",
});

function handleScore() {
};

function handleDeath() {
};

var canvas = document.getElementById('canvas');
var click = false;
var startPos;
var endPos;
var lastTime;
var newTime;


function writeMessage(message) {
  document.getElementById("debugtext").innerHTML = message;
}


canvas.addEventListener("touchstart", handleStart, false);
canvas.addEventListener("touchend", handleEnd, false);


function getTouchPos(canvas, evt) {
  var rect = canvas.getBoundingClientRect();
  return {
    x: evt.pageX - rect.left,
    y: evt.pageY - rect.top
  };
}

function handleStart(evt) {
    var touchPos = getTouchPos(canvas, evt);
    startPos = touchPos;
    lastTime = Date.now();
    click = true;
    writeMessage("DOWN");
}

function handleEnd(evt) {

  evt.preventDefault(); //prvent mouse movement
  newTime = Date.now();

  var totalTime = newTime - lastTime;
  var lastTouch = evt.changedTouches[0];

  var message = "X:  " + startPos.x + "   Y:  " + startPos.y + "      X:  " + lastTouch.pageX + "   Y:  " + lastTouch.pageY + '   Time:   ' + totalTime;
  writeMessage(message);
  g_client.sendCmd('swipe',{ platform: "touch" , startX: startPos.x, startY: startPos.y, endX: lastTouch.pageX, endY: lastTouch.pageY, duration: totalTime});
  click = false;
}



//start of implementation for mouse to test in browser

canvas.addEventListener("mousedown",function(evt){
  var mousePos = getMousePos(canvas,evt);
  startPos = mousePos;
  lastTime = Date.now();
  click = true;
  writeMessage("DOWN");

});
canvas.addEventListener("mouseup",function(evt){
  newTime = Date.now();


  var totalTime = newTime - lastTime;
  var mousePos = getMousePos(canvas,evt);
  endPos = mousePos;
  writeMessage("UP");

  var message = "X:  " + startPos.x + "   Y:  " + startPos.y + "      X:  " + endPos.x + "   Y:  " + endPos.y + '   Time:   ' + totalTime;

  writeMessage(message);

  g_client.sendCmd('swipe',{ platform: "mouse" , startX: startPos.x, startY: startPos.y, endX: endPos.x, endY: endPos.y, duration: totalTime});
  click = false;
});



function getMousePos(canvas, evt) {
  var rect = canvas.getBoundingClientRect();
  return {
    x: evt.clientX - rect.left,
    y: evt.clientY - rect.top
  };
}

  g_client.addEventListener('score', handleScore);
  g_client.addEventListener('die', handleDeath);

  //var color = Misc.randCSSColor();
  //g_client.sendCmd('setColor', { color: color });
  //**/document.body.style.backgroundColor = color;

  //g_audioManager = new AudioManager();

  var sendPad = function(e) {

  };

  CommonUI.setupStandardControllerUI(g_client, globals);
};

var test = document.getElementById('hft-content');

// Start the main app logic.
requirejs(
  [ '../../../scripts/commonui',
    '../../../scripts/gameclient',
    '../../../scripts/misc/dpad',
    '../../../scripts/misc/input',
    '../../../scripts/misc/misc',
    '../../../scripts/misc/mobilehacks',
    '../../../scripts/misc/touch',
    '../../../3rdparty/jsfx/audio',
  ],
  main
);

