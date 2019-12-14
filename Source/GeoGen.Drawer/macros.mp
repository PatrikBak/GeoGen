u:=5cm;
input TEX;

let old_endfig = endfig;

vardef endfig = 	
	draw bbox(currentpicture) withcolor background;	
	old_endfig
enddef; 

%%%%%%%%%%% TEXT CONVERSION MACROS %%%%%%%%%%%

vardef TexTextOnTheRight(expr text) = 
		
	save p; picture p; p:=TEX("\parskip=10pt\parindent=0pt\hsize=8cm\vbox{" & text & "}");
			
	p shifted (urcorner bbox(currentpicture))
	       shifted -(0,ypart urcorner(bbox(p)))
		
enddef;

def Triangle_Text(expr A,B,C) = 
	"Let~$" & A & B & C & "$ be an acute triangle. "
enddef;

def Midpoint_Text(expr M,A,B) = 
	"Let~$" & M & "$ be the midpoint of~$" & A & B & "$. "		
enddef;

def Incircle_Text(expr c,A,B,C) = 
	"Let~$" & c & "$ be the incircle of~$" & A & B & C & "$. "		
enddef;

def ConcurrentLines_Text(expr p,q,r) =
	"Show that lines~$" & p & "," & q & "," & r & "$ are concurrent. "
enddef;

def TangentCircles_Text(expr k,l) =
	"Show that circles~$" & k & "$ and $" & l & "$ are tangent to each other. "
enddef;

%%%%%%%%%%% HELPER MACROS %%%%%%%%%%%

%
% Returns a simple point mark, i.e. a circle filled with the background color, scaled by a given coefficient.
%
vardef PointMarkScaled(expr point, scale) = 
	
	% Prepare circle
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

%
% Labels a given point so that the label is intelligently placed (the code will be cleaned)
%
vardef LabelPoint(expr label, P) =
	
	bboxmargin:=1;
	save pic; picture pic; pic:=currentpicture;
	save box; path box; box:=bbox label;
	
	minD:= 2+abs((llcorner box) + (urcorner box))/2;
	maxD:= 1.5minD;
	
	n:=40;
	m:=20;
	
	vardef place_it(expr angle, d, erasing) = 
		
		save pp; picture pp; 
		bboxmargin:=2;
		
		pp:=label shifted P
		          shifted (d*cosd(angle),d*sind(angle))
				  shifted (-((llcorner box)+(urcorner box))/2);
		
		if erasing:
		
			fill fullcircle scaled (abs(llcorner(bbox pp) - urcorner(bbox pp))/2)
				shifted ((llcorner bbox(pp) + urcorner bbox(pp))/2)
				withcolor background;
			fi;
		
		draw pp;
				
	enddef;
	
	image(	
		save all_c;
		d_index:=0;		
	
		save placed; boolean placed; placed:=false;
	
		for d=minD step ((maxD-minD)/(m-1)) until maxD:
			
			for k=0 upto n-1:
						
				save new_box; path new_box;
				
				new_box:=bbox(label shifted (P+(d*cosd(k*(360/n)),d*sind(k*(360/n)))
								-((llcorner box)+(urcorner box))/2));
				
				c:=0;
														
				for item within pic:
					
					save p,q,r,z; path p,q,r; pair z[];
		
					p:= new_box;
					q:= pathpart item;					
					r:= p;
					
					i:= 0;
					
					forever:
					
						save a;	
						(a, whatever) = q intersectiontimes r;
						
						if a <>-1:
							zz[i]:=a;
						fi;
					
						r := r cutbefore q;
						exitif length cuttings = 0;
						
						
						factor:=epsilon;
						
						forever: 
							z[i]:= (point 0 of r);
							r := subpath(factor, length r) of r;	
							factor:= 2factor;
							exitif i=0;
							exitif abs(z[i-1]-z[i]) > eps;
						endfor;
						
						i := i + 1;
					endfor;
					
					vardef is_inside(expr p) = 
					    ((xpart p >= xpart (ulcorner new_box)) and
						( xpart p <= xpart (urcorner new_box)) and
						( ypart p >= ypart (llcorner new_box)) and
						( ypart p <= ypart (ulcorner new_box)))
					enddef;
					
					if i=0:
						if is_inside(point (length q)/2 of q):
							c:= c + abs((point 0 of q) - (point length q of q));
						fi;
					
					elseif i=1:
						
						% We need to find out which endpoint of the path
						% is inside. 
						save ends; pair ends[];
						
						% Get them first
						ends[0] = point 0 of q;
						ends[1] = point infinity of q;
						
										
						if (is_inside(ends[0])):
							c:= c + abs(ends[0]-z[0]);
						else:                       
							c:= c + abs(ends[1]-z[0]);
						fi;	                        
						                            
					else:						    
						for j=0 upto (i-2):         
							c:= c + abs(z[j]-z[j+1]);
						endfor;						
					fi;		
				
				endfor;
						
				all_c[d_index][k]:=c;
						
				if all_c[d_index][k] < 0:
					all_c[d_index][k]:=0;
				fi;
									
			endfor;
						
			save smallest; smallest:=infinity; 
			save last_non_zero_index; last_non_zero_index:=-1;
			
			for i=0 upto n-1:
			
				if all_c[d_index][i] < smallest:
					smallest:=all_c[d_index][i];
				fi;
				
				if all_c[d_index][i] > 0:
					last_non_zero_index:=i;
				fi;
				
			endfor;
						
			if last_non_zero_index=-1:
				place_it(90,d,false);
				placed:=true;
				exitif true;
			fi;
			
			if smallest=0:
						
				i:=last_non_zero_index;
				
				forever:
					
					exitif all_c[d_index][i] = 0;
					
					i:=i+1;
					if i=n:
						i:=0;
					fi;					
					
				endfor;
								
				longest:=0;
				longest_start:=0;
				longest_end:=0;
				
				save in; boolean in; in:=false;
				current_length := 0;
				current_start := 0;
				current_end := 0;
				
				j:=i;
				
				forever:
								
					if all_c[d_index][j]=0:
						
						if in:
							current_length:=current_length+1;
							current_end:=j;
						else:
							in:=true;
							current_start:=j;
							current_end:=j;
							current_length:=1;
						fi;
					else:
						
						if in:
							in:=false;
							
							if current_length > longest:
								longest:= current_length;
								longest_start:= current_start;
								longest_end:= current_end;
							fi;
						fi;
					fi;
						
					
					j:=j+1;
					if j=n:
						j:=0;
					fi;
					
					exitif j=i;
				
				endfor;
								
				if longest_start > longest_end:
					longest_end:=longest_end + n;
				fi;
				
				place_it((360/n)*(longest_start+longest_end)/2,d,false);
				placed:=true;				
				exitif true;				
			fi;
			
			d_index:=d_index+1;
		
		endfor;
		
		if not placed:
			
			minimal:=infinity;
			min_d:=-1;
			min_k:=-1;
			
			for d=0 upto (d_index-1):
				for k=0 upto n-1:
					if all_c[d][k] < minimal:
						min_d:=d;
						min_k:=k;
						minimal:=all_c[d][k];
					fi;
				endfor;
			endfor;
			
			place_it((360/n)*min_k,minD+min_d*(maxD-minD)/(m-1),true);
						
		fi;
	)
	
enddef;

%%%%%%%%%%% LINE STYLES %%%%%%%%%%%

def WithAuxiliaryStyle = 
	withpen pencircle scaled 0.5
	withdots
enddef;


def WithNormalStyle =
	withpen pencircle scaled 0.5
enddef;


def WithTheoremStyle = 
	withpen pencircle scaled 0.5
	withcolor red
enddef;

%%%%%%%%%%% DRAWING MACROS %%%%%%%%%%%


def CircleAuxiliaryStyle(expr center, radius) = 
	CircleWithRadius(center, radius) WithAuxiliaryStyle
enddef;


def CircleNormalStyle(expr O, r) = 
	CircleWithRadius(O, r) WithNormalStyle
enddef;


def CircleTheoremStyle(expr O, r) = 
	CircleWithRadius(O, r) WithTheoremStyle
enddef;


def LineSegmentAuxiliaryStyle(expr A, B) = 
	A--B WithAuxiliaryStyle
enddef;


def LineSegmentNormalStyle(expr A, B) = 
	A--B 
enddef;


def LineSegmentTheoremStyle(expr A, B) = 
	A--B WithTheoremStyle
enddef;


def PointMarkAuxiliaryStyle(expr P) = 
	PointMarkScaled(P, 1.5)
enddef;


def PointMarkNormalStyle(expr P) = 
	PointMarkScaled(P, 2.4)
enddef;


def PointMarkTheoremStyle(expr P) = 
	PointMarkScaled(P, 2.4) withcolor red
enddef; 