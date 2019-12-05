%%%%%%%%%%% GLOBAL VARIABLES

%
% The scale affecting the size of a point mark (small circle)
%
def PointMarkScale = (sqrt(u)*0.2) enddef;

%
% The distances from an angle arc mark to the intersection point
%
def AngleArcVerySmallRadius = (sqrt(u)*0.3) enddef;
def AngleArcSmallRadius = (sqrt(u)*0.5) enddef;
def AngleArcNormalRadius = (sqrt(u)*0.7) enddef;
def AngleArcBigRadius = (sqrt(u)*0.9) enddef;
def AngleArcVeryBigRadius = (sqrt(u)*1.1) enddef;

%
% The distances from a right angle arc mark to the intersection point
%
def RightAngleArcVerySmallRadius = (sqrt(u)*0.3) enddef;
def RightAngleArcSmallRadius = (sqrt(u)*0.5) enddef;
def RightAngleArcNormalRadius = (sqrt(u)*0.7) enddef;
def RightAngleArcBigRadius = (sqrt(u)*0.9) enddef;
def RightAngleArcVeryBigRadius = (sqrt(u)*1.1) enddef;

%
% The offset between consecutive angle arcs in a multi-arc angle mark
%
def AngleArcsOffset = (u*0.0075) enddef;

%
% The scale affecting right angle's dot
%
def RightAngleDotScale = (u*0.0045) enddef;

%
% The offset of dashdot-dots used in the dashdot pattern
%
def DashDotOffset = (u*0.0076) enddef;

%
% The offset of dots used in the dots pattern
%
def DotsOffset = (u*0.0045) enddef;

%
% The common scale for the dashed pattern
%
def DashesScale = (u*0.0076) enddef;

%
% The length of each line segment making a parallel lines mark
%
def ParallelMarkSegmentLength = (u*0.03) enddef;

%
% The offset between the two line segments making a parallell lines mark
%
def ParallelMarkSegmentOffset = (u*0.0045) enddef;

%
% The angle between the line segments making a parallell lines mark with the line
%
ParallelMarkSegmentAngle = 80;

%%%%%%%%%%% LANGUAGE HELPERS

%
% Pen thickness (the normal one is 0.5)
%
VeryThinPenScale = 0.1;
ThinPenScale = 0.3;
NormalPenScale = 0.5;
ThickPenScale = 0.7;
VeryThickPenScale = 0.9;

%
% Shortcuts for using these degrees of thickness
%
def withverythinpen = withpen pencircle scaled VeryThinPenScale enddef;
def withthinpen = withpen pencircle scaled ThinPenScale enddef;
def withthickpen = withpen pencircle scaled ThickPenScale enddef;
def withverythickpen = withpen pencircle scaled VeryThickPenScale enddef;

% Shortcut for drawing dashdot pattern
%
% Used global variables: DashDotOffset
%
def withdashdots = dashed dashpattern
	(on DashDotOffset 
	off DashDotOffset
	on 0 
	off DashDotOffset 
	on DashDotOffset) enddef;

%	
% Shortcut for drawing dotted pattern
%
def withdots = dashed dashpattern
	(off DotsOffset
	on 0
	off DotsOffset) enddef;

%
% Shortcut for drawing dashed pattern
%
% Used global variables: DashesScale
%
def withdashes = dashed evenly scaled DashesScale enddef;

%
% Shortcut for the pink color
%
def pink = red+blue enddef;

%
% Debug helper to display the position of a point
%
vardef Display(expr point)= draw point withpen pencircle scaled 2 enddef;

%%%%%%%%%%% MARKING POINTS / ANGLES / LINES

%
% Returns a simple point mark, i.e. a circle filled with the background color
%
% Used global variables: PointMarkScale
%
vardef PointMark(expr point) = 
	
	% Prepare circle
	save circle; path circle; circle = fullcircle scaled PointMarkScale shifted point;
	
	% Fill it
	fill circle withcolor background;
	
	% Return it
	circle

enddef;

%
% Returns the angle arc for the given angle <AOB, with the
% specified radius of the angle arc. It can be specified
% whether the angel should be marked as right
% (the markedAsRightAngle parameter), with a dot or not
% (the isEdgy parameter).
%
% Used global variables: RightAngleDotScale
%
vardef AngleMarkInternal(expr A, O, B, arcRadius, markedAsRightAngle, isEdgy) = 

		% Prepare the result
		save result; picture result; result = nullpicture;
	
		% Add the mark for right angle, if requires
		if markedAsRightAngle:
			
			% If it's edgy...
			if isEdgy:

				addto result 
				doublepath (O+arcRadius*sqrt(2)/2*(unitvector(A-O)) -- 
							O+arcRadius*(unitvector(A-O) rotated 45) --
							O+arcRadius*sqrt(2)/2*(unitvector(B-O)));
				
			% If it's not edgy, i.e. an arc with a dot...
			else:			
				addto result 
				contour fullcircle 
				scaled RightAngleDotScale
				shifted (O + arcRadius/2*(unitvector(A-O) rotated 45));
				
			fi;
		fi;
		
		% If the angle is not edgy...
		if (not isEdgy):

			% Add the arc to the result
			addto result 
			doublepath (O + arcRadius*unitvector(A-O)) { unitvector(A-O) rotated 90 }
				.. (O + arcRadius*unitvector(B-O)) { unitvector(B-O) rotated 90 };
			
		fi;
		
		% Return the result
		result
		
enddef;

%
% Returns the angle arc for the given right angle <AOB, with the
% specified radius of the angle arc.
%
def AngleMarkCustom(expr A, O, B, arcRadius) = 

	% Call the internal macro that will assume this is not a right angle
	AngleMarkInternal(A, O, B, arcRadius, false, false)
	
enddef;

%
% Returns the angle arc for the given angle <AOB, with the
% very small radius of the angle arc.
%
% Used global variables: AngleArcVerySmallRadius
%
def AngleMarkVerySmall(expr A, O, B) = 

	% Call the custom version of this macro
	AngleMarkCustom(A, O, B, AngleArcVerySmallRadius)
	
enddef;

%
% Returns the angle arc for the given angle <AOB, with the
% small radius of the angle arc.
%
% Used global variables: AngleArcSmallRadius
%
def AngleMarkSmall(expr A, O, B) = 

	% Call the custom version of this macro
	AngleMarkCustom(A, O, B, AngleArcSmallRadius)
	
enddef;

%
% Returns the angle arc for the given angle <AOB, with the
% normal radius of the angle arc.
%
% Used global variables: AngleArcNormalRadius
%
def AngleMark(expr A, O, B) = 

	% Call the custom version of this macro
	AngleMarkCustom(A, O, B, AngleArcNormalRadius)
	
enddef;

%
% Returns the angle arc for the given angle <AOB, with the
% big radius of the angle arc.
%
% Used global variables: AngleArcBigRadius
%
def AngleMarkBig(expr A, O, B) = 

	% Call the custom version of this macro
	AngleMarkCustom(A, O, B, AngleArcBigRadius)
	
enddef;

%
% Returns the angle arc for the given angle <AOB, with the
% very big radius of the angle arc.
%
% Used global variables: AngleArcVeryBigRadius
%
def AngleMarkVeryBig(expr A, O, B) = 

	% Call the custom version of this macro
	AngleMarkCustom(A, O, B, AngleArcVeryBigRadius)
	
enddef;

%
% Returns the angle arc with a dot for the given right angle <AOB, with the
% specified radius of the angle arc.
%
def RightAngleMarkCustom(expr A, O, B, arcRadius, isEdgy) =

	% Call the internal macro that will take this angle as a right one
	AngleMarkInternal(A, O, B, arcRadius, true, isEdgy)
	
enddef;

%
% Returns the angle arc with a dot for the given right angle <AOB, with the
% very small radius of the angle arc.
%
% Used global variables: RightAngleArcVerySmallRadius
%
def RightAngleMarkVerySmall(expr A, O, B) =

	% Call the custom version of this macro
	RightAngleMarkCustom(A, O, B, RightAngleArcVerySmallRadius, false)
	
enddef;

%
% Returns the angle arc with a dot for the given right angle <AOB, with the
% small radius of the angle arc.
%
% Used global variables: RightAngleArcSmallRadius
%
def RightAngleMarkSmall(expr A, O, B) =

	% Call the custom version of this macro
	RightAngleMarkCustom(A, O, B, RightAngleArcSmallRadius, false)
	
enddef;

%
% Returns the angle arc with a dot for the given right angle <AOB, with the
% normal radius of the angle arc.
%
% Used global variables: RightAngleArcNormalRadius
%
def RightAngleMark(expr A, O, B) =

	% Call the custom version of this macro
	RightAngleMarkCustom(A, O, B, RightAngleArcNormalRadius, false)
	
enddef;

%
% Returns the angle arc with a dot for the given right angle <AOB, with the
% big radius of the angle arc.
%
% Used global variables: RightAngleArcBigRadius
%
def RightAngleMarkBig(expr A, O, B) =

	% Call the custom version of this macro
	RightAngleMarkCustom(A, O, B, RightAngleArcBigRadius, false)
	
enddef;

%
% Returns the angle arc with a dot for the given right angle <AOB, with the
% big radius of the angle arc.
%
% Used global variables: RightAngleArcVeryBigRadius
%
def RightAngleMarkVeryBig(expr A, O, B) =

	% Call the custom version of this macro
	RightAngleMarkCustom(A, O, B, RightAngleArcVeryBigRadius, false)
	
enddef;

%
% Returns the edgy angle arc for the given right angle <AOB, with the
% very small radius of the angle arc.
%
% Used global variables: RightAngleArcVerySmallRadius
%
def EdgyRightAngleMarkVerySmall(expr A, O, B) =

	% Call the custom version of this macro
	RightAngleMarkCustom(A, O, B, RightAngleArcVerySmallRadius, true)
	
enddef;

%
% Returns the edgy angle arc for the given right angle <AOB, with the
% small radius of the angle arc.
%
% Used global variables: RightAngleArcSmallRadius
%
def EdgyRightAngleMarkSmall(expr A, O, B) =

	% Call the custom version of this macro
	RightAngleMarkCustom(A, O, B, RightAngleArcSmallRadius, true)
	
enddef;

%
% Returns the edgy angle arc for the given right angle <AOB, with the
% normal radius of the angle arc.
%
% Used global variables: RightAngleArcNormalRadius
%
def EdgyRightAngleMark(expr A, O, B) =

	% Call the custom version of this macro
	RightAngleMarkCustom(A, O, B, RightAngleArcNormalRadius, true)
	
enddef;

%
% Returns the edgy angle arc for the given right angle <AOB, with the
% big radius of the angle arc.
%
% Used global variables: RightAngleArcBigRadius
%
def EdgyRightAngleMarkBig(expr A, O, B) =

	% Call the custom version of this macro
	RightAngleMarkCustom(A, O, B, RightAngleArcBigRadius, true)
	
enddef;

%
% Returns the edgy angle arc for the given right angle <AOB, with the
% big radius of the angle arc.
%
% Used global variables: RightAngleArcVeryBigRadius
%
def EdgyRightAngleMarkVeryBig(expr A, O, B) =

	% Call the custom version of this macro
	RightAngleMarkCustom(A, O, B, RightAngleArcVeryBigRadius, true)
	
enddef;


%
% Returns the angle arcs for the given angle <AOB, with the
% specified radius of the angle arc and the number of the arcs to be used
%
% Used global variables: AngleArcsOffset
%
vardef AngleMarkWithArcsCustom(expr A, O, B, arcsCount, arcRadius) = 

	% Prepare result
	save result; picture result; result = nullpicture;
	
	% Add the requested number of arcs
	for i=0 upto arcsCount-1:
	
		% Call the internal macro (false indicates no right-angle mark)
		addto result also AngleMarkInternal(A,O,B,arcRadius+AngleArcsOffset*i,false,false);
		
	endfor;	
	
	% Return the result
	result
	
enddef;

%
% Returns the angle arcs for the given angle <AOB, with the
% very small radius of the angle arc and the number of the arcs to be used
%
% Used global variables: AngleArcsOffset, AngleArcVerySmallRadius
%
vardef AngleMarkWithArcsVerySmall(expr A, O, B, arcsCount) = 
		
	% Call the custom version of this macro
	AngleMarkWithArcsCustom(A, O, B, arcsCount, AngleArcVerySmallRadius)
			
enddef;

%
% Returns the angle arcs for the given angle <AOB, with the
% small radius of the angle arc and the number of the arcs to be used
%
% Used global variables: AngleArcsOffset, AngleArcSmallRadius
%
vardef AngleMarkWithArcsSmall(expr A, O, B, arcsCount) = 
		
	% Call the custom version of this macro
	AngleMarkWithArcsCustom(A, O, B, arcsCount, AngleArcSmallRadius)
			
enddef;

%
% Returns the angle arcs for the given angle <AOB, with the
% default radius of the angle arc and the number of the arcs to be used
%
% Used global variables: AngleArcsOffset, AngleArcNormalRadius
%
vardef AngleMarkWithArcs(expr A, O, B, arcsCount) = 
		
	% Call the custom version of this macro
	AngleMarkWithArcsCustom(A, O, B, arcsCount, AngleArcNormalRadius)
			
enddef;

%
% Returns the angle arcs for the given angle <AOB, with the
% big radius of the angle arc and the number of the arcs to be used
%
% Used global variables: AngleArcsOffset, AngleArcBigRadius
%
vardef AngleMarkWithArcsBig(expr A, O, B, arcsCount) = 
		
	% Call the custom version of this macro
	AngleMarkWithArcsCustom(A, O, B, arcsCount, AngleArcBigRadius)
			
enddef;

%
% Returns the angle arcs for the given angle <AOB, with the
% very big radius of the angle arc and the number of the arcs to be used
%
% Used global variables: AngleArcsOffset, AngleArcVeryBigRadius
%
vardef AngleMarkWithArcsVeryBig(expr A, O, B, arcsCount) = 
		
	% Call the custom version of this macro
	AngleMarkWithArcsCustom(A, O, B, arcsCount, AngleArcVeryBigRadius)
			
enddef;

%
% Returns a simple mark used for parallel lines with 2 small line segments
% The marks are replaces according to the value of the 'placement'
% parameter, for example: 0.5 for the middle, 1/3 in one third of AB
%
% Used global variables: ParallelMarkSegmentLength
%					     ParallelMarkSegmentOffset
%						 ParallelMarkSegmentAngle
%
vardef ParallelMark(expr A,B,placement) = 

	% Prepare the midpoint of the mark
	save middle; pair middle; middle = placement[A,B];
	
	% Prepare the left and the right point of the mark
	save left, right; pair left, right;
	left = ((abs(A-middle)-ParallelMarkSegmentOffset)/abs(A-B))[A,B];
	right = ((abs(A-middle)+ParallelMarkSegmentOffset)/abs(A-B))[A,B];
	
	% Prepare the result
	save result; picture result; result = nullpicture;
	
	% Add segments
	addto result doublepath 
	left + ParallelMarkSegmentLength/2 * unitvector(A-B) rotated ParallelMarkSegmentAngle -- 
	left + ParallelMarkSegmentLength/2 * unitvector(B-A) rotated ParallelMarkSegmentAngle;
	
	addto result doublepath 
	right + ParallelMarkSegmentLength/2 * unitvector(A-B) rotated ParallelMarkSegmentAngle 
	-- 
	right + ParallelMarkSegmentLength/2 * unitvector(B-A) rotated ParallelMarkSegmentAngle;
	
	% Return the result
	result
	
enddef;

%%%%%%%%%%% COMMON CONSTRUCTIONS

%
% Returns the interesection of two paths. If causes an error
% if the paths don't interesect each other
%
vardef Intersection(expr firstPath, secondPath) = firstPath intersectionpoint secondPath enddef;

%
% Extends the line segment A, B in both direction by the given length 
%
vardef Line(expr A, B, length) = (-length/abs(A-B))[A,B] -- (-length/abs(A-B))[B,A] enddef;

%
% Returns the line perpendicular to AB at A with a given length
%
vardef PerpendicularLine(expr A, B, length)= 
		
	% Take line AB and rotate it around A by 90
	Line(A, RotatedPoint(B, A, 90), length)

enddef;

%
% Rotates the given point around the given center by the given angle.
%
vardef RotatedPoint(expr pointToRotate, centerOfRotation, angle) = 

	% First shift to the origin, then rotate, and then shift back 
	(pointToRotate-centerOfRotation) rotated angle + centerOfRotation

enddef;

%
% Reflect the given point in the given center
%
vardef ReflectedPointInPoint(expr pointToReflect, center) = 	2[pointToReflect,center] enddef;

%
% Projects the given point onto the line given by two points
%
vardef ProjectedPoint(expr pointToProject, firstPoint, secondPoint) =
	
	% Prepare the result
	save result; pair result; 
	
	% This result on the given line
	result = whatever [firstPoint, secondPoint];
	
	% And line [result, pointToProject] is perpendicular to line [firstPoint, secondPoint]
	result - pointToProject = whatever * (firstPoint - secondPoint) rotated 90;
	
	% Now it's computed
	result
	
enddef;

%
% Return the line perpendicular to BC through A with a given length
%
vardef PerpendicularLineToLine(expr A, B, C, length) =
	
	% It's line from A to the projection of A onto BC
	Line(A, ProjectedPoint(A,B,C), length)
	
enddef;

%
% Reflects the given point onto the line given by two points
%
vardef ReflectedPoint(expr pointToReflect, firstPoint, secondPoint) =

	% Take the point 2 times further than the projection of pointToReflect
	% on the given line from points [firstPoint, secondPoint]
	2[pointToReflect, ProjectedPoint(pointToReflect, firstPoint, secondPoint)]

enddef;

%
% Returns point D such that ABDC is a parallelogram
%
vardef Parallelogram(expr A,B,C) = ReflectedPointInPoint(A,0.5[B,C]) enddef;

%
% Returns the circumcenter of the triangle formed by 
% the given three mutally distinct points.
%
vardef Circumcenter(expr A, B, C) = 

	% O will hold the result
	save O; pair O;
	
	% O Midpoint(A,B) is perpendicular to AB
	O-1/2[A,B] = whatever  * (A-B) rotated 90;
	
	% O Midpoint(A,C) is perpendicular to AC
	O-1/2[A,C] = whatever  * (A-C) rotated 90;
	
	% Now it's computed
	O
	
enddef;

%
% Returns the circle with given diameter points
%
vardef CircleWithDiameter(expr A,B) = Circle(0.5[A,B], A) enddef;

%
% Returns the circumcircle of the triangle formed by 
% the given three mutally distinct points.
%
vardef Circumcircle(expr A, B, C) =
	
	% Find the circumcenter 
	save O; pair O; O = Circumcenter(A,B,C);
	
	% Return the circle with the center at O and the radius OA
	fullcircle scaled 2abs(O-A) shifted O
	
enddef;

%
% Returns the radius of the circumcircle of ABC
%
def RadiusOfCircumcircle(expr A, B, C) = abs(Circumcenter(A, B, C)-A) enddef;

%
% Returns the diameter of the circumcircle of ABC
%
def DiameterOfCircumcircle(expr A, B, C) = 2RadiusOfCircumcircle(A, B, C) enddef;

%
% Returns the midpoint of the arc ABC of the circumcircle of ABC. The points
% should be specified clockwisely
%
vardef MidpointOfArc(expr A, B, C) = 

	% Find the diameter of the circle (ABC)
	save diameter; numeric diameter; diameter = DiameterOfCircumcircle(A, B, C);
	
	% Interesect the arc with the long-enough perpendicular bisector of AC 
	Intersection(Arc(A, B, C), PerpendicularBisector(A, C, 2diameter))

enddef;

%
% Returns the midpoint of the arc (BA) of the circumcircle of ABC. The points
% should be specified clockwisely
%
vardef MidpointOfOppositeArc(expr A, B, C) = 

	% Find the diameter of the circle (ABC)
	save diameter; numeric diameter; diameter = DiameterOfCircumcircle(A, B, C);
	
	% Interesect the opposite arc with the long-enough perpendicular bisector of AC 
	Intersection(OppositeArc(A, B, C), PerpendicularBisector(A, C, 2diameter))

enddef;

%
% Returns the arc BC or BC of the circumcircle of triangle ABC, based on the 
% parameter indacting whether it should contain point A.
% The points B, A, C should be specified clockwisely
%
vardef ArcInternal(expr B, A, C, shouldArcContainA) = 

	% Find the circumcenter O of ABC
	save O; pair O; O = Circumcenter(A,B,C);
	
	% Find the circumcircle of ABC
	save circumcircle; path circumcircle; 
	
	% Pre-calculate the radius of the circle
	save radius; numeric radius; radius = abs(O-A);
	
	% Construct the circumcircle so that in time 0 it is in point B
	circumcircle = (fullcircle scaled 2radius) rotated angle(B-O) shifted O;
	
	% Find the intersection time of C
	save timeC; numeric timeC; 
	(timeC, whatever) = circumcircle intersectiontimes (O -- O + 2radius*unitvector(C-O));
		
	% If the desired arc contains point A...
	if shouldArcContainA: 
	
		% Return the arc (CB) 
		subpath (timeC, length(circumcircle)) of circumcircle		
		
	% Otherwise...
	else:	
	
		% Return the arc (BC)
		subpath(0, timeC) of circumcircle
	fi
enddef;

%	
% Returns the arc CB circumcircle of triangle ABC (i.e. the one containing A)
% The points B, A, C should be specified clockwisely
%
vardef Arc(expr B, A, C) = ArcInternal(B, A, C, true) enddef;

%
% Returns the arc BC circumcircle of triangle ABC (i.e. the one not containing A).
% The points B, A, C should be specified clockwisely
%
vardef OppositeArc(expr B, A, C) = ArcInternal(B, A, C, false) enddef;

%
% Returns the perpendicular bisector of line segment AB. The length parameter
% specifiec the length on both sides (i.e. the total length is 2length)
%
vardef PerpendicularBisector(expr A, B, length) = 
	
	% Find the midpoint of AB
	save M; pair M; M = 1/2[A,B];
	
	% Rotate A around M by 90 degrees
	% (so we have the second point on the desired bisector)
	save N; pair N; N = RotatedPoint(A, M, 90);
	
	% Shift N so it's distance from M is the given length
	save T; pair T; T = (length/abs(M-N))[M,N];
		
	% The result is the line from T to the point 2 times further in direction M,N
	T -- 2[T,M]
	
enddef;

%
% Find point D such that ABCD (or ABDC) is an isoscles trapezoid
% (AC and BD will be symmetric wrt the perpendicular bisector of AB)
%
vardef IsoscelesTrapezoid(expr A,B,C) =

  % Reflect C wrt the perpendicular bisector of AB
  % which is defined by the midpoint of AB
  % and one point lying on it which could be 
  % A rotated by 90 around the midpoint of AB
  ReflectedPoint(C, 1/2[A,B], RotatedPoint(A, 1/2[A,B], 90)) 
 
enddef;

%
% Returns the internal angle bisector of angle ABC with a given length
% going both sides (i.e. the length of each side is equal to length/2)
%
vardef InternalAngleBisector(expr A,B,C,length)= 

	% Find one point lying on this bisector with the distance length/2 from B
	save P; pair P; P = (length/2,0) rotated 1/2(angle(A-B)+angle(C-B)) shifted B;
	
	% Extend it to both directions so the total length is 2length
	P--2[P,B]

enddef;

%
% Returns the external angle bisector of angle ABC with a given length
%
vardef ExternalAngleBisector(expr A,B,C,length)= 

	% Get internal bisector and rotate it by 90 around B
	InternalAngleBisector(A,B,C,length) shifted -B rotated 90 shifted B

enddef;

%
% Returns the line passing through A parallel to BC with a given length
% going both sides (i.e. the length of each side is equal to length/2)
%
vardef ParallelLine(expr A,B,C,length)=
	A+(length/2)*unitvector(B-C) -- A+(length/2)*unitvector(C-B)
enddef;

%
% Returns the semicircle going from A to B clockwisely
%
vardef Semicircle(expr A,B) = 

	% Find its midpoint (the midpoint of AB)
	save O; pair O; O = 1/2[A,B];
	
	% Find its radius 
	save radius; radius = abs(A-B)/2;
	
	% Create a full circle with diameter AB rotated so that the point at time 0 is A
	save circle; path circle; circle = fullcircle rotated angle(A-B) scaled 2radius shifted O;
	
	% Find intersection time of B on the circle 
	save timeB; save timeB; 
	(timeB, whatever) = circle intersectiontimes (O -- O + 2radius*unitvector(B-O));
	
	% Cut it to a semicircle
	subpath (0,timeB) of circle

enddef;

%
% Returns point D such that OAB, OCD are spirally similar triangles
%
vardef SpiralSimilarity(expr O,A,B,C)= 
	
	% We need to make O origin first. 
	% Then we rotate C around by the needed angle
	% Then use homothety so the triangles are similar
	% And shift the result back
	C shifted -O rotated (angle(O-B)-angle(O-A)) scaled (abs(O-B)/abs(O-A)) shifted O
	
enddef;

%
% Returns the arc (AB) of the circle centered at O passing through A,B
% This macro assumes OA = OB. The arc should be specified clockwisely.
%
vardef ArcWithCenter(expr A,B,O) = 
	
	% Draw the whole circle so that A is the point at time 0
	save circle; path circle; 
	circle = fullcircle rotated angle(A-O) scaled 2abs(A-O) shifted O;
	
	% Find intersection time B on the circle 
	save timeB; save timeB; 
	(timeB, whatever) = circle intersectiontimes (O -- O + 2abs(A-O)*unitvector(B-O));
	
	% Return the subarc
	subpath(0,timeB) of circle
		
enddef;

%
% Returns the circle centered at O passing through A
%
vardef Circle(expr O,A) = CircleWithRadius(O, abs(O-A)) enddef;

%
% Returns the circle centered at O with given radius
%
vardef CircleWithRadius(expr O, radius) = fullcircle scaled 2radius shifted O enddef;

%
% Returns the ray opposite to ray AB with given length.
%
vardef OppositeRay(expr A,B,length)= ShiftedOppositeRay(A,B,length,0) enddef;

%
% Returns the ray opposite to ray AB with given length which 'first' point
% has a distance equal to 'shiftLength' from A. The use-case for this is when 
% want to intersect a ray and we don't want to include the first point
%
vardef ShiftedOppositeRay(expr A,B,length,shiftLength) = 

	% Get the shifted point and connect it with the point distanced 'length' from A
	(A - shiftLength*unitvector(B-A)) -- (-length/abs(B-A))[A,B]

enddef;

%
% Returns the ray AB with given length.
%
vardef Ray(expr A,B,length)= ShiftedRay(A,B,length,0) enddef;

%
% Returns the ray AB with given length which 'first' point
% has a distance equal to 'shiftLength' from A. The use-case for this is when 
% want to intersect a ray and we don't want to include the first point
%
vardef ShiftedRay(expr A,B,length,shiftLength) = 

	% Get the shifted point and connect it with the point distanced 'length' from A
	(A - shiftLength*unitvector(A-B)) -- (length/abs(B-A))[A,B]

enddef;

%
% Shifts the given point by the given angle and the given length
%
vardef ShiftedInDirection(expr P, angle, length)= P + length * unitvector(dir angle) enddef;

%
% Draws the segment AB extended by a given length
%
vardef ExtendedSegment(expr A, B, length) = Ray(A, B, abs(A-B)+length) enddef;

%
% Draws the segment AB extended by given lengths
%
vardef ExtendedSegmentDouble(expr leftLength, A, B, rightLength) = ShiftedRay(A, B, abs(A-B)+rightLength, leftLength) enddef;

% Returns the centroid of triangle ABC
vardef Centroid(expr A,B,C) = (A+B+C)/3 enddef;

%
% Returns the orthocenter of triangle ABC
%
def Orthocenter(expr A,B,C) = 
	A + whatever*(B-C) rotated 90 = 
	B + whatever*(A-C) rotated 90 
enddef;

%
% Returns the incenter of triangle ABC
%
vardef Incenter(expr A,B,C) = 

	% Prepare the result
	save I; pair I;

	% I lies on two angle bisectors
	(I-A) = whatever * (A-C) rotated 1/2( angle(B-A) - angle(C-A));
	(I-B) = whatever * (B-A) rotated 1/2( angle(C-B) - angle(A-B));
	
	% Return the result which should be calculated now
	I

enddef;

%
% Finds the point where the A-excircle touch a specified side.
% This side is specified with the 'pointId' parameter like this:
%
%   pointId = 0 --> BC
%   pointId = 1 --> AB
%   pointId = 2 --> AC
%
vardef ExcircleTouchPoint(expr A, B, C, pointId) =
	
	% Calculate the sides of the triangle
	save a; a = abs(B-C);
	save b; b = abs(A-C);
	save c; c = abs(A-B);
	
	% The point on BC is distanced (a+b-c)/2 from B
	if pointId = 0:
		Intersection(B--C, CircleWithRadius(B, (a+b-c)/2))
		
	% The point on AB is distanced (a+b+c)/2 from A
	% which is less than 2a
	elseif pointId = 1:
		Intersection(Ray(A,B,2a), CircleWithRadius(A, (a+b+c)/2))
		
	% The point on AC is distanced (a+b+c)/2 from A
	% which is less than 2a
	elseif pointId = 2:
		Intersection(Ray(A,C,2a), CircleWithRadius(A, (a+b+c)/2))
		
	else:
		errmessage("Incorrect value of the pointId parameter. It should be 0, 1, or 2.");
	fi

enddef;

% 
% Returns the A-excenter of the triangle ABC
%
vardef Excenter(expr A, B, C) =

	% Reuse the circumcircle and the touch points
	Circumcenter(ExcircleTouchPoint(A,B,C,0),
			     ExcircleTouchPoint(A,B,C,1),
				 ExcircleTouchPoint(A,B,C,2))

enddef;

% 
% Return the A-excircle of the triangle ABC
%
vardef Excircle(expr A, B, C) =
	
	% Reuse the excenter and the touch point
	Circle(Excenter(A,B,C),ExcircleTouchPoint(A,B,C,0))
	
enddef;

%
% Returns the second intersection of the circles (ABC), (ADE),
% i.e. the one distinct from A
%
vardef SecondIntersectionOfCircumcenters(expr A,B,C,D,E) =
	ReflectedPoint(A,Circumcenter(A,B,C),Circumcenter(A,D,E))
enddef