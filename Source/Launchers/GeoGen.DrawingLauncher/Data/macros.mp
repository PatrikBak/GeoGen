input language
input labels
input TEX

%%%%%%%%%%% BASIC SETUP %%%%%%%%%%%

% 
% The basic unit of length by each every point is scaled
%
u:=5cm;

%
% The text width of the TeX problem statement on the right of the picture
%
RightTextWidth := 9cm;

%
% The value by which the text on the right is shifted to the right from the right picture end
%
RightTextShift := 8;

% 
% We will redefine the endfig macro so let us save its original definition
%
let old_endfig = endfig;

% 
% Redefine the endfig macro so that it adds more breathing space to the picture and text.
% This is done because of the value of bboxmargin which is by default not zero.
%
vardef endfig = 	

	% Draw the colorless border which will increase the total picture size by a bit
	draw bbox(currentpicture) withcolor background;	
	
	% Perform the save old meaning of the endfig macro
	old_endfig
	
enddef; 

%%%%%%%%%%% TEXT CONVERSION MACROS %%%%%%%%%%%

%
% Returns a string compilable by PlainTeX that represents a ranking table. This table has four columns:
% Type, Ranking, Weight, Contribution. Its items are supposed to be provided as strings (whose number is 
% naturally divisible by 4).
%
vardef RankingTable(text table_items) =

	% Prepare the resulting string
	save result; string result; 
	
	% Start composing the table by making everything centered
	result := "\tabskip=0pt\halign to \hsize{"
		& "\tabskip0pt plus1fil\hfil#\hfil&"
		& "\tabskip0pt plus1fil\hfil#\hfil&"
		& "\tabskip0pt plus1fil\hfil#\hfil&"
		& "\tabskip0pt\hfil#\hfil\cr "
		
		% Add the header
		& "Type&Ranking&Weight&Contribution\cr "
		
		% Add a thick line
		& "\noalign{\vskip2pt\hrule\vskip3pt}";
	
	% Prepare the counter of already processed items
	counter := 0;
	
	% Go through the individual cells of the table
	for item = table_items:

		% Append the current item to the result
		result := result & item;
		
		% Every fourth item 
		if (counter mod 4) = 3:
			
			% Should end a line with a \cr
			result := result & "\cr ";
		
		% And other items
		else:
		
			% Should be separated by &
			result := result & "&";
		fi
		
		% Count in the cell
		counter:= counter+1;	
		
	endfor
		
	% Return the enclosed result
	result & "}"
	
enddef;

%
% Places a given TeX string to the right of the picture.
%
vardef TexTextOnTheRight(expr text) = 

	% Setup some initial TeX settings. First load opmac
	TEXPRE("\input opmac" & char(10) 
	
		% So we can easily use a better font
		& "\fontfam [LModern]" & char(10) 
				
		% Higher paragraph skip
		& "\parskip=10pt" & char(10) 
		
		% No indent
		& "\parindent=0pt" & char(10)
		
		% And the width equal to the preferred value
		& "\hsize=" & decimal(RightTextWidth) & "pt" & char(10));
	
	% Prepare the resulting picture
	save result; picture result; 
	
	% Add the text to it
	result:=TEX("\vbox{" & text & "}");
	
	% Prepare the result shifted to the right a bit
	result shifted (RightTextShift, 0)
		   
		   % And even more by the current right picture corner
		   shifted urcorner currentpicture
	       
		   % And also veritically so it matches the right corner
		   shifted (0, -ypart urcorner(result))
		
enddef;

%%%%%%%%%%% DRAWING HELPERS %%%%%%%%%%%

%
% Returns a simple point mark, i.e. a circle filled with the background color, scaled by a given coefficient.
%
vardef PointMarkScaled(expr point, scale) = 
	
	% Prepare a circle
	save circle; path circle; circle = fullcircle scaled scale shifted point;
	
	% Fill it
	fill circle withcolor background;
	
	% Return it
	circle

enddef;

%
% Returns the circle centered at O with given radius
%
vardef CircleWithRadius(expr O, r) = fullcircle scaled (2r) shifted O enddef;

%%%%%%%%%%% LINE STYLES %%%%%%%%%%%

%
% The drawing style that is used for auxiliary lines.
%
def WithAuxiliaryStyle = 
	withpen pencircle scaled 0.5 dashed withdots scaled 0.3
enddef;

%
% The drawing style that is used for neutral lines.
%
def WithNormalStyle =
	withpen pencircle scaled 0.5
enddef;

%
% The drawing style that is used for theorem lines.
%
def WithTheoremStyle = 
	withpen pencircle scaled 0.5
	withcolor red
enddef;

%%%%%%%%%%% DRAWING MACROS %%%%%%%%%%%

%
% Draws a circle with a given center and radius and the auxiliary line style.
%
def CircleAuxiliaryStyle(expr center, radius) = 
	CircleWithRadius(center, radius) WithAuxiliaryStyle
enddef;

%
% Draws a circle with a given center and radius and the normal line style.
%
def CircleNormalStyle(expr O, r) = 
	CircleWithRadius(O, r) WithNormalStyle
enddef;

%
% Draws a circle with a given center and radius and the theorem line style.
%
def CircleTheoremStyle(expr O, r) = 
	CircleWithRadius(O, r) WithTheoremStyle
enddef;

%
% Draws a line segment given by two points and the auxiliary line style.
%
def LineSegmentAuxiliaryStyle(expr A, B) = 
	A--B WithAuxiliaryStyle
enddef;

%
% Draws a line segment given by two points and the normal line style.
%
def LineSegmentNormalStyle(expr A, B) = 
	A--B 
enddef;

%
% Draws a line segment given by two points and the theorem line style.
%
def LineSegmentTheoremStyle(expr A, B) = 
	A--B WithTheoremStyle
enddef;

%
% Draws a point mark of a given point with the auxiliary style.
%
def PointMarkAuxiliaryStyle(expr P) = 
	PointMarkScaled(P, 1.5)
enddef;

%
% Draws a point mark of a given point with the normal style.
%
def PointMarkNormalStyle(expr P) = 
	PointMarkScaled(P, 2.4)
enddef;

%
% Draws a point mark of a given point with the theorem style.
%
def PointMarkTheoremStyle(expr P) = 
	PointMarkScaled(P, 2.4) withcolor red
enddef; 