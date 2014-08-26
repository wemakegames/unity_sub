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
  var distance;

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

  function force(image) {
  
    if (click == true) {
      
    }
  };
      
canvas.addEventListener("mousedown",function(evt){
  var mousePos = getMousePos(canvas, evt);
  startPos = mousePos;
  lastTime = Date.now();
  click = true;
}); 

avatar.addEventListener("mouseover",function(evt){
  if (click == true) {
  
  //TIME
    newTime = Date.now();
    var totalTime = newTime - lastTime;

  //DISTANCE
    var mousePos = getMousePos(canvas, evt);    
    endPos = mousePos;      
    var xd = endPos.x - startPos.x;
    var yd = endPos.y - startPos.y;     
    distance = Math.sqrt( (xd * xd) + (yd * yd));
    distance = Math.ceil(distance);

    var message = 'Start position: ' + startPos.x + ',' + startPos.y +'    End position: ' + endPos.x + ',' + endPos.y + '    Distance:   ' + distance + '   Time:   ' + totalTime;
    writeMessage(message);
    var swipe = distance;    
    g_client.sendCmd('swipe',{ swipe: "test" , startX: startPos.x, startY: startPos.y, endX: endPos.x, endY: endPos.y});
  }
});

  avatar.addEventListener("mouseout",function(){
    if (click == true) {
      message ="out";
      writeMessage(message);
    }
  });

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

