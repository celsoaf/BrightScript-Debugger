'///////////////////////////////////////////////////////////////
'//
'//  $copyright: Copyright (C) 2016 
'//
'///////////////////////////////////////////////////////////////

'
' newSnakeApp() is regular Function of module scope.
' The object container is a BrightScript Component of type roAssocitiveArray (AA).   
' The AA is used to hold member data and member functions.
'

function SnakeApp() as Object
	this = {

'----------------------------------------
' Properties
'----------------------------------------

		compositor: Invalid
		bitmapset: Invalid
		snake: Invalid
		msgport: Invalid
		screen: Invalid
		cellwidth: Invalid

'----------------------------------------
' Public API
'----------------------------------------

		GameReset: Function() As Void
			m.compositor = CreateObject("roCompositor")
			m.compositor.SetDrawTo(m.screen, 0) ' 0 means "no background color".  Use &hFF for black. 

			width = int(m.screen.GetWidth() / m.cellwidth)
			height = int(m.screen.GetHeight() / m.cellheight)
			water = m.bitmapset.animations.water
			for x = 0 to width - 1
				m.compositor.NewAnimatedSprite(x * m.cellwidth, 0, water)
				m.compositor.NewAnimatedSprite(x * m.cellwidth, m.cellheight, water ) 
				m.compositor.NewAnimatedSprite(x * m.cellwidth,  (height - 2) * m.cellheight, water )
				m.compositor.NewAnimatedSprite(x * m.cellwidth,  (height - 1) * m.cellheight, water )
			end for

			for y = 1 to height - 2
				m.compositor.NewAnimatedSprite(0, y * m.cellheight, water )
				m.compositor.NewAnimatedSprite(m.cellwidth, y * m.cellheight, water ) 
				m.compositor.NewAnimatedSprite(m.cellwidth * 2, y * m.cellheight, water )                   
				m.compositor.NewAnimatedSprite((width-3) * m.cellwidth,  y * m.cellheight, water )
				m.compositor.NewAnimatedSprite((width-2) * m.cellwidth,  y * m.cellheight, water )
				m.compositor.NewAnimatedSprite((width-1) * m.cellwidth,  y * m.cellheight, water )      
			end for

			m.compositor.NewSprite(0, 0, m.bitmapset.Regions.Background).SetMemberFlags(0)

			m.snake = Snake(m, m.StartX, m.StartY)
    
			m.compositor.Draw()
		end Function

		EventLoop: Function() As Void
			tick_count = 0
			codes = bslUniversalControlEventCodes()
    
			clock = CreateObject("roTimespan")
			makelongertime = 0
			moveforwardtime = 0
			framecount = 0
			framecounttime = 0
    
			moveforward_every_n_msecs = 200
			grow_every_n_msecs = 1000
    
			while true
				msg = m.msgport.getmessage()   ' poll for a button press
				if msg <> invalid and type(msg) = "roUniversalControlEvent" 
					' remember that the part of an expression after "and" is only evaluated if the prior parts are true
					if msg.GetInt() = codes.BUTTON_UP_PRESSED    and m.snake.Turn(m,  0, -1) 
						return  ' North
					end if
					if msg.GetInt() = codes.BUTTON_DOWN_PRESSED  and m.snake.Turn(m,  0,  1) 
						return  ' South
					end if
					if msg.GetInt() = codes.BUTTON_RIGHT_PRESSED and m.snake.Turn(m,  1,  0) 
						return  ' East
					end if
					if msg.GetInt() = codes.BUTTON_LEFT_PRESSED  and m.snake.Turn(m, -1,  0) 
						return  ' West
					end if
				end if
				' get elapsed time since last time here
				ticks = clock.totalmilliseconds()
				clock.mark()

				makelongertime = makelongertime + ticks
				moveforwardtime = moveforwardtime + ticks
				framecounttime = framecounttime + ticks
				' make longer every 1 sec
				if makelongertime >= grow_every_n_msecs 
					if m.snake.MakeLonger(m) 
						return
					end if
					makelongertime = makelongertime - grow_every_n_msecs
					moveforwardtime = moveforwardtime - moveforward_every_n_msecs     ' we make longer by moving forward, so decrement this also
				end if
				' move forward 10 times per second
				if moveforwardtime >= moveforward_every_n_msecs 
						if m.snake.MoveForward(m) 
							return
						end if
						moveforwardtime = moveforwardtime - moveforward_every_n_msecs
				end if

				m.compositor.AnimationTick(ticks)
				m.compositor.DrawAll()
				m.screen.SwapBuffers()
				framecount = framecount + 1
				' every 3 seconds print out frame speed
				if framecounttime >= 3000
					' in this calculation also include time to this point
					print "frames per second =" + Stri(1000 * framecount / (framecounttime + clock.totalmilliseconds()))
					framecount = 0
					framecounttime = 0
				end if
			end while
		end Function

		GameOver: Function() As Boolean
			codes = bslUniversalControlEventCodes()

			m.compositor.DrawAll()
			dfDrawMessage(m.screen, m.bitmapset.regions["game-over"])
			m.screen.SwapBuffers()  

			while true
				msg = wait(0, m.msgport)
				if type(msg) = "roUniversalControlEvent" 
					if msg.GetInt() = codes.BUTTON_SELECT_PRESSED 
						return false 
					else 
						return true    
					end if
				end if
			end while
		end Function

'----------------------------------------
' Constructors
'----------------------------------------

		init: Function() As Void
			m.screen = CreateObject("roScreen", true)  ' true := use double buffer
			if type(m.screen) <> "roScreen" 
				print "Unable to create roscreen."
				stop   ' stop exits to the debugger
			end if
    
			m.screen.SetAlphaEnable(true) 

			m.bitmapset=dfNewBitmapSet(ReadAsciiFile("pkg:/snake_assets/sprite.small.map.xml"))
			if (m.bitmapset = invalid) 
				stop
			end if
			m.cellwidth = m.bitmapset.extrainfo.cellsize.toint()     ' each cell on game in pixels width
			m.cellheight = m.cellwidth
			m.msgport = CreateObject("roMessagePort")
			m.screen.SetPort(m.msgport)
			m.StartX = int(m.screen.GetWidth() / 2)
			m.StartY = int(m.screen.GetHeight() / 2)
    
			m.TurnSound = CreateObject("roAudioResource", "pkg:/snake_assets/cartoon002.wav")
			m.GameOverSound = CreateObject("roAudioResource", "pkg:/snake_assets/cartoon008.wav")
		end Function
	}
	this.init()

	return this
end function
