input commons
input convex_hull
input matrix_helpers

%%%%%%%%%%%% VARIABLES %%%%%%%%%%%%

% 
% The name of the font that used for drawing labels.
%
string Font; Font="lmmi12";

%
% The scale factor that is applied to the glyphs loaded via the 'glyph' operator.
%
FontScale:=0.01;

%
% The number of points that are uniformly taken on letter's glyphs in order to approximate its convex hull.
%
LetterPathsSampleSize:= 60;

%
% The minimal offset of the label's center and the labeled point.
%
MinimalLabelDistanceOffset:= 1;

%
% The factor used to calculate the maximal label distance by scaling the minimal one.
%
MaximalLabelDistanceScale := 1.4;

% 
% The number of tested placement distances from the label point's center.
%
DistantLabelPositions := 4;

%
% The number of tested circular placements in a certain distance from the label point's center.
%
CircularLabelPositions := 20;

%
% The scale factor that is applied to the convex hull of a letter that is used to calculate the badness
% of the currently examined placement.
%
BadnessHullScale := 1.5;
	
%
% The scale factor applied to the convex hull of a letter that is drawn with the background color in order 
% to redraw the overlapping lines.
%
PlacementHullScale := 1.1;

%%%%%%%%%%%% MACROS %%%%%%%%%%%%

%
% Labels a given point so that the label is intelligently placed (the code will be cleaned)
%
vardef LabelPoint(expr label, P) =
		
	% First load the label, which will create letter.image and letter.hull pictures.
	Letter(label)(letter);
	
	% Calculate the minimal distance from the position of the placement point by taking into account
	% the offset taken from the global variable and the distance of the center of the image and its vertices
	save minimal_distance; minimal_distance:= MinimalLabelDistanceOffset + abs(llcorner(letter.image)-urcorner(letter.image))/2;
	
	% Calculate the maximal placement distance by scaling the minimal one
	save maximal_distance; maximal_distance:= MaximalLabelDistanceScale*minimal_distance;
	
	% A helper macro that places an object according to a given angle and distances, given by indices.
	% The results depends on the number of examined circular positions, given by CircularLabelPositions,
	% and the number of examined distanced, given by DistantLabelPositions.
	vardef Position(expr object, angle_index, distance_index) =
		
		% Calculate the final distance
		save distance; distance:= minimal_distance + distance_index/(DistantLabelPositions-1)*(maximal_distance-minimal_distance);		
		
		% Calculate the final angle
		save angle; angle:= angle_index*(360/CircularLabelPositions);
		
		% Place the object initially at the point
		object shifted P
		
			   % Shift to the right distance, rotated by the right angle
		       shifted (distance*cosd(angle),distance*sind(angle))
			   
			   % Shift by the center of the picture
			   shifted (-((llcorner object)+(urcorner object))/2)			   
	enddef;
	
	% Prepare a variable for all badness values
	save all_badness;
	
	% Iterate over the requested number of distances
	for distance_index=0 upto DistantLabelPositions-1:
				
		% Iterate over the requested number of circular positions
		for angle_index=0 upto CircularLabelPositions-1:
			
			% Prepare the value of the total badness that will be accumulated 
			save badness; badness:=0;
												
			% Count in the badness for every picture item												
			for item within currentpicture:
			
				% Scale the convex hull of the letter accordingly
				save hull; path hull; hull:=Scaled(letter.hull,BadnessHullScale);
				
				% Position it so the badness can be calculated correctly
				hull:=Position(letter.hull,angle_index, distance_index);
								
				% Count in the current badness
				badness:=badness + Badness(hull, pathpart item);
			
			endfor
				
			% Remember the badness
			all_badness[distance_index][angle_index]:= badness;
														
		endfor
		
	endfor
	
	% Find the chain of zeroes
	FindLongestsZeroChainInRow(all_badness, DistantLabelPositions, CircularLabelPositions, longest_chain);
	
	% Prepare the variable indicating whether we will need to erase overlapping lines
	save erasing; boolean erasing; erasing := false;
	
	% If there is no zero
	if longest_chain.dimension = -1:
		
		% We will make sure there is some. First find the smallest badness value
		save minimal_badness; minimal_badness:=FindMinimum(all_badness, DistantLabelPositions, CircularLabelPositions);
		
		% Let's make at least one zero by decreasing all elements by the minimum
		DecreaseAll(all_badness, DistantLabelPositions, CircularLabelPositions, minimal_badness);
		
		% Repeat the call for the longest chain that will succeed now
		FindLongestsZeroChainInRow(all_badness, DistantLabelPositions, CircularLabelPositions, longest_chain);
		
		% There will be erasing because we most likely cannot place the label without overlapping
		erasing := true;	
	fi
	
	% Fix the longest end so that end > start
	if longest_chain.end_index < longest_chain.start_index:
	
		% We can work modulo CircularLabelPositions
		longest_chain.end_index:= longest_chain.end_index + CircularLabelPositions;
	
	fi
	
	% If the longest chain is basically equal to the whole arc, i.e. there is a distance such that any label
	% placement in that distance works, then the current setup would prefer placing the label to
	% the left. A better default position is placing the label to the top
	if (longest_chain.start_index=0) and (longest_chain.end_index=CircularLabelPositions-1):
	
		% Placing the label to the top is basically achieved by subtracting an angle of 90 degrees
		longest_chain.start_index := longest_chain.start_index - CircularLabelPositions/4;
		longest_chain.end_index := longest_chain.end_index - CircularLabelPositions/4;
	
	fi
	
	% The final relative angle goes into the midpoint of the found longest zero arc
	save angle; angle:= (longest_chain.start_index + longest_chain.end_index)/2;
	
	% The final relative distance is the row index of the longest chain
	save distance; distance:= longest_chain.dimension;
	
	% Return an image
	image
	(			
		% If we're erasing the background
		if erasing:		
			
			% Do it by filling the correctly placed hull with the background color
			fill Position(Scaled(letter.hull,PlacementHullScale), angle, distance) withcolor background;
		fi
				
		% Draw the label at the found position
		draw Position(letter.image, angle, distance);
	)
	
enddef;

%
% Loads a glyph representing the passed letter from the current font (variable 'Font') scaled by 'FontScale'.
% The result is then stored in the variable 'name.picture'. Also, the macro automatically finds the convex
% hull of the letter, which is then stored in the variable 'name.hull'. 
% 
%
vardef Letter(expr letter)(suffix name)=

	% Load the glyph image of the letter
	picture name.image; name.image:=glyph letter of Font scaled FontScale;
	
	% We will sample the letter's path with a specified number of points (LetterPathsSampleSize)
	% Prepare a variable where we will store the current number of them
	save number_of_sample_points; number_of_sample_points:=0;
	
	% Prepare the array where we will store the actual sample points
	save sample_points; pair sample_points[];
	
	% Go through the every inner image of the glyph
	for glyph_part within name.image:
	
		% Get the path part of the current glyph image
		save glyph_path; path glyph_path; glyph_path=pathpart glyph_part;
	
		% Find the requested number of points on the path
		for i=0 upto (LetterPathsSampleSize-1):
		
			% Get the i-th sample point of the path
			sample_points[number_of_sample_points] :=  point (i * length(glyph_path)/LetterPathsSampleSize) of glyph_path;
			
			% Count the sample point in
			number_of_sample_points:= number_of_sample_points+1;
		
		endfor
	
	endfor
	
	% Prepare the array where there will be the final points of the letter's convex hull
	save hull_points; pair hull_points[];	
	
	% Find the convex hull points
	number_of_hull_points := ConvexHull(sample_points, number_of_sample_points, hull_points);
	
	% Create the actual convex hull
	path name.hull; name.hull:= for i=0 upto number_of_hull_points-1: hull_points[i] --- endfor cycle;
	
enddef;

%
% Calculates a numeric value representing how much the tested path intersects the passed box 
% (which is a picture). The more intersections / longer distances between them, the higher
% the returned value (which is units of these distances). The macro doesn't handle when the 
% whole path is inside the box, as it is meant to be used for smaller boxes and larger paths.
%
vardef Badness(expr box, tested_path)=
	
	% We will be finding intersections, declare the needed variables
	save number_of_intersections, intersections; pair intersections[]; 
	
	% Call the helper macro to do the job
	number_of_intersections:= FindIntersectionPoints(box, tested_path)(intersections);
	
	% If there are no intersections...
	if number_of_intersections=0:
	
		% We'll say the result is 0, i.e. it is good. This might be not ideal if the whole 
		% curve is inside the box, but for if it's used for small hulls, it should not happen often
		0
		
	% If there is exactly one intersection
	elseif number_of_intersections=1:
		
		% Then one of the ends of the intersecting paths is inside. 
		save first_end, second_end; pair first_end, second_end;
		
		% Find them
		first_end := point 0 of tested_path;
		second_end := point length tested_path of tested_path;
		
		% If the first is inside
		if IsInBoundingBox(first_end,box):
		
			% Return the distance of the end to this intersection
			abs(first_end-intersections[0])
			
		% If the second is inside
		else:  
		
			% Return the distance of the end to this intersection
			abs(second_end-intersections[0])
		fi                       
	
	% If there are more intersections
	elseif number_of_intersections>1:
	
		% Prepare the result
		save badness; badness:=0;

		% We will count it as the sum of all intersection distances
		for i=0 upto number_of_intersections-1:
			for j=i+1 upto number_of_intersections-1:
				badness:= badness + abs(intersections[i]-intersections[j]);
			endfor
		endfor
		
		% Return the result
		badness
	fi
				
enddef;