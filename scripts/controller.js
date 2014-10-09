"use strict";

// Start the main app logic.
requirejs(
  [ 'hft/commonui',
    'hft/gameclient',
    'hft/misc/dpad',
    'hft/misc/input',
    'hft/misc/misc',
    'hft/misc/mobilehacks',
    'hft/misc/touch',
  ], function(
    CommonUI,
    GameClient,
    DPad,
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

console.log( "Create new GameClient" );
g_client = new GameClient();

function handleScore() {
};

function handleDeath() {
};

var canvas = document.getElementById('canvas');
var context = canvas.getContext("2d");
var startPos;
var endPos;
var lastTime;
var newTime;
var player;
var bgColor;
var mouseX;
var mouseY;
var dragging;
var dragHoldX;
var dragHoldY;

function createPlayer(){

  var centerX = canvas.width/2;
  var centerY = canvas.height/2;
  var radius = 10;
  player = {x:centerX, y:centerY, rad:radius};

  drawScreen();  
}

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

function getMousePos(canvas, evt) {
  var bRect = canvas.getBoundingClientRect();
    mouseX = (evt.clientX - bRect.left)*(canvas.width/bRect.width);
    mouseY = (evt.clientY - bRect.top)*(canvas.height/bRect.height);
  return {
    x: mouseX,
    y: mouseY
  };
}

function handleStart(evt) {
    var touchPos = getTouchPos(canvas, evt);
    startPos = touchPos;
    lastTime = Date.now();    
}

function handleEnd(evt) {

  evt.preventDefault(); //prvent mouse movement
  newTime = Date.now();

  var totalTime = newTime - lastTime;
  var lastTouch = evt.changedTouches[0];

  var message = "X:  " + startPos.x + "   Y:  " + startPos.y + "      X:  " + lastTouch.pageX + "   Y:  " + lastTouch.pageY + '   Time:   ' + totalTime;
  //g_client.sendCmd('kick',{ platform: "touch" , startX: startPos.x, startY: startPos.y, endX: lastTouch.pageX, endY: lastTouch.pageY, duration: totalTime});  
  g_client.sendCmd('kick',{ platform: "touch"});  
}


canvas.addEventListener("mousedown",function(evt){
  var mousePos = getMousePos(canvas,evt);
  startPos = mousePos;
  lastTime = Date.now();  

  if  (hitTest(player, mousePos.x, mousePos.y)) {
    dragging = true;
    dragHoldX = mousePos.x - player.x;
    dragHoldY = mousePos.y - player.y;
  }

  if (dragging) {
    window.addEventListener("mousemove", mouseMoveListener, false);
  }


});

function mouseMoveListener(evt) {
    
    var posX;
    var posY;    
    var shapeRad = player.rad
    var minX = shapeRad;
    var maxX = canvas.width - shapeRad;
    var minY = shapeRad;
    var maxY = canvas.height - shapeRad;
    //getting mouse position correctly 
    var bRect = canvas.getBoundingClientRect();
    mouseX = (evt.clientX - bRect.left)*(canvas.width/bRect.width);
    mouseY = (evt.clientY - bRect.top)*(canvas.height/bRect.height);
    
    //clamp x and y positions to prevent object from dragging outside of canvas
    posX = mouseX - dragHoldX;
    posX = (posX < minX) ? minX : ((posX > maxX) ? maxX : posX);
    posY = mouseY - dragHoldY;
    posY = (posY < minY) ? minY : ((posY > maxY) ? maxY : posY);
    
    player.x = posX;
    player.y = posY;
    
    drawScreen();
  }

function hitTest(shape,mx,my) {
  var dx;
  var dy;
  dx = mx - shape.x;
  dy = my - shape.y;
  
  //a "hit" will be registered if the distance away from the center is less than the radius of the circular object    
  return (dx*dx + dy*dy < shape.rad*shape.rad);
}

window.addEventListener("mouseup",function(evt){
  if (dragging){
    dragging = false;
    window.removeEventListener("mousemove", mouseMoveListener, false);
  }
  newTime = Date.now();
  var totalTime = newTime - lastTime;
  var mousePos = getMousePos(canvas,evt);
  endPos = mousePos;  
   g_client.sendCmd('kick',{ platform: "mouse"});  
  

  player.x = canvas.width/2;
  player.y = canvas.height/2;

  drawScreen();
});

  g_client.addEventListener('score', handleScore);
  g_client.addEventListener('die', handleDeath);


function drawPlayer(){

  context.beginPath();
  context.arc(player.x, player.y, player.rad, 0, 2*Math.PI, false);  
    
  context.fillStyle = "white";
  context.closePath();
  context.fill();  
}

function drawLine() {
  context.beginPath();
  context.moveTo(player.x,player.y);
  context.lineTo(canvas.width/2,canvas.height/2);
  context.stroke();

}

function drawScreen() {
    //bg
    context.fillStyle = bgColor;
    context.fillRect(0,0,canvas.width,canvas.height);
    player.fillStyle = "yellow";
    drawPlayer();
    drawLine();

    g_client.sendCmd('drawLine',{ platform: "mouse" , playerX: player.x, playerY: player.y, lineEndX: canvas.width/2, lineEndY: canvas.height/2});
  }


function changeBG(data){
  if (data.playerTeam = 1) {
    bgColor = 'red';
  } else if (data.playerTeam = 2) {    
    bgColor = 'blue';
  }
  createPlayer();
}

g_client.addEventListener('changeBG', changeBG);
g_client.addEventListener('myTurn', handleTurn);

function handleTurn(data) {
  writeMessage(data.turnText)
}

var sendPad = function(e) {

  };

  CommonUI.setupStandardControllerUI(g_client, globals);
});



