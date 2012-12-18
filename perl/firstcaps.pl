#!/usr/bin/perl
use strict;
use warnings;

open(FIND, "find |");
while(<FIND>) {
   chomp;
   next if $_ eq $0;

   my $name = lc($_);
   $name =~ s/(\s\w)/\u$1/g;
   print $name,"\n";
}
close FIND;

__END__
