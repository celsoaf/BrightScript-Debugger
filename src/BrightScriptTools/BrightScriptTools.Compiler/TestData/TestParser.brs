

sub Main(args={} as Object)
	i=0
	s="teste"
	b=true
	a={}
	t=1+2
	t=1=2
	a.t = 1
	c()
	d(2)
	e(a.t, t, i)
	ty = type("teste")
	arr = [ 1, 2, 3 ]
	arr = [ 
		1
		2
		3
	]
	v = arr[1]
end sub

function test(t as String, b as Boolean, v) as Void
	print "hello world"
	return 22
end function
 