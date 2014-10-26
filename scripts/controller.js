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
g_client.addEventListener('score', handleScore);
g_client.addEventListener('die', handleDeath);

/////////////////////////////////////////////////////////////////////////////

var viewportWidth = window.innerWidth;
var viewportHeight = window.innerHeight;
var canvasWidth = window.innerWidth - 100;
var canvasHeight = window.innerHeight - 100;

canvas.style.position = "absolute";
canvas.setAttribute("width", canvasWidth);
canvas.setAttribute("height", canvasHeight);
canvas.style.top = (viewportHeight - canvasHeight) / 2 + "px";
canvas.style.left = (viewportWidth - canvasWidth) / 2 + "px";
var context = canvas.getContext("2d");
var player;
var playerColor;
var mouseX;
var mouseY;
var dragging;
var dragHoldX;
var dragHoldY;

var actionMaxRadius;

////LISTENERS
g_client.addEventListener('changeBG', changeBG);
g_client.addEventListener('myTurn', handleTurn);
window.addEventListener("touchstart", handleTouchStart, false);
window.addEventListener("touchend", handleTouchEnd, false);

window.addEventListener("mousedown",mouseDown,false);
window.addEventListener("mouseup", mouseUp, false);



/////////

window.onresize = function() {

  viewportWidth = window.innerWidth;
  viewportHeight = window.innerHeight;
  canvasWidth = window.innerWidth - 50;
  canvasHeight = window.innerHeight - 50;
  canvas.setAttribute("width", canvasWidth);
  canvas.setAttribute("height", canvasHeight);
  canvas.style.top = (viewportHeight - canvasHeight) / 2 + "px";
  canvas.style.left = (viewportWidth - canvasWidth) / 2 + "px";
  player.x = canvasWidth/2;
  player.y = canvasHeight/2;
  drawScreen();
}


function createPlayer(){

  var centerX = canvas.width/2;
  var centerY = canvas.height/2;
  var radius = 50;
  player = {x:centerX, y:centerY, rad:radius};

  drawScreen();  
}


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
    dragHoldX = mousePos.x - player.x;
    dragHoldY = mousePos.y - player.y;

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
    dragHoldX = mousePos.x - player.x;
    dragHoldY = mousePos.y - player.y;

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
  
  player.x = canvas.width/2;
  player.y = canvas.height/2;

  drawScreen();  
}

function mouseUp(evt){
  if (dragging){
    dragging = false;
    window.removeEventListener("mousemove", mouseMove, false);
  } 
  var mousePos = getMousePos(canvas,evt);
  g_client.sendCmd('kick',{ platform: "mouse"});   

  player.x = canvas.width/2;
  player.y = canvas.height/2;

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
    
    player.x = posX;
    player.y = posY;
    
    drawScreen();
  }

function mouseMove(evt) {  
    var posX;
    var posY;    
    var shapeRad = player.rad
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

function writeMessage(message) {
  document.getElementById("debugtext").innerHTML = message;
}

function drawPlayer(){

  context.beginPath();
  context.arc(player.x, player.y, player.rad, 0, 2*Math.PI, false);      
  context.fillStyle = playerColor;  
  context.closePath();
  context.fill();  

  context.beginPath();
  context.arc(player.x, player.y, player.rad/1.25, 0, 2*Math.PI, false);      
  context.fillStyle = "white";  
  context.closePath();
  context.fill();


  context.beginPath();  
  context.moveTo(player.x - player.rad/1.5, player.y); // A1
  
  context.bezierCurveTo(
    player.x - player.rad/1.5, player.y - player.rad/2.5, // C1
    player.x + player.rad/1.5, player.y - player.rad/2.5, // C2
    
    player.x + player.rad/1.5, player.y); // A2

  context.bezierCurveTo(
    player.x + player.rad/1.5, player.y + player.rad/2.5, // C1
    player.x - player.rad/1.5, player.y + player.rad/2.5, // C2
    
    player.x - player.rad/1.5, player.y); //A1

  
 
  context.fillStyle = playerColor;
  context.fill();
  context.closePath();  


  context.beginPath();
  context.arc(player.x, player.y, player.rad/2.5, 0, 2*Math.PI, false);      
  context.fillStyle = "pink";  
  context.closePath();
  context.fill();


  context.beginPath();
  context.arc(player.x, player.y, player.rad/2.5, 100, 10, false);      
  context.fillStyle = "brown"; 
  context.closePath();
  context.fill();  
}


function drawLine() {
  context.beginPath();
  context.moveTo(player.x,player.y);
  context.lineTo(canvas.width/2,canvas.height/2);
  context.lineWidth = 10;
  context.setLineDash([25,5])
  context.strokeStyle = "white";
  context.stroke();
}

function drawContour(){

  //check screen ratio  
  if (canvas.height < canvas.width){
    actionMaxRadius =  canvasHeight/2 - ((canvasHeight/100) * 5);
  } else {
    actionMaxRadius =  canvasWidth/2  - ((canvasWidth/100) * 5);
  }

  context.beginPath();
  context.arc(canvas.width/2, canvas.height/2, actionMaxRadius, 0, 2*Math.PI, false);        
  context.closePath();
  context.setLineDash([25,5])
  context.lineWidth = 5;
  context.strokeStyle = 'white';
  context.stroke();
  
}

function drawScreen() {      
  context.clearRect(0, 0, canvas.width, canvas.height);
  drawPlayer();
  drawLine();
  drawContour();
  var kickStrength = adjustStrengthRatio();  
  g_client.sendCmd('drawLine',{playerX: player.x, playerY: player.y, lineEndX: canvas.width/2, lineEndY: canvas.height/2, strength: kickStrength});
}

function adjustStrengthRatio() {

  var realDistance = lineDistance();
  var radiusUnit = actionMaxRadius / 100;
  
  if (realDistance/radiusUnit > 100) {

    return 100;
  } else {
    return realDistance / radiusUnit;
  }
}

function lineDistance()
{
  var xd = player.x - canvasWidth/2;
  var yd = player.y - canvasHeight/2;
 
  return Math.sqrt( xd*xd + yd*yd );
}


function changeBG(data){
  if (data.playerTeam = 1) {
    playerColor = 'red';
  } else if (data.playerTeam = 2) {    
    playerColor = 'blue';
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




