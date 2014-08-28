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

  var canvas = document.getElementById('hft-content');      
  var avatar = document.getElementById('avatar');
  var click = false;
  var startPos;
  var endPos;  
  var lastTime;
  var newTime;  
  

  function writeMessage(message) {
    
    document.getElementById("debugtext").innerHTML = message;
            
  }
  
  function getMousePos(canvas, evt) {
    var rect = canvas.getBoundingClientRect();
    return {
      x: evt.clientX - rect.left,
      y: evt.clientY - rect.top
    };
  }

  function getTouchPos(canvas, evt) {
    var rect = canvas.getBoundingClientRect();
    return {
      x: evt.pageX - rect.left,
      y: evt.pageY - rect.top
    };
  }


//////////////////////////////////////////////////////
//MOUSE CLICK  DOWN
canvas.addEventListener("mousedown",function(evt){
  var mousePos = getMousePos(canvas, evt);
  startPos = mousePos;
  lastTime = Date.now();
  click = true;
});

//TOUCH DOWN
canvas.addEventListener("touchstart", handleStart, false);
function handleStart(evt) {   
    var touchPos = getTouchPos(canvas, evt);
    startPos = touchPos;
    lastTime = Date.now();
    click = true;
}
//////////////////////////////////////////////////////



/////////////////////////////////////////////////////
//MOUSE OVER AVATAR
avatar.addEventListener("mouseover",function(evt){
  if (click == true) {
  
  //TIME
    newTime = Date.now();
    var totalTime = newTime - lastTime;

  //POSITION
    var mousePos = getMousePos(canvas, evt);    
    endPos = mousePos;    
    
    writeMessage('Start position: ' + startPos.x + ',' + startPos.y +'    End position: ' + endPos.x + ',' + endPos.y + '   Time:   ' + totalTime);    

    g_client.sendCmd('swipe',{ platform: "mouse" , startX: startPos.x, startY: startPos.y, endX: endPos.x, endY: endPos.y});
  }
});

//TOUCH OVER AVATAR
canvas.addEventListener("touchmove", handleMove, false);

function handleMove(evt) {
  if (click == true) {
    
    evt.preventDefault(); //prevents mouse event

    var touches = evt.changedTouches;   //Array of touches that is updated as long as the swipe goes
    
    var touchRect = avatar.getBoundingClientRect();
    
    if ((evt.changedTouches[0].pageX > touchRect.left) && (evt.changedTouches[0].pageX < touchRect.right)) { //if inside avatar X
      if ((evt.changedTouches[0].pageY > touchRect.top) && (evt.changedTouches[0].pageY < touchRect.bottom)) { //if inside avatar Y        
    
      //TIME
        newTime = Date.now();
        var totalTime = newTime - lastTime;

      //POSITION
        var touchPos = getTouchPos(canvas, evt);    
        endPos = touchPos;
        var message = "X:  " + startPos.x + "   Y:  " + startPos.y + "      X:  " + endPos.x + "   Y:  " + endPos.y + '   Time:   ' + totalTime;
        writeMessage(message);
        
        
        g_client.sendCmd('swipe',{ platform: "touch" , startX: startPos.x, startY: startPos.y, endX: endPos.x, endY: endPos.y});
        click = false;
      } 

    }    
  }  
};

/////////////////////////////////////////////////////////////////////////////////////////////////////


canvas.addEventListener("mouseup",function(){
  click = false;
});




  g_client.addEventListener('score', handleScore);
  g_client.addEventListener('die', handleDeath);

  var color = Misc.randCSSColor();
  g_client.sendCmd('setColor', { color: color });
  document.body.style.backgroundColor = color;

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

