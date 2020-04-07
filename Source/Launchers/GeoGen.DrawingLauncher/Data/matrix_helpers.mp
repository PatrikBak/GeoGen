input commons

%
% Finds the longest sequence of zeroes in some row of the passed m x n matrix. If there are more 
% rows with such an sequence, the first one is taken. This macro creates the following values: 
%
% 	result.dimension = the index of the first row with the longest chain, or -1, if there is no such row
%   result.start_index = the start index of the longest chain
%   result.end_index = the end index of the longest chain
%
% Each row is taken cyclically, e.g. the longest chain in the sequence 0 0 1 0 0 0 has a length of 5. Therefore
% it may happen that the end index is smaller than the start index.
%
vardef FindLongestsZeroChainInRow(suffix matrix)(expr m,n)(suffix result) =

	% No result by default
	result.dimension := -1;
	
	% Prepare the currently best length result
	save best_length; best_length := -1;
	
	% Iterate over rows
	for i=0 upto m-1:
	
		% Find the first non-zero value
		save non_zero_index; non_zero_index:=-1;
		
		% Iterate over elements of the current row
		for j=0 upto n-1:		
		
			% If there is a non-zero value
			if matrix[i][j] <> 0:
			
				% Note the column index
				non_zero_index:=j;
				
				% We're done
				break
			fi
			
		endfor
		
		% If all elements are zeroes
		if non_zero_index=-1:
			
			% Then we may stop looking for the longest chain
			result.start_index := 0;
			result.end_index := n-1;
			result.dimension := i;
			
			% And terminate
			break
		fi
		
		% Prepare the variable that will indicate whether the last column index points to a zero,
		% i.e. whether we currently are in a zero chain
		save inside_chain; boolean inside_chain; inside_chain:=false;
		
		% Prepare the value of the current chain length, its start and index indices.
		save current_length, current_start, current_end; current_length := 0;
		
		% We know one non-zero index, so we just need to iterate over the remaning n-1 values
		for j=1 upto n:
		
			% Find the current index by shifting from the non-zero index
			save index; index:= (non_zero_index+j) mod n;
			
			% If we are at a zero value
			if matrix[i][index]=0:
						
				% And we are inside a chain
				if inside_chain:
				
					% Then we may prolong it
					current_length:= current_length+1;
					
					% And mark the current chain end
					current_end:= index;
					
				% If we are not inside a chain
				else:
					
					% Then we've just entered one
					inside_chain:= true;
					
					% We can mark it by changing the local variables appropriately
					current_start:= index;
					current_end:= index;
					current_length:= 1;
				fi
			
			% If we are at a non-zero value and inside a chain...
			elseif inside_chain:
				
				% Then we are no longer inside a chain
				inside_chain:=false;
				
				% If we have broken a longer chain than the best one so far
				if current_length > best_length:
					
					% Mark the currently best length
					best_length:= current_length;
					
					% Rewrite the returned values
					result.start_index:= current_start;
					result.end_index:= current_end;
					result.dimension:= i;
				fi
			fi
			
		endfor
	
	endfor

enddef;

% 
% Finds the minimal value of all elements of the passed m x n matrix.
%
vardef FindMinimum(suffix matrix)(expr m,n) =

	% Prepare the result
	save current_minimum; current_minimum:=infinity;
	
	% Iterate over all elements of the matrix
	for i=0 upto m-1:
		for j=0 upto n-1:
		
			% If the current one is smaller
			if matrix[i][j]<current_minimum:
			
				% Update the current minimum
				current_minimum:= matrix[i][j];
			fi
			
		endfor
	endfor
	
	% Return the found minimum
	current_minimum

enddef;

%
% Decreases all the fields of the passed m x n matrix by a given value.
%
vardef DecreaseAll(suffix matrix)(expr m,n,value) =
	
	% Iterate over all elements of the matrix
	for i=0 upto m-1:
		for j=0 upto n-1:
			
			% Update the current element accordingly
			matrix[i][j]:= matrix[i][j]-value;
		
		endfor
	endfor

enddef;