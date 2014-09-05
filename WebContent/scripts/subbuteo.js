"use strict";

var main = function(
    GameClient,
    AudioManager,
    DPad,
    ExampleUI,
    Input,
    Misc,
    MobileHacks,
    Touch) {
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
var lastTime;
var newTime;  
  
canvas.addEventListener("touchstart", handleStart, false);
canvas.addEventListener("touchend", handleEnd, false);

function writeMessage(message) {    
  document.getElementById("debugtext").innerHTML = message;            
}

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
canvas.addEventListener("mouseup",function(){
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

  g_audioManager = new AudioManager();

  var sendPad = function(e) {
    
  };

  ExampleUI.setupStandardControllerUI(g_client, globals);  

};

var test = document.getElementById('hft-content');  

    

// Start the main app logic.
requirejs(
  [ '../../../scripts/gameclient',    
    '../../scripts/audio',
    '../../scripts/dpad',
    '../../scripts/exampleui',
    '../../scripts/input',
    '../../scripts/misc',
    '../../scripts/mobilehacks',
    '../../scripts/touch',

  ],
  main
);

