%
% Applies a standard algorithm to find a convex hull of points. These points
% are passed in a point array 'points' together with their count 'n'. The output
% is then written to the provided 'output' array and the number of hull points
% is returned. 
%
% Source: https://www.geeksforgeeks.org/convex-hull-set-1-jarviss-algorithm-or-wrapping/
%
vardef ConvexHull(suffix points)(expr n)(suffix output)= 

	% First we find the leftmost point, initially guess it's the first one
	save leftmost_points_index; leftmost_points_index:=0;
		
	% Iterate to find a point that is more to the left
	for i=1 upto n-1:
	
		% We can spot this by comparing the x-coordinates
		if xpart points[i] < xpart points[leftmost_points_index]:
			
			% Set an index of a point more to the left
			leftmost_points_index := i;
		fi
		
	endfor
		
	% The first hull point is the leftmost point
	output[0] := points[leftmost_points_index];
	
	% Prepare the total number of hull points, initially we have one
	save number_of_hull_points; number_of_hull_points := 1;
	
	% Prepare the index of the last hull point, initially the leftmost one
	save last_point_index; last_point_index:=leftmost_points_index;
	
	% Repeat until we've found the whole hull
	forever:
	
		% We will find the index of the next point of the hull
		save next_point_index; next_point_index:=(last_point_index+1) mod n;
			
		% Go through the points
		for i=0 upto n-1:
									
			% And if the current candidate is more to the right
			if ArePointsOrientedClockwisely(points[last_point_index], points[i], points[next_point_index]):
				
				% Than we have found a better candidate
				next_point_index := i;
			fi
		
		endfor
		
		% We're done when we have returned to the beginning
		exitif next_point_index = leftmost_points_index;
		
		% If we're not done, then we have found the next hull point
		last_point_index := next_point_index;
		
		% Set the found point
		output[number_of_hull_points]:= points[last_point_index];
		
		% Count it in
		number_of_hull_points := number_of_hull_points+1;
	
	endfor

	% Return the number of points
	number_of_hull_points
	
enddef;

%
% Determines whether passed points P,Q,R are oriented in such a way that oriented 
% angle PQR would normally be considered higher than 180 degrees
%
def ArePointsOrientedClockwisely(expr P,Q,R) =
	
	% Apply standard slope comparison algorithm 
	% (see for example https://www.geeksforgeeks.org/orientation-3-ordered-points/)
	(ypart Q - ypart P) * (xpart R - xpart Q) < (xpart Q - xpart P) * (ypart R - ypart Q)
	
enddef;