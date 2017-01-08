
function Snake(app, x, y) as Object
	this = {

'----------------------------------------
' Properties
'----------------------------------------
		
		app: app
		x: x
		y: y
		tail: Invalid
		tongue: Invalid

		dx : 1      ' default snake direction
		dy : 0      ' default snake direction

'----------------------------------------
' Public API
'----------------------------------------

		Turn : Function(app, newdx, newdy) 
			'stop
			if newdx = m.dx and newdy = m.dy 
				return false   ' already heading this way
			end if
    
			tongue_x = m.tongue.GetX() + newdx * app.cellwidth * 2
			tongue_y = m.tongue.GetY() + newdy * app.cellheight * 2
    
			head_x = tongue_x - newdx * app.cellwidth
			head_y = tongue_y - newdy * app.cellwidth
    
			corner_x = head_x - newdx * app.cellwidth
			corner_y = head_y - newdy * app.cellwidth
    
			prior_dx = m.dx
			prior_dy = m.dy
        
			m.dx = newdx
			m.dy = newdy
    
			newtongue = app.compositor.NewAnimatedSprite(tongue_x, tongue_y, app.bitmapset.animations[m.DirectionName( newdx, newdy, "tongue-")] )
			newhead = app.compositor.NewSprite(head_x, head_y, app.bitmapset.regions[m.RegionName( newdx, newdy, "head-")] )
			newcorner = m.tongue
			newbody = newcorner.GetData().previous
    
			newtongue.SetMemberFlags(0)
    
   
			 newcorner.SetData( {dx: newdx, dy: newdy, next: newhead,  previous: newcorner.GetData().previous} )
			   newhead.SetData( {dx: newdx, dy: newdy, next: newtongue,  previous: newcorner} )
			 newtongue.SetData( {dx: newdx, dy: newdy, next: invalid,  previous: newhead} )

			m.tongue = newtongue
    
			if newhead.CheckCollision() <> invalid 
				return true
			end if
  
		   ' fixup the last segment render 
		   ' (there is a tongue which turns into a corner, and a head which turns into body)    

			newbody.SetRegion(app.bitmapset.regions[m.RegionName(newbody.GetData().dx, newbody.GetData().dy, "body-")])
    
			if m.dy = -1 ' turned north
				if prior_dx = -1  ' was west-bound
					newcorner.SetRegion(app.bitmapset.regions[m.RegionName(0, -1, "corner-")])
				else
					newcorner.SetRegion(app.bitmapset.regions[m.RegionName(-1, 0, "corner-")])
				end if
			end if    
			if m.dy = 1  ' turned south
				if prior_dx = -1  ' was west-bound
					newcorner.SetRegion(app.bitmapset.regions[m.RegionName(1, 0, "corner-")])
				else
					newcorner.SetRegion(app.bitmapset.regions[m.RegionName(0, 1, "corner-")])
				end if
			end if    
			if m.dx = -1  ' turned west / left
				if prior_dy = -1 ' was north-bound
					newcorner.SetRegion(app.bitmapset.regions[m.RegionName(0, 1, "corner-")])
				else
					newcorner.SetRegion(app.bitmapset.regions[m.RegionName(-1, 0, "corner-")])
				end if
			end if
			if m.dx = 1  ' turned east / right
				if prior_dy = -1   ' was north-bound
					newcorner.SetRegion(app.bitmapset.regions[m.RegionName(1, 0, "corner-")])
				else
					newcorner.SetRegion(app.bitmapset.regions[m.RegionName(0, -1, "corner-")])
				end if
			end if
    
			app.TurnSound.Trigger(100)
    
			return false
		end Function

        MoveForward : Function(app)
			sprite = m.tail
			m.tail = m.tail.GetData().next
			m.tail.GetData().previous = invalid
			sprite.Remove()
			m.tail.SetRegion(app.bitmapset.regions[m.RegionName(m.tail.GetData().dx, m.tail.GetData().dy, "butt-")])
			return m.MakeLonger(app) ' This isnt actually making the snake longer, its just the 2nd half of MoveForward()
		end Function

        MakeLonger : Function(app)
			newbody_x = m.tongue.GetX() - m.dx * app.cellwidth
			newbody_y = m.tongue.GetY() - m.dy * app.cellheight
			newbody = app.compositor.NewSprite(newbody_x, newbody_y, app.bitmapset.regions[m.RegionName( m.dx, m.dy, "body-")] )

			m.tongue.MoveOffset(m.dx * app.cellwidth, m.dy * app.cellheight)
			head = m.tongue.GetData().previous
			head.MoveOffset(m.dx * app.cellwidth, m.dy * app.cellheight)
    
			body = head.GetData().previous
			head.GetData().previous = newbody
			body.GetData().next = newbody
			newbody.SetData( {dx: m.dx, dy: m.dy, next: head , previous: body} )
			collision = head.CheckCollision()
			return head.CheckCollision() <> invalid
		end Function

		DirectionName : function(xdelta, ydelta, base)
            if xdelta = 1 
               dir="East"
			end if
            if xdelta = -1  
               dir="West"
			end if
            if ydelta = 1  
               dir = "South"
			end if
			if ydelta = -1   
               dir = "North"
            end if
            return base+dir
        end function    
        
        RegionName : function(xdelta, ydelta, base)
            if xdelta = 1 
               dir="East"
            end if
            if xdelta = -1  
               dir="West"
            end if
            if ydelta = 1  
               dir = "South"
            end if
            if ydelta = -1  
               dir = "North"
            end if
            return "snake." + base+dir
        end function

'----------------------------------------
' Constructors
'----------------------------------------

		init: Function() As Void
			m.tongue = m.app.compositor.NewAnimatedSprite(m.x, m.y, m.app.bitmapset.animations[m.DirectionName( 1, 0, "tongue-")] )
			head = m.app.compositor.NewSprite(m.x - m.app.cellwidth, m.y, m.app.bitmapset.regions[m.RegionName( 1, 0, "head-")] )
			body = m.app.compositor.NewSprite(m.x - 2 * m.app.cellwidth, m.y, m.app.bitmapset.regions[m.RegionName( 1, 0, "body-")] )
			m.tail = m.app.compositor.NewSprite(m.x - 3 * m.app.cellwidth, m.y, m.app.bitmapset.regions[m.RegionName( 1, 0, "butt-")] )

			m.tail.SetData( {dx: 1, dy: 0, next: body, previous: invalid} )
			body.SetData( {dx: 1, dy: 0, next: head, previous: m.tail} )
			head.SetData( {dx: 1, dy: 0, next: m.tongue, previous: body} )
			m.tongue.SetData( {dx: 1, dy: 0, next: invalid, previous: head} )

			m.tongue.SetmemberFlags(0)
		end Function
	}

	this.init()

	return this
end function
