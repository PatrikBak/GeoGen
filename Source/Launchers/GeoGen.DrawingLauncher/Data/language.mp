%
% This file contains macros that provide human-readable text for problems. The macro names
% must correspond to the names from the code. The suffix (_Text) is optional, but must be 
% same for all the macros. All these macros receive string names of the individual objects 
% and compose a human-readable string as a result.
%

%%%%%%%%%%%% LAYOUTS %%%%%%%%%%%%

def Triangle_Text(expr A,B,C) = 
	"Let~$" & A & B & C & "$ be a~triangle. "
enddef;

def Quadrilateral_Text(expr A,B,C,D) = 
	"Let~$" & A & B & C & D & "$ be a~convex quadrilateral. "
enddef;

%%%%%%%%%%%% CONSTRUCTIONS %%%%%%%%%%%%

def Midpoint_Text(expr M,A,B) = 
	"Let~$" & M & "$ be the midpoint of~$" & A & B & "$. "		
enddef;

def PointReflection_Text(expr P,A,B) = 
	"Let~$" & P & "$ be the reflection of~$" & A & "$ in~$" & B & "$. "
enddef;

def ParallelLine_Text(expr l,P,k) = 
	"Let~$" & l & "$ be the line through~$" & P & "$ parallel to~$" & k & "$. "		
enddef;

def ParallelLineToLineFromPoints_Text(expr l,A,B,C) = 
	"Let~$" & l & "$ be the line through $" & A & "$ parallel to $" & B & C & "$. "
enddef;

def PerpendicularLine_Text(expr l,A,B) = 
	"Let~$" & l & "$ be the line through~$" & A & "$ such that~$" & l & " \perp " & A & B & "$. "		
enddef;

def PerpendicularLineToLineFromPoints_Text(expr l,A,B,C) = 
	"Let~$" & l & "$ be the line through $" & A & "$ perpendicular to $" & B & C & "$. "
enddef;

def PerpendicularLineAtPointOfLine_Text(expr l,A,B) = 
	"Let~$" & l & "$ be the line through $" & A & "$ perpendicular to $" & A & B & "$. "
enddef;

def PerpendicularBisector_Text(expr l,A,B) = 
	"Let~$" & l & "$ be the perpendicular bisector of the segment~$" & A & B & "$. "
enddef;

def TangentLine_Text(expr l,A,B,C) = 
	"Let~$" & l & "$ be the line tangent to circle~$" & A & B & C & "$ at~$" & A & "$. "
enddef;

def InternalAngleBisector_Text(expr l,A,B,C) = 
	"Let~$" & l & "$ be the internal angle bisector of~$\angle " & B & A & C & "$. "
enddef;

def ExternalAngleBisector_Text(expr l,A,B,C) = 
	"Let~$" & l & "$ be the external angle bisector of~$\angle " & B & A & C & "$. "
enddef;

def PerpendicularProjectionOnLineFromPoints_Text(expr P,A,B,C) = 
	"Let~$" & P & "$ be the projection of~$" & A & "$ on~$" & B & C & "$. "
enddef;

def PerpendicularProjection_Text(expr P,A,l) = 
	"Let~$" & P & "$ be the projection of~$" & A & "$ on~$" & l & "$. "
enddef;

def ReflectionInLineFromPoints_Text(expr P,A,B,C) = 
	"Let~$" & P & "$ be the reflection of~$" & A & "$ in~$" & B & C & "$. "
enddef;

def ReflectionInLine_Text(expr P,l,A) = 
	"Let~$" & P & "$ be the reflection of~$" & A & "$ in~$" & l & "$. "
enddef;

def MidpointOfArc_Text(expr P,A,B,C) = 
	"Let~$" & P & "$ be the midpoint of arc~$" & B & A & C & "$. "
enddef;

def MidpointOfOppositeArc_Text(expr P,A,B,C) = 
	"Let~$" & P & "$ be the midpoint of opposite arc~$" & B & A & C & "$. "
enddef;

def ParallelogramPoint_Text(expr P,A,B,C) = 
	"Let~$" & P & "$ be such a~point that~$" & A & B & P & C & "$ is a~parallelogram. "
enddef;

def OppositePointOnCircumcircle_Text(expr P,A,B,C) = 
	"Let~$" & P & "$ be the point opposite to~$" & A & "$ on circle~$" & B & A & C & "$. "
enddef;

def Circumcenter_Text(expr P,A,B,C) = 
	"Let~$" & P & "$ be the circumcenter of~$" & A & B & C & "$. "
enddef;

def Orthocenter_Text(expr P,A,B,C) = 
	"Let~$" & P & "$ be the orthocenter of~$" & A & B & C & "$. "
enddef;

def Median_Text(expr l,A,B,C) = 
	"Let~$" & l & "$ be the~$" & A & "$-median of~$" & A & B & C & "$. "
enddef;

def IntersectionOfLines_Text(expr P,k,l) = 
	"Let~$" & P & "$ be the intersection point of~$" & k & "$ and $" & l & "$. "		
enddef;

def IntersectionOfLineAndLineFromPoints_Text(expr P,k,A,B) = 
	"Let~$" & P & "$ be the intersection point of~$" & k & "$ and $" & A & B & "$. "		
enddef;

def IntersectionOfLinesFromPoints_Text(expr P,A,B,C,D) = 
	"Let~$" & P & "$ be the intersection point of~$" & A & B & "$ and $" & C & D & "$. "		
enddef;

def Incircle_Text(expr c,A,B,C) = 
	"Let~$" & c & "$ be the incircle of~$" & A & B & C & "$. "		
enddef;

def Excircle_Text(expr c,A,B,C) = 
	"Let~$" & c & "$ be the~$" & A & "$-excircle of~$" & A & B & C & "$. "		
enddef;

def Incenter_Text(expr P,A,B,C) = 
	"Let~$" & P & "$ be the incenter of~$" & A & B & C & "$. "		
enddef;

def Excenter_Text(expr P,A,B,C) = 
	"Let~$" & P & "$ be the~$" & A & "$-excenter of~$" & A & B & C & "$. "		
enddef;

def CircleWithDiameter_Text(expr c,A,B) = 
	"Let~$" & c & "$ be the circle with a~diameter~$" & A & B & "$. "		
enddef;
%%%%%%%%%%%% THEOREMS %%%%%%%%%%%%

def EqualLineSegments_Text(expr k,l) =
	"Show that $" & k & "=" & l & "$. "
enddef;

def ConcurrentLines_Text(expr p,q,r) =
	"Show that lines~$" & p & "," & q & "," & r & "$ are concurrent. "
enddef;

def TangentCircles_Text(expr k,l) =
	"Show that circles~$" & k & "$ and $" & l & "$ are tangent to each other. "
enddef;

def LineTangentToCircle_Text(expr k,l) =
	"Show that line~$" & k & "$ and circle $" & l & "$ are tangent to each other. "
enddef;

def ConcyclicPoints_Text(expr A,B,C,D) =
	"Show that points~$" & A & "," & B & "," & C & "," & D & "$ are concyclic. "
enddef;

def CollinearPoints_Text(expr A,B,C) =
	"Show that points~$" & A & "," & B & "," & C & "$ are collinear. "
enddef;

def ParallelLines_Text(expr k,l) =
	"Show that $" & k & " \parallel " & l & "$. "
enddef;

def PerpendicularLines_Text(expr k,l) =
	"Show that $" & k & " \perp " & l & "$. "
enddef;

def Incidence_Text(expr P,k) =
	"Show that $" & P & "$ lies on $" & k & "$. "
enddef;