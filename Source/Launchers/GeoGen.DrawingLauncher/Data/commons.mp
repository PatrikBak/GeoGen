% 
% A helper macro that can be used to break from loops without causing 'end occurred before...'
% messages. The usage is the following:
%
% (1) If it's used in a non-nested if condition, then it must not be followed by ;
% (2) If it's used outside an if condition, it must be followed by ;
% 
def break = expandafter exitif expandafter true expandafter; enddef;

%
% Scales a given picture without changing the position of its center
%
vardef Scaled(expr picture, scale) =
	
	% Find the center of the picture
	save middle; pair middle; middle:=0.5[llcorner(picture),urcorner(picture)];
	
	% Shift it so it can be scaled and then shift back
	picture shifted (-middle) scaled scale shifted middle
	
enddef;

%
% Expands to a boolean condition indicating whether a given point is within the bounding box
% of a passed item (which can be a picture or path)
%
def IsInBoundingBox(expr point, item) = 
	
	% Check every coordinate
	((xpart point >= xpart (ulcorner item)) and
	( xpart point <= xpart (urcorner item)) and
	( ypart point >= ypart (llcorner item)) and
	( ypart point <= ypart (ulcorner item)))
	
enddef;

%
% Finds all intersection points of the two passed paths. They will be written to the
% provided 'intersections' point array.
%
vardef FindIntersectionPoints(expr path_one, path_two)(suffix intersections) =

	% Prepare the path that will get cut from the intersection points
	save cut_path; path cut_path; cut_path := path_one;
	
	% Prepare the number of intersections
	save number_of_intersections; number_of_intersections := 0;
	
	% Do until we find all the intersection points
	forever:
		
		% Cut the non-intersecting part
		cut_path := cut_path cutbefore path_two;
		
		% If there are no more intersections, we're done
		exitif length cuttings = 0;
		
		% We will now try to cut the current path so that its first point is not the
		% already found intersection point anymore. But we will do it only if the path
		% isn't too short already (just in case)
		exitif eps >= length cut_path;
		
		% Cut the path by little
		cut_path := subpath(eps, length cut_path) of cut_path;
		
		% Mark the intersection point
		intersections[number_of_intersections] = point 0 of cut_path;
		
		% Count the intersection point in
		number_of_intersections := number_of_intersections + 1;
		
	endfor;
	
	% Return the final number of intersection points
	number_of_intersections
	
enddef;