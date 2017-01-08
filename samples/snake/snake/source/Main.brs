Library "v30/bslDefender.brs"

'
' The game of Snake
' Demonstrates BrightScript programming concepts
' August 24, 2010


function Main()

	app = SnakeApp()
    dfDrawMessage(app.screen, app.bitmapset.regions["title-screen"])
    app.screen.swapbuffers()
	condition = true
    while condition
        msg=wait(0, app.msgport)
        if type(msg)="roUniversalControlEvent" 
			condition = false
		end if
    end while
    
    condition = true
    while condition
        app.GameReset()        
        app.EventLoop()
        app.GameOverSound.Trigger(100)
        if app.GameOver() 
			condition = false
		end if
    end while

	?"Note: GC - Found 4263 orphaned objects (objects in a circular ref loop)."    
end function
