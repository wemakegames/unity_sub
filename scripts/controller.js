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
    ''
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
g_client.addEventListener('score', handleScore);
g_client.addEventListener('die', handleDeath);

/////////////////////////////////////////////////////////////////////////////

var canvas = document.getElementById('canvas');
var context = canvas.getContext("2d");
var lastTime;
var newTime;
var player;
var bgColor;
var mouseX;
var mouseY;
var dragging;
var dragHoldX;
var dragHoldY;

////LISTENERS
g_client.addEventListener('changeBG', changeBG);
g_client.addEventListener('myTurn', handleTurn);

window.load = createPlayer();
window.addEventListener("touchstart", handleTouchStart, false);
window.addEventListener("touchend", handleTouchEnd, false);

window.addEventListener("mousedown",mouseDown,false);
window.addEventListener("mouseup", mouseUp, false);

var player;

var playerWidth = 100;
var playerHeight = 100;
var playerX = canvas.width/2 - playerWidth/2;
var playerY = canvas.height/2 - playerHeight/2;



function createPlayer(){

  
  player = new Image();

  player.load = function() {    
    context.drawImage(player,playerX,playerY);
    
    drawScreen();

  }
  player.src = 'hft-assets/player.png';



}

  

/////////




function getTouchPos(canvas, evt) {
  var bRect = canvas.getBoundingClientRect();
    mouseX = (evt.pageX - bRect.left)*(canvas.width/bRect.width);
    mouseY = (evt.pageY - bRect.top)*(canvas.height/bRect.height);   
  
  return {
    x: mouseX,
    y: mouseY
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

function mouseDown(evt) {
  var mousePos = getMousePos(canvas,evt);
    
  if  (hitTest(player, mousePos.x, mousePos.y)) {
    dragging = true;
    dragHoldX = mousePos.x - playerX;
    dragHoldY = mousePos.y - playerY;

  }
  if (dragging) {
    window.addEventListener("mousemove", mouseMove, false);
  }
}

function handleTouchStart(evt) {
    evt.preventDefault(); //prvent mouse movement
    var mousePos = getTouchPos(canvas, evt);
    
    if  (hitTest(player, mousePos.x, mousePos.y)) {
    dragging = true;
    dragHoldX = mousePos.x - playerX;
    dragHoldY = mousePos.y - playerY;

  }
  if (dragging) {
    window.addEventListener("touchmove", touchMove, false);
  }
}

function handleTouchEnd(evt) {

  evt.preventDefault(); //prvent mouse movement  
  if (dragging){
    dragging = false;
    //window.removeEventListener("touchmove", touchmove, false);
  }
  
  var mousePos = getMousePos(canvas,evt);
  g_client.sendCmd('kick',{ platform: "touch"});  
  
  playerX = canvas.width/2;
  playerY = canvas.height/2;

  drawScreen();  
}

function mouseUp(evt){
  if (dragging){
    dragging = false;
    window.removeEventListener("mousemove", mouseMove, false);
  }
  newTime = Date.now();
  var totalTime = newTime - lastTime;
  var mousePos = getMousePos(canvas,evt);
  g_client.sendCmd('kick',{ platform: "mouse"});  
  

  playerWidth = canvas.width/2;
  playerHeight = canvas.height/2;

  drawScreen();
};


function touchMove(evt) {  
    var posX;
    var posY;    
    var shapeRad = player.rad
    var minX = shapeRad;
    var maxX = canvas.width - shapeRad;
    var minY = shapeRad;
    var maxY = canvas.height - shapeRad;
    //getting mouse position correctly 
    var touchPos = getTouchPos(canvas,evt);
    
    //clamp x and y positions to prevent object from dragging outside of canvas
    posX = touchPos.x - dragHoldX;
    posX = (posX < minX) ? minX : ((posX > maxX) ? maxX : posX);
    posY = touchPos.y - dragHoldY;
    posY = (posY < minY) ? minY : ((posY > maxY) ? maxY : posY);
    
    playerX = posX;
    playerY = posY;
    
    drawScreen();
  }

function mouseMove(evt) {  
    var posX;
    var posY;    
    var shapeRad = playerWidth/2;
    var minX = shapeRad;
    var maxX = canvas.width - shapeRad;
    var minY = shapeRad;
    var maxY = canvas.height - shapeRad;
    //getting mouse position correctly 
    var mousePos = getMousePos(canvas,evt)
    
    //clamp x and y positions to prevent object from dragging outside of canvas
    posX = mousePos.x - dragHoldX;
    posX = (posX < minX) ? minX : ((posX > maxX) ? maxX : posX);
    posY = mousePos.y - dragHoldY;
    posY = (posY < minY) ? minY : ((posY > maxY) ? maxY : posY);
    
    playerX = posX;
    playerY = posY;
    
    drawScreen();
  }

function hitTest(shape,mx,my) {
  var dx;
  var dy;  
  dx = mx - (player.x + playerWidth/2);
  dy = my - (player.y + playerHeight/2);  
  //a "hit" will be registered if the distance away from the center is less than the radius of the circular object    
  return (dx*dx + dy*dy < playerWidth*playerWidth);
  console.log("hit");
}


function writeMessage(message) {
  document.getElementById("debugtext").innerHTML = message;
}



function drawPlayer(){ 
  
  context.drawImage(player,playerX,playerY);
}


function drawLine() {
  context.beginPath();
  context.moveTo(playerX,playerY);
  context.lineTo(canvas.width/2,canvas.height/2);
  context.stroke();
  g_client.sendCmd('drawLine',{ platform: "mouse" , playerX: playerX, playerY: playerY, lineEndX: canvas.width/2, lineEndY: canvas.height/2});

}

function drawScreen() {
    //bg
    context.clearRect(0, 0, canvas.width, canvas.height);
    context.fillStyle = bgColor;
    context.fillRect(0,0,canvas.width,canvas.height);    
    drawPlayer();
    drawLine();
    
    
    

    
  }


function changeBG(data){
  if (data.playerTeam = 1) {
    bgColor = 'red';
  } else if (data.playerTeam = 2) {    
    bgColor = 'blue';
  }
  createPlayer();
}

function handleTurn(data) {
  writeMessage(data.turnText)
}

var sendPad = function(e) {

  };

  CommonUI.setupStandardControllerUI(g_client, globals);
});




