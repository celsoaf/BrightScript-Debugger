

sub Main(args={} as Object)
	a = {
		index: 0
		getIndex: function() as Integer
			return m.index
		end function
	}
end sub

function test(s as String) as Integer
	return len(s)
end function
 