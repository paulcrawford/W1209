\ W1209 temperature control functions
\ © 2017 TG9541, refer to https://github.com/TG9541/W1209/blob/master/LICENSE

\ For now: just a simple 2-point controller

#include STARTTEMP

   0 CONSTANT OFF
  -1 CONSTANT ON

TARGET

  \ VARIABLE c.heat
  VARIABLE c.cool
  VARIABLE c.delay

  : C.on ( -- n )
    \ lower threshold [0.1ºC]
    \ C.off EE.HYS @ -
    EE.SET @ 
  ;

  : C.off ( -- n )
    \ upper threshold [0.1ºC]
    C.on EE.HYS @ +
    \ for cooling rather than heating need to reverse C.off and C.on
    \ EE.SET @
  ;

  : controller ( theta -- flag )
    \ simple temperature control with hystesis & delay
    \ c.heat @ IF
    c.cool @ IF
      \ ( theta ) C.off SWAP < IF
      ( theta ) C.on < IF
        \ OFF c.heat !
        OFF c.cool !
        EE.DEL @ ( [10s] )
        20 ( ticks [5ms] ) * c.delay !
      THEN
    ELSE
      \ ( theta ) C.on < IF
      ( theta ) C.off SWAP < IF
        c.delay @ IF
          -1 c.delay +!
        ELSE
         \ ON c.heat !
          ON c.cool !
        THEN
      THEN
    THEN
    \ c.heat @           \ return flag
    c.cool @           \ return flag
  ;

  : control ( theta -- theta )
    DUP DEFAULT = NOT IF
      DUP controller ( flag )
    ELSE
      0  ( flag )      \ sensor value undefined: control variable inactive
    THEN
    ( flag ) OUT!      \ switch relay
  ;

  : init ( -- ) init   \ chained init
    \ OFF c.heat !
    \ OFF c.cool !
    ON c.cool !
   0 c.delay !
  ;

ENDTEMP
