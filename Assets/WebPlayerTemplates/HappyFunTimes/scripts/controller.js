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
    orientation: "portrait",
  };
Misc.applyUrlSettings(globals);
MobileHacks.fixHeightHack();


var $ = document.getElementById.bind(document);

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
var canvasWidth = window.innerWidth - 50;
var canvasHeight = window.innerHeight - 50;

canvas.style.position = "absolute";
canvas.setAttribute("width", canvasWidth);
canvas.setAttribute("height", canvasHeight);
canvas.setAttribute("z-index", 1);
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

var myTurn = true;

var actionMaxRadius;

CommonUI.setupStandardControllerUI(g_client, globals);

////LISTENERS
g_client.addEventListener('setPlayerColor', setPlayerColor);
g_client.addEventListener('myTurn', handleTurn);
$("inputarea").addEventListener("pointerdown", handlePointerDown, false);
$("inputarea").addEventListener("pointerup", handlePointerUp, false);
$("inputarea").addEventListener("pointermove", handlePointerMove, false);

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
  var radius = 25;
  player = {x:centerX, y:centerY, rad:radius};

  drawScreen();  
}


function getTouchPos(canvas, evt) {
  return Input.getRelativeCoordinates(canvas, evt);
}

function handlePointerDown(evt) {
  var mousePos = getTouchPos(canvas, evt);
  if  (myTurn && hitTest(player, mousePos.x, mousePos.y)) {
    dragging = true;
    dragHoldX = mousePos.x - player.x;
    dragHoldY = mousePos.y - player.y;
  }
}

function handlePointerUp(evt) {
  if (dragging){
    dragging = false;
  }
  
  var mousePos = getTouchPos(canvas,evt);  
  g_client.sendCmd('kick');  
  
  player.x = canvas.width/2;
  player.y = canvas.height/2;

  drawScreen();  
}

function handlePointerMove(evt) {
  if (!dragging) {
    return;
  }
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
  context.strokeStyle = playerColor;
  context.stroke();
}

function drawContour(){

  //check screen ratio  
  if (canvas.height < canvas.width){
    actionMaxRadius =  canvasHeight/2 - player.rad;
  } else {
    actionMaxRadius =  canvasWidth/2  - player.rad;
  }

  context.beginPath();
  context.arc(canvas.width/2, canvas.height/2, actionMaxRadius, 0, 2*Math.PI, false);        
  context.closePath();
  context.setLineDash([25,5])
  context.lineWidth = 5;
  context.strokeStyle = playerColor;
  context.stroke();
  
}

function drawScreen() {      
  context.clearRect(0, 0, canvas.width, canvas.height);
  
  if (myTurn == true){
    drawPlayer();
    drawLine();    
    var kickStrength = adjustStrengthRatio();  
    g_client.sendCmd('drawLine',{playerX: player.x, playerY: player.y, lineEndX: canvas.width/2, lineEndY: canvas.height/2, strength: kickStrength});
  
  } else if (myTurn == false) {

    var imageObj = new Image();

      imageObj.onload = function() {
        context.drawImage(imageObj, (canvasWidth/2-imageObj.width/2),(canvasHeight/2 - imageObj.height/2));
      };
      imageObj.src = 'hft-assets/noplay.png';
  }
  drawContour();
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


function setPlayerColor(data){
  
  if (data.playerTeam == 1) {
    playerColor = 'red';    
  } else if (data.playerTeam == 2) {        
    playerColor = 'blue';
  }
  
  createPlayer();

}

function handleTurn(data) {
  
  var  turnMsg;
  if (data.myTurn == true){
    turnMsg = "MY TURN";    
    myTurn = true;

  } else {
    turnMsg = "NOT MY TURN";
    myTurn = false;
  }
  
  drawScreen();
  //writeMessage(turnMsg)
}

  var sendPad = function(e) {

  };

});




